using Microsoft.Playwright;

namespace Utils.DotNetCore.ExportPDF
{
    public sealed class PlaywrightPdfExporter : IHtmlPdfExporter, IAsyncDisposable
    {
        private readonly string _playwrightPath;
        private readonly Lazy<Task<(IPlaywright Playwright, IBrowser Browser)>> _browserLazy;
        private bool _disposed;

        public PlaywrightPdfExporter(string? playwrightPath = null)
        {
            _playwrightPath = playwrightPath ?? Path.Combine(AppContext.BaseDirectory, ".playwright");
            _browserLazy = new Lazy<Task<(IPlaywright, IBrowser)>>(InitAsync, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        internal PlaywrightPdfExporter(Func<Task<(IPlaywright, IBrowser)>> factory)
        {
            _playwrightPath = string.Empty;
            _browserLazy = new Lazy<Task<(IPlaywright, IBrowser)>>(
                factory,
                LazyThreadSafetyMode.ExecutionAndPublication);
        }

        private async Task<(IPlaywright, IBrowser)> InitAsync()
        {
            Environment.SetEnvironmentVariable("PLAYWRIGHT_BROWSERS_PATH", _playwrightPath);
            Microsoft.Playwright.Program.Main(["install", "chromium"]);

            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            });

            return (playwright, browser);
        }

        public async Task<byte[]> ExportAsync(string html, object options)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(PlaywrightPdfExporter));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.GetType() != typeof(PagePdfOptions))
            {
                throw new ArgumentException($"Options must be of type {nameof(PagePdfOptions)}.", nameof(options));
            }

            var (_, browser) = await _browserLazy.Value;

            var page = await browser.NewPageAsync();
            try
            {
                await page.SetContentAsync(html);
                return await page.PdfAsync((PagePdfOptions)options);
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
                var (playwright, browser) = await _browserLazy.Value;

                await browser.DisposeAsync();

                playwright.Dispose();
            }
        }
    }

}
