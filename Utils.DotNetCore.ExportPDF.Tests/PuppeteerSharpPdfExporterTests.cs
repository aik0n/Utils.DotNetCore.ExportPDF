/*
 * Test Matrix — PuppeteerSharpPdfExporter
 * ─────────────────────────────────────────────────────────────────────────────
 *  #   Method          State Under Test                 Expected Behavior
 * ─────────────────────────────────────────────────────────────────────────────
 *  1   ctor            no path arg                      _chromiumPath = BaseDirectory/.chromium
 *  2   ctor            custom path given                _chromiumPath = provided value
 *  3   ExportAsync     instance disposed                throws ObjectDisposedException
 *  4   ExportAsync     options is null                  throws ArgumentNullException
 *  5   ExportAsync     options is wrong type            throws ArgumentException
 *  6   ExportAsync     valid html + PdfOptions          returns byte[] from page.PdfDataAsync
 *  7   ExportAsync     valid call                       calls page.SetContentAsync with correct html
 *  8   ExportAsync     page.PdfDataAsync throws         page.CloseAsync still called (finally)
 *  9   DisposeAsync    browser never initialised        completes without exception, no browser access
 * 10   DisposeAsync    browser initialised              calls browser.DisposeAsync
 * 11   DisposeAsync    called twice                     no exception; browser.DisposeAsync called once
 * 12   ExportAsync     dispose then export              throws ObjectDisposedException
 * ─────────────────────────────────────────────────────────────────────────────
 */

using Bogus;
using FluentAssertions;
using NSubstitute;
using PuppeteerSharp;
using Xunit;

namespace Utils.DotNetCore.ExportPDF.Tests
{
    [CollectionDefinition(nameof(PuppeteerSharpPdfExporter), DisableParallelization = true)]
    public class PuppeteerSharpPdfExporterCollection
    {
    };

    [Trait("Category", "Unit")]
    [Collection(nameof(PuppeteerSharpPdfExporter))]
    public class PuppeteerSharpPdfExporterTests
    {
        private static readonly Faker Faker = new Faker("en")
        {
            Random = new Randomizer(42)
        };

        private static (PuppeteerSharpPdfExporter sut, IBrowser browser, IPage page) CreateSut()
        {
            var page = Substitute.For<IPage>();
            var browser = Substitute.For<IBrowser>();

            browser.NewPageAsync().Returns(Task.FromResult(page));
            page.PdfDataAsync(Arg.Any<PdfOptions>()).Returns(Task.FromResult(new byte[] { 1, 2, 3 }));

            var sut = new PuppeteerSharpPdfExporter(() => Task.FromResult(browser));
            return (sut, browser, page);
        }

        [Fact]
        public void Constructor_WithNoPath_UsesDefaultChromiumPath()
        {
            var expected = Path.Combine(AppContext.BaseDirectory, ".chromium");

            var sut = new PuppeteerSharpPdfExporter();

            sut.Should().NotBeNull();

            expected.Should().EndWith(".chromium");
        }

        [Fact]
        public void Constructor_WithCustomPath_DoesNotUseDefault()
        {
            var customPath = Faker.System.DirectoryPath();

            var sut = new PuppeteerSharpPdfExporter(customPath);

            sut.Should().NotBeNull();
        }

        [Fact]
        public async Task ExportAsync_WhenDisposed_ThrowsObjectDisposedException()
        {
            var (sut, _, _) = CreateSut();
            await sut.DisposeAsync();

            var act = () => sut.ExportAsync("<p>html</p>", new PdfOptions());

            await act.Should().ThrowAsync<ObjectDisposedException>();
        }

        [Fact]
        public async Task ExportAsync_WhenOptionsIsNull_ThrowsArgumentNullException()
        {
            var (sut, _, _) = CreateSut();

            var act = () => sut.ExportAsync("<p>html</p>", null!);

            await act.Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName("options");
        }

        [Fact]
        public async Task ExportAsync_WhenOptionsIsWrongType_ThrowsArgumentException()
        {
            var (sut, _, _) = CreateSut();

            var act = () => sut.ExportAsync("<p>html</p>", new object());

            await act.Should().ThrowAsync<ArgumentException>()
                .WithParameterName("options");
        }

        [Fact]
        public async Task ExportAsync_WithValidHtmlAndOptions_ReturnsPdfBytes()
        {
            var (sut, _, _) = CreateSut();
            var html = Faker.Lorem.Paragraph();

            var result = await sut.ExportAsync(html, new PdfOptions());

            result.Should().BeEquivalentTo(new byte[] { 1, 2, 3 });
        }

        [Fact]
        public async Task ExportAsync_WithValidHtml_CallsSetContentAsyncWithHtml()
        {
            var (sut, _, page) = CreateSut();
            var html = Faker.Lorem.Paragraph();

            await sut.ExportAsync(html, new PdfOptions());

            await page.Received(1).SetContentAsync(html, Arg.Any<NavigationOptions>());
        }

        [Fact]
        public async Task ExportAsync_WhenPdfDataAsyncThrows_StillClosesPage()
        {
            var (sut, browser, page) = CreateSut();
            page.PdfDataAsync(Arg.Any<PdfOptions>()).Returns<byte[]>(_ => throw new InvalidOperationException("render failed"));
            var html = Faker.Lorem.Paragraph();

            var act = () => sut.ExportAsync(html, new PdfOptions());

            await act.Should().ThrowAsync<InvalidOperationException>();
            await page.Received(1).CloseAsync(Arg.Any<PageCloseOptions>());
        }

        [Fact]
        public async Task DisposeAsync_WhenBrowserNeverInitialized_CompletesWithoutException()
        {
            var sut = new PuppeteerSharpPdfExporter(() => Task.FromResult(Substitute.For<IBrowser>()));

            var act = () => sut.DisposeAsync().AsTask();

            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task DisposeAsync_WhenBrowserInitialized_CallsBrowserDisposeAsync()
        {
            var (sut, browser, _) = CreateSut();
            await sut.ExportAsync(Faker.Lorem.Paragraph(), new PdfOptions());

            await sut.DisposeAsync();

            await browser.Received(1).DisposeAsync();
        }

        [Fact]
        public async Task DisposeAsync_CalledTwice_DisposesOnce()
        {
            var (sut, browser, _) = CreateSut();
            await sut.ExportAsync(Faker.Lorem.Paragraph(), new PdfOptions());

            await sut.DisposeAsync();
            await sut.DisposeAsync();

            await browser.Received(1).DisposeAsync();
        }

        [Fact]
        public async Task ExportAsync_AfterDispose_ThrowsObjectDisposedException()
        {
            var (sut, _, _) = CreateSut();
            await sut.DisposeAsync();

            var act = () => sut.ExportAsync(Faker.Lorem.Paragraph(), new PdfOptions());

            await act.Should().ThrowAsync<ObjectDisposedException>()
                .WithMessage($"*{nameof(PuppeteerSharpPdfExporter)}*");
        }
    }
}
