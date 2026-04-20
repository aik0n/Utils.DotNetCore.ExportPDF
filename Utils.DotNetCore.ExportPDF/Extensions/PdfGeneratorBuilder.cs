using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Utils.DotNetCore.ExportPDF
{
    public sealed class PdfGeneratorBuilder
    {
        private readonly IServiceCollection _services;

        internal PdfGeneratorBuilder(IServiceCollection services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public PdfGeneratorBuilder WithDefaultContentRenderer()
        {
            _services.AddRazorTemplating();
            _services.TryAddScoped<IHtmlContentRenderer, HtmlContentRenderer>();
            return this;
        }

        public PdfGeneratorBuilder WithCustomContentRenderer<TRenderer>() where TRenderer : class, IHtmlContentRenderer
        {
            _services.TryAddScoped<IHtmlContentRenderer, TRenderer>();
            return this;
        }

        public PdfGeneratorBuilder WithPuppeteerExporter()
        {
            _services.TryAddSingleton<IHtmlPdfExporter, PuppeteerSharpPdfExporter>();
            return this;
        }

        public PdfGeneratorBuilder WithPlaywrightExporter()
        {
            _services.TryAddSingleton<IHtmlPdfExporter, PlaywrightPdfExporter>();
            return this;
        }

        public PdfGeneratorBuilder WithCustomExporter<TExporter>() where TExporter : class, IHtmlPdfExporter
        {
            _services.TryAddSingleton<IHtmlPdfExporter, TExporter>();
            return this;
        }

        public IServiceCollection Build()
        {
            return _services;
        }
    }
}
