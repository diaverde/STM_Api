namespace StmApi
{
    public class Route
    {
        public int Id {get; set;}
        public string? AgencyId {get; set;}

        public string? ShortName {get; set;}
        public string? LongName {get; set;}
        public string? Type {get; set;}
        public string? Url {get; set;}
        public string? Color {get; set;}
        public string? TextColor {get; set;}
    }

    public class Stop
    {
        public int Id {get; set;}
        public string? Code {get; set;}

        public string? Name {get; set;}
        public string? Latitude {get; set;}
        public string? Longitude {get; set;}
        public string? Url {get; set;}
        public string? Type {get; set;}
        public string? ParentStation {get; set;}
        public string? WheelChairBoarding {get; set;}
    }

    public class Service
    {
        public string? Id {get; set;}
        public string? Monday {get; set;}

        public string? Tuesday {get; set;}
        public string? Wednesday {get; set;}
        public string? Thursday {get; set;}
        public string? Friday {get; set;}
        public string? Saturday {get; set;}
        public string? Sunday {get; set;}
        public string? StartDate {get; set;}
        public string? EndDate {get; set;}
    }

    public class CustomTrip
    {
        public int RouteId {get; set;}
        public string? ServiceId {get; set;}

        public int TripId {get; set;}
        public string? TripHeadsign {get; set;}
        public string? Direction {get; set;}
        public int ShapeId {get; set;}
        public string? RouteShortName {get; set;}
        public string? RouteLongName {get; set;}
        public string? RouteType {get; set;}
        public string? ArrivalTime {get; set;}
        public string? DepartureTime {get; set;}
        public required string StopId {get; set;}
        public int StopSequence {get; set;}
        public string? StopName {get; set;}
        public string? Latitude {get; set;}
        public string? Longitude {get; set;}
        public string? LocationType {get; set;}
        public string? ParentStation {get; set;}
    }
}