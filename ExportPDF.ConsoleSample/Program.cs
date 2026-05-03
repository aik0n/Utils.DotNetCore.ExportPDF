using ExportPDF.ConsoleSample.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
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
                .WithPlaywrightExporter()
                .BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();
            var pdfGenerator = scope.ServiceProvider.GetRequiredService<IPdfDocumentGenerator>();

            var model = PurchaseOrderFactory.Build();

            var options = new PagePdfOptions
            {
                Format = "A4",
                PrintBackground = true,
                Margin = new Margin
                {
                    Top = "15mm",
                    Bottom = "15mm",
                    Left = "15mm",
                    Right = "15mm"
                }
            };

            var bytes = await pdfGenerator.GenerateAsync("Templates/PurchaseOrderExport.cshtml", model, options);

            var outputPath = Path.Combine(AppContext.BaseDirectory, "purchase-order.pdf");
            await File.WriteAllBytesAsync(outputPath, bytes);

            Console.WriteLine($"PDF saved: {outputPath}");
        }
    }
}
