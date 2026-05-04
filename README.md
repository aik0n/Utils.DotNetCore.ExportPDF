[![License: MIT](https://img.shields.io/badge/license-MIT-FFD700.svg?style=flat)](https://opensource.org/licenses/MIT)
![Tag](https://img.shields.io/github/v/tag/aik0n/Utils.DotNetCore.ExportPDF?label=version&style=flat&color=090&sort=semver)
![Repo Size](https://img.shields.io/github/repo-size/aik0n/Utils.DotNetCore.ExportPDF?style=flat&color=036)
![Stars](https://img.shields.io/github/stars/aik0n/Utils.DotNetCore.ExportPDF?style=flat&color=DAA520)

# Utils.DotNetCore.ExportPDF

A .NET 8 library for generating PDF files from typed models via Razor templates and a headless Chromium browser. No paid tools required.  
Features showcase explain in two examples.

## ExportPDF.WebSample

Feature-rich templates and code samples demonstrating real-world PDF generation scenarios:

- **Invoice (Bootstrap)** — professional business invoice with line items, subtotals, tax calculation, and payment notes; demonstrates table layouts and financial formatting
- **Invoice (CSS Only)** — same invoice structure with stress-test capability and page footers; pure custom CSS using Grid for the From/To layout, gradient header bar, and alternating row colors
- **Editable Invoice** — interactive form to fill in parties, line items, tax rate, and notes, then export to PDF in one click
- **Layout + Watermark** — two- and three-column layouts, rounded corners and borders, fixed-position watermark, and selectable page orientation
- **Typography & Fonts** — custom fonts via `@font-face`, Unicode and special characters, long-text overflow handling, and `@media print` CSS rules
- **Image Embed** — embedding images via base64 data URIs; image is read from disk and injected inline with no external requests

## ExportPDF.ConsoleSample

Display how to do not use browser like approach. And operate without "chromium-like" stuff
Known.  Implements custom Renderer and Exporter.  

Internal dependencies trade-off - supports HTML 4 / CSS 2 — flexbox, CSS grid, and CSS variables in a template will be silently ignored.  

---

## Installation

**.NET CLI**
```bash
dotnet add package Utils.DotNetCore.ExportPDF
```

**Package Manager**
```powershell
Install-Package Utils.DotNetCore.ExportPDF
```

**PackageReference**
```xml
<PackageReference Include="Utils.DotNetCore.ExportPDF" Version="1.0.1" />
```

## Architecture

```
┌──────────────────────────────────────────────────────────────────┐
│                        Caller / API                              │
└──────────────────────────────┬───────────────────────────────────┘
                               │  templateKey + TModel
                               ▼
┌──────────────────────────────────────────────────────────────────┐
│                    PdfDocumentGenerator  (orchestrator)          │
│                                                                  │
│   ┌──────────────────────────────────────────────────────────┐   │
│   │  STAGE 1 — IHtmlContentRenderer                          │   │
│   │                                                          │   │
│   │  • Resolves .cshtml template by key                      │   │
│   │  • Binds typed model via Razor.Templating.Core           │   │
│   │  • Returns HTML string                                   │   │
│   │                                                          │   │
│   │  Implementation: HtmlContentRenderer                     │   │
│   └──────────────────────────┬───────────────────────────────┘   │
│                              │  HTML string                      │
│   ┌──────────────────────────▼───────────────────────────────┐   │
│   │  STAGE 2 — IHtmlPdfExporter                              │   │
│   │                                                          │   │
│   │  • Receives raw HTML string + options                    │   │
│   │  • Spins up a headless Chromium browser (once, lazily)   │   │
│   │  • Emits PDF byte[]                                      │   │
│   │                                                          │   │
│   │  Implementations:                                        │   │
│   │    PuppeteerSharpPdfExporter  (PuppeteerSharp, MIT)      │   │
│   │    PlaywrightPdfExporter      (Microsoft.Playwright)     │   │
│   └──────────────────────────┬───────────────────────────────┘   │
│                              │  byte[]                           │
└──────────────────────────────┴───────────────────────────────────┘
```

### Why this decomposition?

| Concern | Stage | Can be swapped? |
|---|---|---|
| Template syntax / styling | Stage 1 | Yes — implement `IHtmlContentRenderer` |
| PDF rendering engine | Stage 2 | Yes — implement `IHtmlPdfExporter` |
| Orchestration | `PdfDocumentGenerator` | Rarely — thin glue only |

Each stage depends only on plain strings or `byte[]`, so every layer is independently testable.

---

## Libraries

| Package | Version | Purpose |
|---|---|---|
| `Razor.Templating.Core` | 2.1.0 | Renders `.cshtml` templates with typed models outside of ASP.NET MVC |
| `PuppeteerSharp` | 20.* | Headless Chromium driver — auto-downloads Chromium on first use |
| `Microsoft.Playwright` | 1.* | Alternative headless Chromium driver — auto-installs Chromium on first use |
| `Microsoft.Extensions.DependencyInjection.Abstractions` | 8.0.0 | DI extension point |

---

## Project Structure

```
Utils.DotNetCore.ExportPDF/
├── Utils.DotNetCore.ExportPDF.csproj
├── GlobalSuppressions.cs
│
├── Interfaces/
│   ├── IHtmlContentRenderer.cs    ← Stage 1 contract
│   ├── IHtmlPdfExporter.cs        ← Stage 2 contract
│   └── IPdfDocumentGenerator.cs   ← Orchestrator contract
│
├── Implementation/
│   ├── HtmlContentRenderer.cs          ← Stage 1: Razor.Templating.Core wrapper
│   ├── PuppeteerSharpPdfExporter.cs    ← Stage 2: PuppeteerSharp implementation
│   ├── PlaywrightPdfExporter.cs        ← Stage 2: Playwright implementation
│   └── PdfDocumentGenerator.cs        ← Orchestrator
│
└── Extensions/
    ├── ServiceCollectionExtensions.cs  ← AddPdfGenerator() entry point
    └── PdfGeneratorBuilder.cs          ← Fluent builder (renderer + exporter steps)
```

## DI Registration

```csharp
builder.Services
    .AddPdfGenerator()
    .WithDefaultContentRenderer()   // registers HtmlContentRenderer (Scoped)
    .WithPuppeteerExporter();       // registers PuppeteerSharpPdfExporter (Singleton)
                                    // or: .WithPlaywrightExporter()
                                    // or: .WithCustomExporter<TExporter>()
```

`AddRazorTemplating()` is called automatically by `WithDefaultContentRenderer()` if not already registered.

**Lifetimes**

| Service | Lifetime | Reason |
|---|---|---|
| `IPdfDocumentGenerator` | Scoped | Orchestrator, tied to request |
| `IHtmlContentRenderer` | Scoped | Razor rendering is per-request |
| `IHtmlPdfExporter` | Singleton | Browser initialisation is expensive; one instance reused |

Custom renderer or exporter variants:
```csharp
builder.Services
    .AddPdfGenerator()
    .WithCustomContentRenderer<MyRenderer>()
    .WithCustomExporter<MyExporter>();
```

---

## Chromium Download

Both exporters download Chromium automatically on first use:

- `PuppeteerSharpPdfExporter` — downloads to `.chromium/` relative to the application base directory.
- `PlaywrightPdfExporter` — installs to `.playwright/` relative to the application base directory (sets `PLAYWRIGHT_BROWSERS_PATH` before installation).

The browser is initialised once via `Lazy<Task<IBrowser>>` with `ExecutionAndPublication` thread-safety.

---

## Swapping the PDF Engine

Implement `IHtmlPdfExporter` and register it:

```csharp
builder.Services
    .AddPdfGenerator()
    .WithDefaultContentRenderer()
    .WithCustomExporter<MyCustomPdfExporter>();
```

Stage 1 (HTML rendering) requires zero changes in this case.

---

## License
This project is licensed under the [MIT License](./LICENSE)