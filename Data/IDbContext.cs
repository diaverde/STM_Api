namespace StmApi.Data
{
    interface IDbContext
    {
        public void OpenConnection();

        public void CloseConnection();

        public List<Service> GetServices();

        public List<Route> GetRoutes();

        public Route? GetRoute(int id);

        public List<CustomTrip> GetRouteTrips(int id, string day, string direction, bool limit);
    }
}