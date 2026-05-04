using ExportPDF.ConsoleSample.Models;
using PdfSharpCore.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using Utils.DotNetCore.ExportPDF;

namespace ExportPDF.ConsoleSample.Services
{
    public sealed class PdfSharpHtmlPdfExporter : IHtmlPdfExporter
    {
        public Task<byte[]> ExportAsync(string html, object options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.GetType() != typeof(PdfSharpExportOptions))
            {
                throw new ArgumentException(
                    $"Options must be of type {nameof(PdfSharpExportOptions)}.",
                    nameof(options));
            }

            var exportOptions = (PdfSharpExportOptions)options;

            PdfGenerateConfig config = new PdfGenerateConfig
            {
                PageOrientation = exportOptions.PageOrientation,
                PageSize        = exportOptions.PageSize,
                MarginBottom    = (int)exportOptions.MarginPt,
                MarginLeft      = (int)exportOptions.MarginPt,
                MarginRight     = (int)exportOptions.MarginPt,
                MarginTop       = (int)exportOptions.MarginPt,
            };

            using PdfDocument pdf = PdfGenerator.GeneratePdf(html, config);
            pdf.Info.Author = exportOptions.Author;

            using var ms = new MemoryStream();
            pdf.Save(ms, closeStream: false);
            
            return Task.FromResult(ms.ToArray());
        }
    }
}
