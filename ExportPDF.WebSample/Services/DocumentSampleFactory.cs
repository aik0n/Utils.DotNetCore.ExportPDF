using Bogus;
using ExportPDF.WebSample.Models;

namespace ExportPDF.WebSample.Services
{
    public class DocumentSampleFactory : IDocumentSampleFactory
    {
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

        public PartialViewModel BuildPartialViewsSample()
        {
            var faker = new Faker("en");

            var sectionCount = faker.Random.Number(3, 6);
            var sections = new List<ContentSection>();
            for (var i = 0; i < sectionCount; i++)
            {
                sections.Add(new ContentSection
                {
                    SectionTitle = faker.Lorem.Sentence(4),
                    Body = faker.Lorem.Sentences(2),
                    IsHighlighted = faker.Random.Bool()
                });
            }

            return new PartialViewModel
            {
                Title = faker.Lorem.Sentence(4),
                IsLandscape = faker.Random.Bool(),
                Header = new DocumentHeader
                {
                    CompanyName = faker.Company.CompanyName(),
                    DocumentTitle = faker.Lorem.Sentence(5),
                    GeneratedAt = faker.Date.Recent(7).ToUniversalTime()
                },
                Footer = new DocumentFooter
                {
                    ConfidentialityNote = faker.Lorem.Sentence(6),
                    PageNumber = faker.Random.Number(1, 10)
                },
                Sections = sections
            };
        }
    }
}
