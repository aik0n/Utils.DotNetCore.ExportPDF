using PuppeteerSharp;

namespace Utils.DotNetCore.ExportPDF;

public sealed class PuppeteerSharpPdfExporter : IHtmlPdfExporter, IAsyncDisposable
{
    private readonly string _chromiumPath;
    private readonly Lazy<Task<IBrowser>> _browserLazy;
    private bool _disposed;

    public PuppeteerSharpPdfExporter(string? chromiumPath = null)
    {
        _chromiumPath = chromiumPath ?? Path.Combine(AppContext.BaseDirectory, ".chromium");
        _browserLazy = new Lazy<Task<IBrowser>>(InitBrowserAsync, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    internal PuppeteerSharpPdfExporter(Func<Task<IBrowser>> browserFactory)
    {
        _chromiumPath = string.Empty;
        _browserLazy = new Lazy<Task<IBrowser>>(browserFactory, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    private async Task<IBrowser> InitBrowserAsync()
    {
        var fetcher = new BrowserFetcher(new BrowserFetcherOptions { Path = _chromiumPath });

        var browser = await fetcher.DownloadAsync();

        return await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
            ExecutablePath = browser.GetExecutablePath()
        });
    }

    public async Task<byte[]> ExportAsync(string html, object options)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(PuppeteerSharpPdfExporter));
        }

        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (options.GetType() != typeof(PdfOptions))
        {
            throw new ArgumentException($"Options must be of type {nameof(PdfOptions)}.", nameof(options));
        }

        var browser = await _browserLazy.Value;

        await using var page = await browser.NewPageAsync();

        try
        {
            await page.SetContentAsync(html);
            return await page.PdfDataAsync((PdfOptions)options);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (_browserLazy.IsValueCreated)
        {
            var browser = await _browserLazy.Value;

            await browser.DisposeAsync();
        }
    }
}