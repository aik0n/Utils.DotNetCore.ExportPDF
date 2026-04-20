using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Razor.Templating.Core;

namespace Utils.DotNetCore.ExportPDF
{
    public sealed class PdfGeneratorRendererBuilder
    {
        private readonly IServiceCollection _services;

        internal PdfGeneratorRendererBuilder(IServiceCollection services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public PdfGeneratorExporterBuilder WithDefaultContentRenderer()
        {
            _services.TryAddScoped<IHtmlContentRenderer, HtmlContentRenderer>();
            return new PdfGeneratorExporterBuilder(_services);
        }

        public PdfGeneratorExporterBuilder WithCustomContentRenderer<TRenderer>() where TRenderer : class, IHtmlContentRenderer
        {
            _services.TryAddScoped<IHtmlContentRenderer, TRenderer>();
            return new PdfGeneratorExporterBuilder(_services);
        }
    }

    public sealed class PdfGeneratorExporterBuilder
    {
        private readonly IServiceCollection _services;

        internal PdfGeneratorExporterBuilder(IServiceCollection services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public IServiceCollection WithPuppeteerExporter()
        {
            _services.TryAddSingleton<IHtmlPdfExporter, PuppeteerSharpPdfExporter>();
            return _services;
        }

        public IServiceCollection WithPlaywrightExporter()
        {
            _services.TryAddSingleton<IHtmlPdfExporter, PlaywrightPdfExporter>();
            return _services;
        }

        public IServiceCollection WithCustomExporter<TExporter>() where TExporter : class, IHtmlPdfExporter
        {
            _services.TryAddSingleton<IHtmlPdfExporter, TExporter>();
            return _services;
        }
    }
}
