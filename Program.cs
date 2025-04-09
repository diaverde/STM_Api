using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using StmApi;
using Route = StmApi.Route;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IStmService, StmService>();
builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin()
        .AllowAnyMethod()
       .AllowAnyHeader();
}));

var app = builder.Build();

// Configure exception middleware
app.UseStatusCodePages(async statusCodeContext
    => await Results.Problem(statusCode: statusCodeContext.HttpContext.Response.StatusCode)
        .ExecuteAsync(statusCodeContext.HttpContext));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("MyPolicy");
app.UseHttpsRedirection();


// Example
app.MapGet("/", () => "Hello my friend, we meet again!");

// Services
app.MapGet("/services", (IStmService stmService) => 
    TypedResults.Ok(stmService.GetServices()))
        .WithName("GetServices")
        .WithOpenApi(x => new OpenApiOperation(x)
        {
            Summary = "Get STM Services",
            Description = "Returns information about the available services during a time frame.",
            Tags = new List<OpenApiTag> { new() { Name = "STM Information" } }
        });

// Routes
app.MapGet("/routes", (IStmService stmService) =>
    TypedResults.Ok(stmService.GetRoutes()))
        .WithName("GetRoutes")
        .WithOpenApi(x => new OpenApiOperation(x)
        {
            Summary = "Get STM Routes",
            Description = "Returns information about the available routes.",
            Tags = new List<OpenApiTag> { new() { Name = "STM Information" } }
        });

// Route
app.MapGet("/route/{id}", Results<Ok<Route>, NotFound> (IStmService stmService, int id) =>
    stmService.GetRoute(id) is { } route 
        ? TypedResults.Ok(route)
        : TypedResults.NotFound())
    .WithName("GetRouteById")
    .WithOpenApi(x => new OpenApiOperation(x)
    {
        Summary = "Get STM Route Id",
        Description = "Returns information about a route number.",
        Tags = new List<OpenApiTag> { new() { Name = "STM Information" } }
    });

// Services
app.MapGet("/trips/{id}", (
    IStmService stmService, 
    int id, 
    [FromQuery(Name = "day")] string? day,
    [FromQuery(Name = "dir")] string? direction,
    [FromQuery(Name = "lim")] bool? limit) => 
    TypedResults.Ok(stmService.GetRouteTrips(id, day ?? "S", direction ?? "0", limit ?? false)))
        .WithName("GetTrips")
        .WithOpenApi(x => new OpenApiOperation(x)
        {
            Summary = "Get STM Trips for a route",
            Description = "Returns information about the trips for a given route.",
            Tags = new List<OpenApiTag> { new() { Name = "STM Information" } }
        });

app.MapGet("/pdf/{id}", (
    IStmService stmService,
    int id,
    [FromQuery(Name = "day")] string? day,
    [FromQuery(Name = "dir")] string? direction) => 
    {
        string mimeType = "application/pdf";
        string fileName = $"{id}-{day}-{direction}.pdf";
        string filePath = Path.Combine("Files", fileName);

        if (!File.Exists(filePath))
        {
            var file = stmService.GetRoutePdf(id, day ?? "S", direction ?? "0");
        }

        if (!File.Exists(filePath))
        {
            return Results.NotFound("File not found.");
        }
        else
        {
            var fileBytes = File.ReadAllBytes(filePath);
            return Results.File(fileBytes, mimeType, fileName);
        }
    })
        .WithName("GetRoutePdf")
        .WithOpenApi(x => new OpenApiOperation(x)
       {
            Summary = "Get STM pdf for a route",
            Description = "Returns a pdf file with information about the trips for a given route.",
            Tags = new List<OpenApiTag> { new() { Name = "STM Information" } }
        });

app.Run();