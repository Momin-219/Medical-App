using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;

public static class PdfGenerator
{
    public static void Generate(PatientData data, string filePath)
    {
        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(50);
                page.Header().Element(headerContainer => ComposeHeader(headerContainer, data));
                page.Content().Element(contentContainer => ComposeContent(contentContainer, data));
                page.Footer().AlignCenter().Text(x => { x.Span("Page "); x.CurrentPageNumber(); });
            });
        }).GeneratePdf(filePath);
    }

    private static void ComposeHeader(IContainer container, PatientData data)
    {
        var titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text($"Diagnosis Report").Style(titleStyle);
                column.Item().Text($"Patient ID: {data.PatientId}");
                column.Item().Text(System.DateTime.Now.ToString("g"));
            });
        });
    }

    private static void ComposeContent(IContainer container, PatientData data)
    {
        container.PaddingVertical(30).Column(column =>
        {
            column.Spacing(25); // Increased spacing between sections

            column.Item().Element(c => ComposeSection(c, "Patient Information", table =>
            {
                table.Cell().Text("Full Name");
                table.Cell().Text(data.PatientName ?? "N/A");
                table.Cell().Text("Age");
                table.Cell().Text(data.Age ?? "N/A");
                table.Cell().Text("Symptoms");
                table.Cell().Text(data.Symptoms ?? "N/A");
            }));

            column.Item().Element(c => ComposeSection(c, "Diagnosis Details", table =>
            {
                foreach (var selection in data.Selections)
                {
                    table.Cell().Text(selection.Key);
                    table.Cell().Text(selection.Value ?? "N/A");
                }
            }));

            // --- NEW: Diagnosis Result Section in PDF ---
            column.Item().Element(c =>
            {
                c.Background(Colors.Grey.Lighten4).Padding(10).Column(col =>
                {
                    col.Item().Text("Final Diagnosis").FontSize(14).Bold();
                    col.Item().PaddingTop(5).Text(data.DiagnosisResult);
                });
            });
        });
    }

    private static void ComposeSection(IContainer container, string title, System.Action<TableDescriptor> tableContent)
    {
        container.ShowOnce().Border(1).BorderColor(Colors.Grey.Lighten2).Column(column =>
        {
            column.Item().Background(Colors.Grey.Lighten4).Padding(5).Text(title).Bold();
            column.Item().Padding(5).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(150);
                    columns.RelativeColumn();
                });
                tableContent(table);
            });
        });
    }
}