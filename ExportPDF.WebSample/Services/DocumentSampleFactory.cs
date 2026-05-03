using Bogus;
using ExportPDF.WebSample.Models;
using Microsoft.AspNetCore.Hosting;

namespace ExportPDF.WebSample.Services
{
    public class DocumentSampleFactory : IDocumentSampleFactory
    {
        private readonly IWebHostEnvironment _env;

        public DocumentSampleFactory(IWebHostEnvironment env)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        public InvoiceModel BuildInvoiceSample(int? numberOfRows = null)
        {
            var faker = new Faker("en");
            var issueDate = faker.Date.Recent(30).ToUniversalTime();
            var dueDate = issueDate.AddDays(30);
            var taxRates = new[] { 0.10m, 0.15m, 0.20m };

            var itemCount = numberOfRows ?? faker.Random.Number(2, 5);
            var items = new List<LineItem>();
            for (var i = 0; i < itemCount; i++)
            {
                items.Add(new LineItem
                {
                    Description = faker.Commerce.ProductName(),
                    Quantity = faker.Random.Number(1, 40),
                    UnitPrice = Math.Round(faker.Random.Decimal(25, 999), 2)
                });
            }

            return new InvoiceModel()
            {
                InvoiceNumber = $"INV-{issueDate.Year}-{faker.Random.Number(1000, 9999)}",
                IssueDate = issueDate,
                DueDate = dueDate,
                TaxRate = faker.PickRandom(taxRates),
                BillFrom = new PartyInfo
                {
                    Name = faker.Company.CompanyName(),
                    Address = faker.Address.FullAddress(),
                    Email = faker.Internet.Email()
                },
                BillTo = new PartyInfo
                {
                    Name = faker.Company.CompanyName(),
                    Address = faker.Address.FullAddress(),
                    Email = faker.Internet.Email()
                },
                Items = items,
                Notes = faker.Lorem.Sentence(12)
            };
        }

        public LayoutShowcaseModel BuildLayoutShowcaseSample()
        {
            var faker = new Faker("en");
            var watermarks = new[] { "DRAFT", "CONFIDENTIAL", "SAMPLE", "INTERNAL" };
            
            var paragraphCount = faker.Random.Number(10, 20);

            return new LayoutShowcaseModel
            {
                Title = faker.Commerce.Department() + " Showcase",
                WatermarkText = faker.PickRandom(watermarks),
                ShowWatermark = true,
                LeftColumn = new ColumnContent
                {
                    Heading = faker.Lorem.Sentence(5),
                    Body = faker.Lorem.Paragraph()
                },
                RightColumn = new ColumnContent
                {
                    Heading = faker.Lorem.Sentence(5),
                    Body = faker.Lorem.Paragraph()
                },
                ThreeColumns =
                [
                    new ColumnContent { Heading = faker.Lorem.Sentence(4), Body = faker.Lorem.Paragraph() },
                    new ColumnContent { Heading = faker.Lorem.Sentence(4), Body = faker.Lorem.Paragraph() },
                    new ColumnContent { Heading = faker.Lorem.Sentence(4), Body = faker.Lorem.Paragraph() }
                ],
                Overview = Enumerable.Range(0, paragraphCount)
                    .Select(_ => faker.Lorem.Paragraph())
                    .ToList()
            };
        }

        public TypographyModel BuildTypographySample()
        {
            var faker = new Faker("en");

            return new TypographyModel
            {
                Title = "Typography & Fonts",
                UnicodeShowcase = "Unicode: éàü ☃ © ♥ αβγ € — ¿¡ ✓ ✗ → ← ↑ ↓ ™ ® § ¶",
                PageSizeNote = "This document is rendered at A4 (210 × 297 mm).",
                FontSamples =
                [
                    new FontSample { Name = "Segoe UI (system)",          CssStack = "'Segoe UI', Arial, sans-serif",             PreviewText = faker.Lorem.Sentence(10) },
                    new FontSample { Name = "Georgia (system serif)",      CssStack = "Georgia, 'Times New Roman', serif",          PreviewText = faker.Lorem.Sentence(10) },
                    new FontSample { Name = "Merriweather (Google)",       CssStack = "'Merriweather', Georgia, serif",             PreviewText = faker.Lorem.Sentence(10) },
                    new FontSample { Name = "Courier New (monospace)",     CssStack = "'Courier New', 'Lucida Console', monospace", PreviewText = faker.Lorem.Sentence(10) },
                    new FontSample { Name = "ShowcaseSerif (@font-face)",  CssStack = "'ShowcaseSerif', Georgia, serif",            PreviewText = faker.Lorem.Sentence(10) }
                ]
            };
        }

        public ImageShowcaseModel BuildImageShowcaseSample()
        {
            var imagePath = Path.Combine(_env.WebRootPath, "images", "brand_logo_acme.png");
            var imageBytes = File.ReadAllBytes(imagePath);
            var imageBase64 = Convert.ToBase64String(imageBytes);

            var qrPath = Path.Combine(_env.WebRootPath, "images", "github_qr_url.png");
            var qrBytes = File.ReadAllBytes(qrPath);
            var qrBase64 = Convert.ToBase64String(qrBytes);

            return new ImageShowcaseModel
            {
                ImageBase64 = imageBase64,
                ImageMimeType = "image/png",
                Title = "Image Embedding in PDFs",
                Caption = "Acme Brand Logo — embedded as a base64 data URI",
                Description = "This showcase demonstrates that Chromium (via PuppeteerSharp) correctly renders " +
                              "multiple images supplied as inline base64 data URIs. No external HTTP requests are needed: " +
                              "each image's bytes are read from disk at generation time, encoded to Base64, and " +
                              "injected directly into the HTML as a data: src attribute. Both the brand logo and the " +
                              "GitHub repository QR code are embedded this way.",
                FeatureHighlights =
                [
                    "Images load even when the server has no public URL",
                    "Works identically in preview (IWebHostEnvironment path) and export (Puppeteer headless)",
                    "PNG, JPEG, GIF, and SVG are all supported via their respective MIME types",
                    "Encoding is performed once per request and stored on the model",
                    "No <img src='/...' /> path resolution issues in Puppeteer's virtual context",
                    "Multiple images can be embedded in the same document"
                ],
                QrCodeBase64 = qrBase64,
                QrCodeMimeType = "image/png",
                QrCodeCaption = "GitHub repo — scan to open"
            };
        }
    }
}
