namespace StmApi.Services;
public interface IStmService
{
    List<Service> GetServices();
    List<Route> GetRoutes();
    Route? GetRoute(int id);
    List<CustomTrip> GetRouteTrips(int routeId, string day, string direction, bool limit);
    Stream GetRoutePdf(int routeId, string day, string direction);
}