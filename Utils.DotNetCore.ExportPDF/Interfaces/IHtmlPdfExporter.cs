using PuppeteerSharp;

namespace Utils.DotNetCore.ExportPDF
{
    /// <summary>
    /// Responsibility: accept any HTML string and return the raw bytes of a
    /// valid PDF document.
    /// This stage knows nothing about Razor or models.
    /// </summary>
    public interface IHtmlPdfExporter
    {
        Task<byte[]> ExportAsync(string html, object options);
    }
}