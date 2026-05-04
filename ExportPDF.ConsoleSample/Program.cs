using ExportPDF.ConsoleSample.Models;
using ExportPDF.ConsoleSample.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PdfSharpCore;
using RazorLight;
using Utils.DotNetCore.ExportPDF;

namespace ExportPDF.ConsoleSample
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddSingleton<IRazorLightEngine>(sp =>
                    new RazorLightEngineBuilder()
                        .UseFileSystemProject(AppContext.BaseDirectory)
                        .UseMemoryCachingProvider()
                        .Build())
                .AddPdfGenerator()
                .WithCustomContentRenderer<RazorLightHtmlContentRenderer>()
                .WithCustomExporter<PdfSharpHtmlPdfExporter>()
                .BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();
            var pdfGenerator = scope.ServiceProvider.GetRequiredService<IPdfDocumentGenerator>();

            var model = PurchaseOrderFactory.Build();

            var options = new PdfSharpExportOptions
            {
                PageSize        = PageSize.A4,
                PageOrientation = PageOrientation.Portrait,
                MarginPt        = 42,
                Author          = "Little Brave Developer",
            };

            var bytes = await pdfGenerator.GenerateAsync("Templates/PurchaseOrderExport.cshtml", model, options);

            var outputPath = Path.Combine(AppContext.BaseDirectory, "purchase-order.pdf");
            await File.WriteAllBytesAsync(outputPath, bytes);

            Console.WriteLine($"PDF saved: {outputPath}");
        }
    }
}
