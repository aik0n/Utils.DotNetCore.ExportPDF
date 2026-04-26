using ExportPDF.WebSample.Models;
using ExportPDF.WebSample.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using Utils.DotNetCore.ExportPDF;

namespace ExportPDF.WebSample.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly IPdfDocumentGenerator _pdfGenerator;
        private readonly IDocumentSampleFactory _sampleFactory;
        private readonly IDocumentCache _documentCache;

        public DocumentsController(IPdfDocumentGenerator pdfGenerator, IDocumentSampleFactory sampleFactory, IDocumentCache documentCache)
        {
            _pdfGenerator = pdfGenerator ?? throw new ArgumentNullException(nameof(pdfGenerator));
            _sampleFactory = sampleFactory ?? throw new ArgumentNullException(nameof(sampleFactory));
            _documentCache = documentCache ?? throw new ArgumentNullException(nameof(documentCache));
        }

        [HttpGet]
        public async Task<IActionResult> InvoiceBootstrap()
        {
            var model = _sampleFactory.BuildInvoiceSample();
            var cacheKey = _documentCache.CreateCacheKey();

            _documentCache.Set(cacheKey, model, TimeSpan.FromMinutes(15));
            
            ViewData["CacheKey"] = cacheKey;
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> InvoiceBootstrap([FromForm] string cacheKey)
        {
            var options = new PdfOptions
            {
                Format = PaperFormat.A4,
                PrintBackground = true,
                MarginOptions = new MarginOptions { Top = "20px", Bottom = "20px" }
            };

            var model = _documentCache.TryGetValue(cacheKey, out InvoiceModel? cached) ? cached! : _sampleFactory.BuildInvoiceSample();

            var bytes = await _pdfGenerator.GenerateAsync("/Views/Documents/InvoiceBootstrapExport.cshtml", model, options);

            return File(bytes, "application/pdf", $"Invoice-{model.InvoiceNumber}.pdf");
        }

        [HttpGet]
        public async Task<IActionResult> InvoiceCss(int numberOfRows = 1000)
        {
            var model = _sampleFactory.BuildInvoiceSample(numberOfRows);
            var cacheKey = _documentCache.CreateCacheKey();

            _documentCache.Set(cacheKey, model, TimeSpan.FromMinutes(15));

            ViewData["CacheKey"] = cacheKey;
            ViewData["NumberOfRows"] = numberOfRows;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> InvoiceCss([FromForm] string cacheKey)
        {
            var options = new PdfOptions
            {
                Format = PaperFormat.A4,
                PrintBackground = true,
                MarginOptions = new MarginOptions { Top = "20px", Bottom = "20px" }
            };

            var model = _documentCache.TryGetValue(cacheKey, out InvoiceModel? cached) ? cached! : _sampleFactory.BuildInvoiceSample(1000);

            var bytes = await _pdfGenerator.GenerateAsync("/Views/Documents/InvoiceCssExport.cshtml", model, options);

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
