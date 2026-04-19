using Microsoft.Extensions.Logging;

namespace Utils.DotNetCore.ExportPDF
{
    public sealed class PdfDocumentGenerator : IPdfDocumentGenerator
    {
        private readonly IHtmlContentRenderer _htmlRenderer;
        private readonly IHtmlPdfExporter _pdfExporter;
        private readonly ILogger<PdfDocumentGenerator> _logger;

        public PdfDocumentGenerator(
            IHtmlContentRenderer htmlRenderer,
            IHtmlPdfExporter pdfExporter,
            ILogger<PdfDocumentGenerator> logger)
        {
            _htmlRenderer = htmlRenderer ?? throw new ArgumentNullException(nameof(htmlRenderer));
            _pdfExporter = pdfExporter ?? throw new ArgumentNullException(nameof(pdfExporter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<byte[]> GenerateAsync<TModel>(string templateKey, TModel model, object options) where TModel : class
        {
            var guid = Guid.NewGuid();

            _logger.LogInformation("{Guid} PDF generation started. Template={Template}, Model={Model}", guid.ToString(), templateKey, typeof(TModel).Name);

            var html = await _htmlRenderer.RenderAsync(templateKey, model);
            _logger.LogDebug("{Guid} HTML rendered ({Bytes} bytes)", guid.ToString(), html.Length);

            var pdfBytes = await _pdfExporter.ExportAsync(html, options);
            _logger.LogInformation("{Guid} PDF generated ({Bytes} bytes)", guid.ToString(), pdfBytes.Length);

            return pdfBytes;
        }
    }
}