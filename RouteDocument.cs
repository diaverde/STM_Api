using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace StmApi
{
    public class RouteDocument : IDocument
    {
        public List<CustomTrip> Trips { get; }

        public RouteDocument(List<CustomTrip> trips)
        {
            Trips = trips;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Margin(50);
                    page.Size(PageSizes.A1.Landscape());
                    //page.Size(PageSizes.A1);
                
                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
        }

        void ComposeHeader(IContainer container)
        {
            string routeNumber = Trips.First().RouteShortName ?? string.Empty;
            string routeName = Trips.First().RouteLongName ?? string.Empty;
            string direction = Trips.First().TripHeadsign ?? string.Empty;
            string weekday = string.Empty;
            if (Trips.First().ServiceId!.EndsWith("A"))
                weekday = "Samedi";
            else if (Trips.First().ServiceId!.EndsWith("I"))
                weekday = "Dimanche";
            else
                weekday = "Semaine";

            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item()
                        .Text($"Route #{routeNumber}")
                        .FontSize(30).Bold().FontColor(Colors.Blue.Medium);

                    column.Item()
                        .Text(routeName)
                        .FontSize(24).SemiBold().FontColor(Colors.Grey.Darken4);
                });

                row.RelativeItem().Column(column =>
                {
                    column.Item()
                        .Text($"Weekday: {weekday}")
                        .FontSize(20);

                    column.Item()
                        .Text($"Direction: {direction}")
                        .FontSize(20);
                });

                //row.ConstantItem(100).Height(50).Placeholder();
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(40).Column(column =>
            {
                column.Spacing(5);

                column.Item().Element(ComposeTable);
            });
        }

        void ComposeTable(IContainer container)
        {
            var groupedTrips = Trips.GroupBy(t => t.TripId).ToList();
            int tripCount = groupedTrips.Count;
            Dictionary<string, string?> stops = groupedTrips
                .SelectMany(g => g.Select(t => new { t.StopId, t.StopName }))
                .Distinct()
                .ToDictionary(x => x.StopId, x => x.StopName);
            int stopCount = stops.Count;

            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(25);
                    for (int i = 0; i < stopCount; i++)
                    {
                        columns.RelativeColumn();
                    }
                });
                
                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Trip");
                    for (int i = 0; i < stopCount; i++)
                    {
                        header.Cell().Element(CellStyle).Text($"{stops.ElementAt(i).Value}");
                    }
                    
                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold())
                            .RotateLeft()
                            .PaddingHorizontal(5)
                            .BorderLeft(1)
                            .BorderColor(Colors.Black);
                    }
                });
                
                for (int i = 0; i < groupedTrips.Count; i++)
                {
                    table.Cell().Element(TimeCellStyle).Text($"{i + 1}");

                    foreach (var stop in stops)
                    {
                        CustomTrip? stopForTrip = groupedTrips[i].FirstOrDefault(t => t.StopId == stop.Key);
                        if (stopForTrip is not null)
                        {
                            table.Cell().Element(TimeCellStyle).Text(stopForTrip.DepartureTime);
                        }
                        else
                        {
                            table.Cell().Element(TimeCellStyle).Text(string.Empty);
                        }
                    }
                }
                
                static IContainer TimeCellStyle(IContainer container)
                {
                    return container
                        .BorderBottom(1)
                        .BorderColor(Colors.Grey.Lighten2)
                        .PaddingVertical(5);
                }
            });
        }
    }
}