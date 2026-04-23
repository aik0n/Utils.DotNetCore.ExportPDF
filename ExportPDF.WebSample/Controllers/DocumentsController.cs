using Microsoft.AspNetCore.Mvc;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using Utils.DotNetCore.ExportPDF;
using ExportPDF.WebSample.Models;

namespace ExportPDF.WebSample.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly IPdfDocumentGenerator _pdfGenerator;

        public DocumentsController(IPdfDocumentGenerator pdfGenerator)
            => _pdfGenerator = pdfGenerator;

        // ── Invoice ───────────────────────────────────────────────────────

        public IActionResult InvoiceShow()
            => View(BuildInvoiceSample());

        public async Task<IActionResult> InvoiceExport()
        {
            var options = new PdfOptions
            {
                Format = PaperFormat.A4,
                PrintBackground = true,
                MarginOptions = new MarginOptions { Top = "20px", Bottom = "20px" }
            };
            var model = BuildInvoiceSample();
            var bytes = await _pdfGenerator.GenerateAsync(
                "/Views/Documents/InvoicePdfTemplate.cshtml", model, options);
            return File(bytes, "application/pdf", $"Invoice-{model.InvoiceNumber}.pdf");
        }

        // ── Layout Showcase ───────────────────────────────────────────────

        public IActionResult LayoutShowcaseShow()
            => View(BuildLayoutShowcaseSample());

        public async Task<IActionResult> LayoutShowcaseExport()
        {
            var options = new PdfOptions
            {
                Format = PaperFormat.A4,
                PrintBackground = true,
                MarginOptions = new MarginOptions { Top = "20px", Bottom = "20px" }
            };
            var bytes = await _pdfGenerator.GenerateAsync(
                "/Views/Documents/LayoutShowcasePdfTemplate.cshtml",
                BuildLayoutShowcaseSample(), options);
            return File(bytes, "application/pdf", "LayoutShowcase.pdf");
        }

        // ── Typography ────────────────────────────────────────────────────

        public IActionResult TypographyShow()
            => View(BuildTypographySample());

        public async Task<IActionResult> TypographyExport()
        {
            var options = new PdfOptions
            {
                Format = PaperFormat.A4,
                PrintBackground = true,
                MarginOptions = new MarginOptions { Top = "20px", Bottom = "20px" }
            };
            var bytes = await _pdfGenerator.GenerateAsync(
                "/Views/Documents/TypographyPdfTemplate.cshtml",
                BuildTypographySample(), options);
            return File(bytes, "application/pdf", "Typography.pdf");
        }

        // ── Partial Views ─────────────────────────────────────────────────

        public IActionResult PartialViewsShow()
            => View(BuildPartialViewsSample());

        public async Task<IActionResult> PartialViewsExport()
        {
            var sample = BuildPartialViewsSample();
            var options = new PdfOptions
            {
                Format = PaperFormat.A4,
                Landscape = sample.IsLandscape,
                PrintBackground = true,
                MarginOptions = new MarginOptions { Top = "20px", Bottom = "20px" }
            };
            var bytes = await _pdfGenerator.GenerateAsync(
                "/Views/Documents/PartialViewsPdfTemplate.cshtml", sample, options);
            return File(bytes, "application/pdf", "PartialViews.pdf");
        }

        // ── Demo data builders ────────────────────────────────────────────

        private static InvoiceModel BuildInvoiceSample() => new()
        {
            InvoiceNumber = "INV-2024-0042",
            IssueDate = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc),
            DueDate = new DateTime(2024, 6, 30, 0, 0, 0, DateTimeKind.Utc),
            TaxRate = 0.20m,
            BillFrom = new()
            {
                Name = "ACME Corp.",
                Address = "123 Business Ave, New York, NY 10001",
                Email = "billing@acme.example.com"
            },
            BillTo = new()
            {
                Name = "Globex Corporation",
                Address = "742 Evergreen Terrace, Springfield, IL 62701",
                Email = "accounts@globex.example.com"
            },
            Items =
            [
                new() { Description = "Web Application Development (40 h)",  Quantity = 40, UnitPrice = 150.00m },
                new() { Description = "UI/UX Design — Figma Prototypes",      Quantity =  8, UnitPrice =  95.00m },
                new() { Description = "Cloud Infrastructure Setup (AWS)",     Quantity =  1, UnitPrice = 750.00m },
                new() { Description = "Monthly SLA Support Retainer",         Quantity =  1, UnitPrice = 500.00m },
            ],
            Notes = "Payment due within 30 days. Bank transfer preferred. " +
                    "Please quote the invoice number in your payment reference."
        };

        private static LayoutShowcaseModel BuildLayoutShowcaseSample() => new()
        {
            Title = "Layout Showcase",
            WatermarkText = "DRAFT",
            ShowWatermark = true,
            LeftColumn = new()
            {
                Heading = "Product Overview",
                Body = "Our flagship platform delivers enterprise-grade reliability with a developer-friendly API. " +
                       "Deployable on any cloud provider, it scales seamlessly from startup to global enterprise. " +
                       "Built-in observability, RBAC, and audit logging are included out of the box."
            },
            RightColumn = new()
            {
                Heading = "Key Benefits",
                Body = "• 99.99% SLA with multi-region redundancy\n" +
                       "• Sub-50 ms median latency on all core endpoints\n" +
                       "• SOC 2 Type II certified\n" +
                       "• Dedicated support with 4-hour response SLA\n" +
                       "• Zero-downtime deployments via blue/green switching"
            },
            Tabs =
            [
                new() { Label = "Overview",       IsActive = true,  Content = "The Overview tab provides a high-level summary of the platform architecture, deployment topology, and core capabilities available to all subscribers." },
                new() { Label = "Technical Spec", IsActive = false, Content = "REST and GraphQL APIs, OpenAPI 3.1 documented. SDKs available for Python, Node.js, Go, Java, and .NET. Webhooks with HMAC-SHA256 signatures." },
                new() { Label = "Pricing",        IsActive = false, Content = "Starter: $49/month (up to 10 users). Professional: $199/month (up to 100 users). Enterprise: custom pricing with volume discounts and dedicated infrastructure." },
            ]
        };

        private static TypographyModel BuildTypographySample() => new()
        {
            Title = "Typography & Fonts",
            UnicodeShowcase = "Unicode: éàü ☃ © ♥ αβγ € — ¿¡ ✓ ✗ → ← ↑ ↓ ™ ® § ¶",
            PageSizeNote = "This document is rendered at A4 (210 × 297 mm).",
            LongParagraph =
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore " +
                "et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut " +
                "aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum " +
                "dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui " +
                "officia deserunt mollit anim id est laborum. Pellentesque habitant morbi tristique senectus et netus " +
                "et malesuada fames ac turpis egestas. Vestibulum tortor quam, feugiat vitae, ultricies eget, tempor " +
                "sit amet, ante. Donec eu libero sit amet quam egestas semper. Aenean ultricies mi vitae est. " +
                "Mauris placerat eleifend leo. Quisque sit amet est et sapien ullamcorper pharetra. Vestibulum erat " +
                "wisi, condimentum sed, commodo vitae, ornare sit amet, wisi. Aenean fermentum, elit eget tincidunt " +
                "condimentum, eros ipsum rutrum orci, sagittis tempus lacus enim ac dui. Donec non enim in turpis " +
                "pulvinar facilisis. Ut felis. Praesent dapibus, neque id cursus faucibus, tortor neque egestas augue, " +
                "eu vulputate magna eros eu erat. Aliquam erat volutpat. Nam dui mi, tincidunt quis, accumsan " +
                "porttitor, facilisis luctus, metus. Phasellus ultrices nulla quis nibh. Quisque a lectus.",
            FontSamples =
            [
                new() { Name = "Segoe UI (system)",       CssStack = "'Segoe UI', Arial, sans-serif",              PreviewText = "The quick brown fox jumps over the lazy dog. 0123456789" },
                new() { Name = "Georgia (system serif)",  CssStack = "Georgia, 'Times New Roman', serif",           PreviewText = "The quick brown fox jumps over the lazy dog. 0123456789" },
                new() { Name = "Merriweather (Google)",   CssStack = "'Merriweather', Georgia, serif",              PreviewText = "The quick brown fox jumps over the lazy dog. 0123456789" },
                new() { Name = "Courier New (monospace)", CssStack = "'Courier New', 'Lucida Console', monospace",  PreviewText = "The quick brown fox jumps over the lazy dog. 0123456789" },
                new() { Name = "ShowcaseSerif (@font-face)", CssStack = "'ShowcaseSerif', Georgia, serif",          PreviewText = "The quick brown fox jumps over the lazy dog. 0123456789" },
            ]
        };

        private static PartialViewModel BuildPartialViewsSample() => new()
        {
            Title = "Partial Views & Composition",
            IsLandscape = false,
            Header = new()
            {
                CompanyName = "ACME Corp.",
                DocumentTitle = "Q2 2024 Status Report",
                GeneratedAt = new DateTime(2024, 7, 1, 9, 0, 0, DateTimeKind.Utc)
            },
            Footer = new()
            {
                ConfidentialityNote = "Confidential — Internal use only",
                PageNumber = 1
            },
            Sections =
            [
                new() { SectionTitle = "Executive Summary",         IsHighlighted = false, Body = "Q2 revenue exceeded targets by 12%. Customer retention improved to 94.3%, up from 91.8% in Q1. The new self-serve onboarding flow reduced time-to-first-value from 7 days to under 24 hours." },
                new() { SectionTitle = "Engineering Milestones",    IsHighlighted = true,  Body = "GraphQL API v2 launched with 40% reduction in over-fetching. Deployed multi-region active-active in US-East and EU-West. P99 latency dropped from 320 ms to 48 ms." },
                new() { SectionTitle = "Sales & Partnerships",      IsHighlighted = false, Body = "Closed 3 new enterprise accounts (ARR $1.2M combined). Expanded partnership with AWS — listed on AWS Marketplace. Pipeline for Q3 stands at $4.8M, up 30% QoQ." },
                new() { SectionTitle = "Risk & Mitigations",        IsHighlighted = true,  Body = "Supply-chain dependency on a deprecated logging library identified. Remediation sprint scheduled for July weeks 2–3. No customer impact expected; internal monitoring in place." },
                new() { SectionTitle = "Q3 Priorities",             IsHighlighted = false, Body = "Launch mobile SDK (iOS/Android). Achieve SOC 2 Type II renewal. Onboard 5 additional enterprise accounts. Reduce infrastructure cost per request by 15%." },
            ]
        };
    }
}
