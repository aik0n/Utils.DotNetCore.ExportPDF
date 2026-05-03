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
                DisplayHeaderFooter = true,
                HeaderTemplate = "<span></span>",
                FooterTemplate = "<div style='width:100%; font-family:Segoe UI,Arial,sans-serif; font-size:9px !important; color:#6b7280; text-align:right; padding-right:30px;'>" +
                                 "Page <span class='pageNumber'></span> of <span class='totalPages'></span>" +
                                 "</div>",
                MarginOptions = new MarginOptions { Top = "20px", Bottom = "50px" }
            };

            var model = _documentCache.TryGetValue(cacheKey, out InvoiceModel? cached) ? cached! : _sampleFactory.BuildInvoiceSample(1000);

            var bytes = await _pdfGenerator.GenerateAsync("/Views/Documents/InvoiceCssExport.cshtml", model, options);

            return File(bytes, "application/pdf", $"Invoice-{model.InvoiceNumber}.pdf");
        }

        [HttpGet]
        public async Task<IActionResult> LayoutShowcase()
        {
            var model = _sampleFactory.BuildLayoutShowcaseSample();
            var cacheKey = _documentCache.CreateCacheKey();

            _documentCache.Set(cacheKey, model, TimeSpan.FromMinutes(15));

            ViewData["CacheKey"] = cacheKey;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> LayoutShowcase([FromForm] string cacheKey, [FromForm] bool isLandscape = false)
        {
            var options = new PdfOptions
            {
                Format = PaperFormat.A4,
                Landscape = isLandscape,
                PrintBackground = true,
                MarginOptions = new MarginOptions { Top = "20px", Bottom = "20px" }
            };

            var model = _documentCache.TryGetValue(cacheKey, out LayoutShowcaseModel? cachedLayout) ? cachedLayout! : _sampleFactory.BuildLayoutShowcaseSample();

            var bytes = await _pdfGenerator.GenerateAsync("/Views/Documents/LayoutShowcaseExport.cshtml", model, options);

            return File(bytes, "application/pdf", "LayoutShowcase.pdf");
        }

        [HttpGet]
        public async Task<IActionResult> EditableInvoice()
        {
            var sample = _sampleFactory.BuildInvoiceSample();

            var model = new InvoiceRequest
            {
                InvoiceNumber = sample.InvoiceNumber,
                IssueDate = sample.IssueDate,
                DueDate = sample.DueDate,
                TaxRatePercent = sample.TaxRate * 100,
                Notes = sample.Notes,
                BillFrom = new PartyRequest
                {
                    Name = sample.BillFrom.Name,
                    Address = sample.BillFrom.Address,
                    Email = sample.BillFrom.Email
                },
                BillTo = new PartyRequest
                {
                    Name = sample.BillTo.Name,
                    Address = sample.BillTo.Address,
                    Email = sample.BillTo.Email
                },
                Items = sample.Items.Select(i => new LineItemRequest
                {
                    Description = i.Description,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditableInvoice([FromForm] InvoiceRequest model)
        {
            model.Items ??= [];

            var data = new InvoiceModel
            {
                InvoiceNumber = model.InvoiceNumber,
                IssueDate = model.IssueDate,
                DueDate = model.DueDate,
                TaxRate = model.TaxRatePercent / 100m,
                Notes = model.Notes,
                BillFrom = new PartyInfo
                {
                    Name = model.BillFrom.Name,
                    Address = model.BillFrom.Address,
                    Email = model.BillFrom.Email
                },
                BillTo = new PartyInfo
                {
                    Name = model.BillTo.Name,
                    Address = model.BillTo.Address,
                    Email = model.BillTo.Email
                },
                Items = model.Items.Select(i => new LineItem
                {
                    Description = i.Description,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            var options = new PdfOptions
            {
                Format = PaperFormat.A4,
                PrintBackground = true,
                MarginOptions = new MarginOptions { Top = "20px", Bottom = "20px" }
            };

            var bytes = await _pdfGenerator.GenerateAsync("/Views/Documents/EditableInvoiceExport.cshtml", data, options);

            return File(bytes, "application/pdf", $"Invoice-{data.InvoiceNumber}.pdf");
        }

        [HttpGet]
        public async Task<IActionResult> TypographyShow()
        {
            var model = _sampleFactory.BuildTypographySample();
            var cacheKey = _documentCache.CreateCacheKey();

            _documentCache.Set(cacheKey, model, TimeSpan.FromMinutes(15));

            ViewData["CacheKey"] = cacheKey;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> TypographyShow([FromForm] string cacheKey)
        {
            var options = new PdfOptions
            {
                Format = PaperFormat.A4,
                PrintBackground = true,
                MarginOptions = new MarginOptions { Top = "20px", Bottom = "20px" }
            };

            var model = _documentCache.TryGetValue(cacheKey, out TypographyModel? cachedTypography) ? cachedTypography! : _sampleFactory.BuildTypographySample();

            var bytes = await _pdfGenerator.GenerateAsync("/Views/Documents/TypographyExport.cshtml", model, options);

            return File(bytes, "application/pdf", "Typography.pdf");
        }

        [HttpGet]
        public async Task<IActionResult> ImageShowcase()
        {
            var model = _sampleFactory.BuildImageShowcaseSample();
            var cacheKey = _documentCache.CreateCacheKey();

            _documentCache.Set(cacheKey, model, TimeSpan.FromMinutes(15));

            ViewData["CacheKey"] = cacheKey;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ImageShowcase([FromForm] string cacheKey)
        {
            var options = new PdfOptions
            {
                Format = PaperFormat.A4,
                PrintBackground = true,
                MarginOptions = new MarginOptions { Top = "20px", Bottom = "20px" }
            };

            var model = _documentCache.TryGetValue(cacheKey, out ImageShowcaseModel? cached) ? cached! : _sampleFactory.BuildImageShowcaseSample();

            var bytes = await _pdfGenerator.GenerateAsync("/Views/Documents/ImageShowcaseExport.cshtml", model, options);

            return File(bytes, "application/pdf", "ImageShowcase.pdf");
        }
    }
}
