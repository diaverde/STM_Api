using System.Data.SQLite;
using StmApi.Data;

namespace StmApi
{
    class SqliteContext : IDbContext
    {
        private SQLiteConnection sqlite_conn;

        public SqliteContext(string connectionString)
        {
            sqlite_conn = new SQLiteConnection(connectionString);
        }
        public void OpenConnection()
        {
            try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Problem opening database: {ex.Message}");
            }
        }

        public void CloseConnection()
        {
            sqlite_conn.Close();
        }

        public List<Service> GetServices()
        {
            List<Service> services = new List<Service>();
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT service_id,monday,tuesday,wednesday,thursday,friday,saturday,sunday,start_date,end_date FROM services;";

            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                Service service = new Service
                {
                    Id = sqlite_datareader.GetString(0),
                    Monday = sqlite_datareader.GetString(1),
                    Tuesday = sqlite_datareader.GetString(2),
                    Wednesday = sqlite_datareader.GetString(3),
                    Thursday = sqlite_datareader.GetString(4),
                    Friday = sqlite_datareader.GetString(5),
                    Saturday = sqlite_datareader.GetString(6),
                    Sunday = sqlite_datareader.GetString(7),
                    StartDate = sqlite_datareader.GetString(8),
                    EndDate = sqlite_datareader.GetString(9)
                };
                //Console.WriteLine(JsonSerializer.Serialize(service));
                services.Add(service);
            }

            return services;
        }

        public List<Route> GetRoutes()
        {
            List<Route> routes = new List<Route>();
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT route_id,agency_id,route_short_name,route_long_name,route_type,route_url,route_color,route_text_color FROM routes;";
            
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                Route route = new Route
                {
                    Id = sqlite_datareader.GetInt32(0),
                    AgencyId = sqlite_datareader.GetString(1),
                    ShortName = sqlite_datareader.GetString(2),
                    LongName = sqlite_datareader.GetString(3),
                    Type = sqlite_datareader.GetString(4),
                    Url = sqlite_datareader.GetString(5),
                    Color = sqlite_datareader.GetString(6),
                    TextColor = sqlite_datareader.GetString(7)
                };
                routes.Add(route);
            }

            return routes;
        }

        public Route? GetRoute(int id)
        {
            Route? route = null;
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT route_id,agency_id,route_short_name,route_long_name,route_type,route_url,route_color,route_text_color FROM routes WHERE route_id = $id;";
            sqlite_cmd.Parameters.AddWithValue("$id", id);

            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                route = new Route
                {
                    Id = sqlite_datareader.GetInt32(0),
                    AgencyId = sqlite_datareader.GetString(1),
                    ShortName = sqlite_datareader.GetString(2),
                    LongName = sqlite_datareader.GetString(3),
                    Type = sqlite_datareader.GetString(4),
                    Url = sqlite_datareader.GetString(5),
                    Color = sqlite_datareader.GetString(6),
                    TextColor = sqlite_datareader.GetString(7)
                };
            }

            return route;
        }

        public List<CustomTrip> GetRouteTrips(int id, string day, string direction, bool limit)
        {
            string today = DateTime.Now.ToString("yyyyMMdd");
            //string today = "20250401";
            List<CustomTrip> routeTrips = new List<CustomTrip>();
            string sortAndLimit = "ORDER BY st.departure_time, st.stop_sequence;";
            if (limit)
            {
                sortAndLimit = "ORDER BY st.departure_time ASC, st.stop_sequence ASC LIMIT 1;";
            }

            readData();

            if (limit)
            {
                sortAndLimit = "ORDER BY st.departure_time DESC, st.stop_sequence DESC LIMIT 1;";
                readData();
            }

            return routeTrips;

            // Helper
            void readData() 
            {
                string sqlStatement = "SELECT t.route_id, t.service_id, t.trip_id, t.trip_headsign, t.direction_id, t.shape_id, "
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
                + sortAndLimit;

                SQLiteDataReader sqlite_datareader;
                SQLiteCommand sqlite_cmd;
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = sqlStatement;
                sqlite_cmd.Parameters.AddWithValue("$route_id", id);
                sqlite_cmd.Parameters.AddWithValue("$week_day", $"%-{day}");
                sqlite_cmd.Parameters.AddWithValue("$direction", direction);
                sqlite_cmd.Parameters.AddWithValue("$today", today);

                sqlite_datareader = sqlite_cmd.ExecuteReader();
                while (sqlite_datareader.Read())
                {
                    CustomTrip trip = new CustomTrip
                    {
                        RouteId = sqlite_datareader.GetInt32(0),
                        ServiceId = sqlite_datareader.GetString(1),
                        TripId = sqlite_datareader.GetInt32(2),
                        TripHeadsign = sqlite_datareader.GetString(3),
                        Direction = sqlite_datareader.GetString(4),
                        ShapeId = sqlite_datareader.GetInt32(5),
                        RouteShortName = sqlite_datareader.GetString(6),
                        RouteLongName = sqlite_datareader.GetString(7),
                        RouteType = sqlite_datareader.GetString(8),
                        ArrivalTime = sqlite_datareader.GetString(9),
                        DepartureTime = sqlite_datareader.GetString(10),
                        StopId = sqlite_datareader.GetString(11),
                        StopSequence = sqlite_datareader.GetInt32(12),
                        StopName = sqlite_datareader.GetString(13),
                        Latitude = sqlite_datareader.GetString(14),
                        Longitude = sqlite_datareader.GetString(15),
                        LocationType = sqlite_datareader.GetString(16),
                        ParentStation = sqlite_datareader.GetString(17)
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