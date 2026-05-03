using ExportPDF.ConsoleSample.Models;
using ExportPDF.ConsoleSample.Renderers;
using Microsoft.Extensions.DependencyInjection;
using RazorLight;
using Utils.DotNetCore.ExportPDF;

namespace ExportPDF.ConsoleSample
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
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
            var renderer = scope.ServiceProvider.GetRequiredService<IHtmlContentRenderer>();

            var model = new GreetingModel
            {
                RecipientName = "World",
                SenderName = "ExportPDF.ConsoleSample",
                GeneratedAt = DateTime.UtcNow
            };

            var html = await renderer.RenderAsync("Templates/Greeting.cshtml", model);

            Console.WriteLine(html);
        }
    }
}
