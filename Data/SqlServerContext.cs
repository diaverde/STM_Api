using System.Data.SqlClient;
using StmApi.Data;

namespace StmApi
{
    class SqlServerContext : IDbContext
    {
        //public string ConnectionString = "Data Source=stmDB.sqlite;Version=3;New=True;Compress=True;";
        //private SqlConnection db_conn {get; set;} = new SqlConnection(connectionString);
        private SqlConnection db_conn;

        public SqlServerContext(string connectionString)
        {
            db_conn = new SqlConnection(connectionString);
        }

        public void OpenConnection()
        {
            try
            {
                db_conn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Problem opening database: {ex.Message}");
            }
        }

        public void CloseConnection()
        {
            db_conn.Close();
        }

        public List<Service> GetServices()
        {
            List<Service> services = new List<Service>();
            SqlDataReader dataReader;
            SqlCommand cmd;
            cmd = db_conn.CreateCommand();
            cmd.CommandText = "SELECT service_id,monday,tuesday,wednesday,thursday,friday,saturday,sunday,start_date,end_date FROM services;";

            dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                Service service = new Service
                {
                    Id = dataReader.GetString(0),
                    Monday = dataReader.GetString(1),
                    Tuesday = dataReader.GetString(2),
                    Wednesday = dataReader.GetString(3),
                    Thursday = dataReader.GetString(4),
                    Friday = dataReader.GetString(5),
                    Saturday = dataReader.GetString(6),
                    Sunday = dataReader.GetString(7),
                    StartDate = dataReader.GetString(8),
                    EndDate = dataReader.GetString(9)
                };
                //Console.WriteLine(JsonSerializer.Serialize(service));
                services.Add(service);
            }

            return services;
        }

        public List<Route> GetRoutes()
        {
            List<Route> routes = new List<Route>();
            SqlDataReader dataReader;
            SqlCommand cmd;
            cmd = db_conn.CreateCommand();
            cmd.CommandText = "SELECT route_id,agency_id,route_short_name,route_long_name,route_type,route_url,route_color,route_text_color FROM routes;";
            
            dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                Route route = new Route
                {
                    Id = dataReader.GetInt32(0),
                    AgencyId = dataReader.GetString(1),
                    ShortName = dataReader.GetString(2),
                    LongName = dataReader.GetString(3),
                    Type = dataReader.GetString(4),
                    Url = dataReader.GetString(5),
                    Color = dataReader.GetString(6),
                    TextColor = dataReader.GetString(7)
                };
                routes.Add(route);
            }

            return routes;
        }

        public Route? GetRoute(int id)
        {
            Route? route = null;
            SqlDataReader dataReader;
            SqlCommand cmd;
            cmd = db_conn.CreateCommand();
            cmd.CommandText = "SELECT route_id,agency_id,route_short_name,route_long_name,route_type,route_url,route_color,route_text_color FROM routes WHERE route_id = $id;";
            cmd.Parameters.AddWithValue("$id", id);

            dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                route = new Route
                {
                    Id = dataReader.GetInt32(0),
                    AgencyId = dataReader.GetString(1),
                    ShortName = dataReader.GetString(2),
                    LongName = dataReader.GetString(3),
                    Type = dataReader.GetString(4),
                    Url = dataReader.GetString(5),
                    Color = dataReader.GetString(6),
                    TextColor = dataReader.GetString(7)
                };
            }

            return route;
        }

        public List<CustomTrip> GetRouteTrips(int id, string day, string direction, bool limit)
        {
            string today = DateTime.Now.ToString("yyyyMMdd");
            //string today = "20250401";
            List<CustomTrip> routeTrips = new List<CustomTrip>();
            string sortSuffix = "ORDER BY st.departure_time, st.stop_sequence;";
            string limitPrefix = "SELECT ";
            if (limit)
            {
                limitPrefix += "TOP 1 ";
                sortSuffix = "ORDER BY st.departure_time ASC, st.stop_sequence ASC;";
            }

            readData();

            if (limit)
            {
                sortSuffix = "ORDER BY st.departure_time DESC, st.stop_sequence DESC;";
                readData();
            }

            return routeTrips;

            // Helper
            void readData() 
            {
                string sqlStatement = limitPrefix
                + "t.route_id, t.service_id, t.trip_id, t.trip_headsign, t.direction_id, t.shape_id, "
                + "r.route_short_name, r.route_long_name, r.route_type, "
                + "st.arrival_time, st.departure_time, st.stop_id, st.stop_sequence, "
                + "s.stop_name, s.stop_lat, s.stop_lon, s.location_type, s.parent_station "
                + "FROM trips AS t, routes AS r, stop_times AS st, stops AS s, services AS ser "
                + "WHERE t.route_id = $route_id "
                + "AND $today BETWEEN ser.start_date AND ser.end_date "
                + "AND t.service_id = ser.service_id "
                + "AND t.service_id LIKE $week_day "
                + "AND t.direction_id = $direction "
                + "AND t.route_id = r.route_id "
                + "AND t.trip_id = st.trip_id "
                + "AND st.stop_id = s.stop_id "
                + sortSuffix;

                SqlDataReader dataReader;
                SqlCommand cmd;
                cmd = db_conn.CreateCommand();
                cmd.CommandText = sqlStatement;
                cmd.Parameters.AddWithValue("$route_id", id);
                cmd.Parameters.AddWithValue("$week_day", $"%-{day}");
                cmd.Parameters.AddWithValue("$direction", direction);
                cmd.Parameters.AddWithValue("$today", today);

                dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    CustomTrip trip = new CustomTrip
                    {
                        RouteId = dataReader.GetInt32(0),
                        ServiceId = dataReader.GetString(1),
                        TripId = dataReader.GetInt32(2),
                        TripHeadsign = dataReader.GetString(3),
                        Direction = dataReader.GetString(4),
                        ShapeId = dataReader.GetInt32(5),
                        RouteShortName = dataReader.GetString(6),
                        RouteLongName = dataReader.GetString(7),
                        RouteType = dataReader.GetString(8),
                        ArrivalTime = dataReader.GetString(9),
                        DepartureTime = dataReader.GetString(10),
                        StopId = dataReader.GetString(11),
                        StopSequence = dataReader.GetInt32(12),
                        StopName = dataReader.GetString(13),
                        Latitude = dataReader.GetString(14),
                        Longitude = dataReader.GetString(15),
                        LocationType = dataReader.GetString(16),
                        ParentStation = dataReader.GetString(17)
                    };

                    int secondsSeparatorIndex = trip.DepartureTime.LastIndexOf(':');
                    if (secondsSeparatorIndex != -1)
                    {
                        trip.DepartureTime = trip.DepartureTime.Remove(secondsSeparatorIndex);
                    }
                    secondsSeparatorIndex = trip.ArrivalTime.LastIndexOf(':');
                    if (secondsSeparatorIndex != -1)
                    {
                        trip.ArrivalTime = trip.ArrivalTime.Remove(secondsSeparatorIndex);
                    }
                    routeTrips.Add(trip);
                }
            }
        }
    }
}