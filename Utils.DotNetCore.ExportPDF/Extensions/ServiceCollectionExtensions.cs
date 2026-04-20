using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Utils.DotNetCore.ExportPDF.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static PdfGeneratorBuilder AddPdfGenerator(this IServiceCollection services)
        {
            services.TryAddScoped<IPdfDocumentGenerator, PdfDocumentGenerator>();
            return new PdfGeneratorBuilder(services);
        }
    }
}
