using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Razor.Templating.Core;

namespace Utils.DotNetCore.ExportPDF.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static PdfGeneratorRendererBuilder AddPdfGenerator(this IServiceCollection services)
        {
            if (!services.Any(d => d.ServiceType == typeof(IRazorTemplateEngine)))
            {
                services.AddRazorTemplating();
            }

            services.TryAddScoped<IPdfDocumentGenerator, PdfDocumentGenerator>();
            return new PdfGeneratorRendererBuilder(services);
        }
    }
}
