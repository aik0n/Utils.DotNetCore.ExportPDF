using PuppeteerSharp;
using PuppeteerSharp.Media;
using Utils.DotNetCore.ExportPDF;

namespace ExportPDF.WebSample
{
    /// <summary>
    /// STAGE 3 — Deliver the PDF to the caller.
    ///
    /// Two endpoints are registered:
    ///
    ///   POST /invoices/pdf           — accepts a JSON body, streams back the PDF.
    ///   GET  /invoices/pdf/sample    — generates a hard-coded sample for quick testing.
    ///
    /// Both return "application/pdf" with Content-Disposition: attachment so the
    /// browser triggers a file-save dialog.
    /// </summary>
    public static class InvoiceEndpoints
    {
        // Path relative to content root; leading slash is required.
        private const string TemplateKey = "/Templates/Invoice.cshtml";

        public static void MapInvoiceEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/invoices");

            // ── POST: caller supplies the model as JSON ────────────────────
            group.MapPost("/pdf", async (
                InvoiceModel model,
                IPdfDocumentGenerator pdfService) =>
            {
                var options = new PdfOptions
                {
                    Format = PaperFormat.A4,
                    PrintBackground = true,
                    MarginOptions = new MarginOptions { Top = "20px", Bottom = "20px" }
                };

                var pdfBytes = await pdfService.GenerateAsync(TemplateKey, model, options);
                return DownloadResult(pdfBytes, $"Invoice-{model.InvoiceNumber}.pdf");
            })
            .WithName("GenerateInvoicePdf")
            .WithSummary("Render an invoice model to a downloadable PDF.");

            // ── GET: quick smoke-test with a pre-built sample ──────────────
            group.MapGet("/pdf/sample", async (IPdfDocumentGenerator pdfService) =>
                {
                    var options = new PdfOptions
                    {
                        Format = PaperFormat.A4,
                        PrintBackground = true,
                        MarginOptions = new MarginOptions { Top = "20px", Bottom = "20px" }
                    };

                    var sample = BuildSample();
                    var pdfBytes = await pdfService.GenerateAsync(TemplateKey, sample, options);
                    return DownloadResult(pdfBytes, $"Invoice-{sample.InvoiceNumber}.pdf");
                })
                .WithName("SampleInvoicePdf")
                .WithSummary("Generate a sample invoice PDF for testing.");
        }

        // ── Helpers ───────────────────────────────────────────────────────

        private static IResult DownloadResult(byte[] pdfBytes, string fileName)
        {
            // Returns the raw bytes with correct MIME type and a download filename.
            return Results.File(
                fileContents: pdfBytes,
                contentType: "application/pdf",
                fileDownloadName: fileName);
        }

        private static InvoiceModel BuildSample() => new()
        {
            InvoiceNumber = "INV-2024-0042",
            IssueDate = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc),
            DueDate = new DateTime(2024, 6, 30, 0, 0, 0, DateTimeKind.Utc),
            TaxRate = 0.20m,

            BillFrom = new()
            {
                Name = "ACME Corp.",
                Address = "123 Business Ave, New York, NY 10001",
                Email = "billing@acme.example.com"
            },
            BillTo = new()
            {
                Name = "Globex Corporation",
                Address = "742 Evergreen Terrace, Springfield, IL 62701",
                Email = "accounts@globex.example.com"
            },

            Items = [
                new() { Description = "Web Application Development (40 h)",  Quantity = 40, UnitPrice = 150.00m },
            new() { Description = "UI/UX Design — Figma Prototypes",      Quantity =  8, UnitPrice =  95.00m },
            new() { Description = "Cloud Infrastructure Setup (AWS)",     Quantity =  1, UnitPrice = 750.00m },
            new() { Description = "Monthly SLA Support Retainer",        Quantity =  1, UnitPrice = 500.00m },
        ],

            Notes = "Payment due within 30 days. Bank transfer preferred. " +
                    "Please quote the invoice number in your payment reference."
        };
    }
}
