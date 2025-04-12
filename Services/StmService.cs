using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using StmApi.Data;

namespace StmApi.Services;
public class StmService : IStmService
{
    private readonly IDbContext _dbContext;

    public StmService()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        string connectionString = configuration.GetConnectionString("DefaultConnection");
        _dbContext = new SqliteContext(connectionString);
        //_dbContext = new SqlServerContext();
    }

    public List<Service> GetServices()
    {
        _dbContext.OpenConnection();
        List<Service> services = _dbContext.GetServices();
        _dbContext.CloseConnection();
            
        return services;
    }

    public List<Route> GetRoutes()
    {
        _dbContext.OpenConnection();
        List<Route> routes = _dbContext.GetRoutes();
        _dbContext.CloseConnection();
            
        return routes;
    }

    public Route? GetRoute(int id)
    {
        _dbContext.OpenConnection();
        Route? route = _dbContext.GetRoute(id);
        _dbContext.CloseConnection();
            
        return route;
    }

    public List<CustomTrip> GetRouteTrips(int routeId, string day, string direction, bool limit)
    {
        _dbContext.OpenConnection();
        List<CustomTrip> trips = _dbContext.GetRouteTrips(routeId, day, direction, limit);
        _dbContext.CloseConnection();
            
        return trips;
    }

    public Stream GetRoutePdf(int routeId, string day, string direction)
    {
        /*
        QuestPDF.Settings.License = LicenseType.Community;
        var invoice = InvoiceDocumentDataSource.GetInvoiceDetails();
        var document = new InvoiceDocument(invoice);
        document.GeneratePdfAndShow();
        //document.GeneratePdf("invoice.pdf");
        */
        _dbContext.OpenConnection();
        List<CustomTrip> trips = _dbContext.GetRouteTrips(routeId, day, direction, false);
        _dbContext.CloseConnection();

        QuestPDF.Settings.License = LicenseType.Community;
        var document = new RouteDocument(trips);
        //document.GeneratePdfAndShow();

        string fileName = $"{routeId}-{day}-{direction}.pdf";
        string filePath = Path.Combine("Files", fileName);
        using var stream = new FileStream(filePath, FileMode.Create);
        document.GeneratePdf(stream);

        return stream;
    }
}