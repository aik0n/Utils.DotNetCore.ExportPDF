using Microsoft.AspNetCore.Mvc;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using Utils.DotNetCore.ExportPDF;
using ExportPDF.WebSample.Factories;

namespace ExportPDF.WebSample.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly IPdfDocumentGenerator _pdfGenerator;
        private readonly IDocumentSampleFactory _sampleFactory;

        public DocumentsController(IPdfDocumentGenerator pdfGenerator, IDocumentSampleFactory sampleFactory)
        {
            _pdfGenerator = pdfGenerator;
            _sampleFactory = sampleFactory;
        }

        public IActionResult InvoiceShow()
        {
            return View(_sampleFactory.BuildInvoiceSample());
        }

        public async Task<IActionResult> InvoiceExport()
        {
            var options = new PdfOptions
            {
                Format = PaperFormat.A4,
                PrintBackground = true,
                MarginOptions = new MarginOptions { Top = "20px", Bottom = "20px" }
            };

            var model = _sampleFactory.BuildInvoiceSample();
            var bytes = await _pdfGenerator.GenerateAsync("/Views/Documents/InvoicePdfTemplate.cshtml", model, options);
            
            return File(bytes, "application/pdf", $"Invoice-{model.InvoiceNumber}.pdf");
        }

        public IActionResult LayoutShowcaseShow()
        {
            return View(_sampleFactory.BuildLayoutShowcaseSample());
        }

        public async Task<IActionResult> LayoutShowcaseExport()
        {
            var options = new PdfOptions
            {
                Format = PaperFormat.A4,
                PrintBackground = true,
                MarginOptions = new MarginOptions { Top = "20px", Bottom = "20px" }
            };

            var model = _sampleFactory.BuildLayoutShowcaseSample();
            var bytes = await _pdfGenerator.GenerateAsync("/Views/Documents/LayoutShowcasePdfTemplate.cshtml", model, options);
            
            return File(bytes, "application/pdf", "LayoutShowcase.pdf");
        }

        public IActionResult TypographyShow()
        {
            return View(_sampleFactory.BuildTypographySample());
        }

        public async Task<IActionResult> TypographyExport()
        {
            var options = new PdfOptions
            {
                Format = PaperFormat.A4,
                PrintBackground = true,
                MarginOptions = new MarginOptions { Top = "20px", Bottom = "20px" }
            };

            var model = _sampleFactory.BuildTypographySample();
            var bytes = await _pdfGenerator.GenerateAsync("/Views/Documents/TypographyPdfTemplate.cshtml", model, options);
            
            return File(bytes, "application/pdf", "Typography.pdf");
        }

        public IActionResult PartialViewsShow()
        {
            return View(_sampleFactory.BuildPartialViewsSample());
        }

        public async Task<IActionResult> PartialViewsExport()
        {
            var options = new PdfOptions
            {
                Format = PaperFormat.A4,
                Landscape = true,
                PrintBackground = true,
                MarginOptions = new MarginOptions { Top = "20px", Bottom = "20px" }
            };
            
            var model = _sampleFactory.BuildPartialViewsSample();
            var bytes = await _pdfGenerator.GenerateAsync("/Views/Documents/PartialViewsPdfTemplate.cshtml", model, options);

            return File(bytes, "application/pdf", "PartialViews.pdf");
        }
    }
}
