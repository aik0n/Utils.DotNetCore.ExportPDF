using Bogus;
using FluentAssertions;
using Microsoft.Playwright;
using NSubstitute;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = false)]

namespace Utils.DotNetCore.ExportPDF.Tests;

[Trait("Category", "Unit")]
public class PlaywrightPdfExporterTests
{
    private static readonly Faker Faker = new("en") { Random = new Randomizer(42) };

    private static (IPlaywright Playwright, IBrowser Browser, IPage Page) BuildMocks(
        byte[]? pdfBytes = null)
    {
        var page = Substitute.For<IPage>();
        var browser = Substitute.For<IBrowser>();
        var playwright = Substitute.For<IPlaywright>();

        browser.NewPageAsync().Returns(Task.FromResult(page));
        page.PdfAsync(Arg.Any<PagePdfOptions>())
            .Returns(Task.FromResult(pdfBytes ?? new byte[] { 1, 2, 3 }));

        return (playwright, browser, page);
    }

    private static PlaywrightPdfExporter CreateSut(IBrowser browser, IPlaywright playwright) =>
        new PlaywrightPdfExporter(() => Task.FromResult((playwright, browser)));

    [Fact]
    public async Task ExportAsync_WithValidHtmlAndOptions_ReturnsPdfBytes()
    {
        var expectedBytes = new byte[] { 37, 80, 68, 70 };
        var (playwright, browser, _) = BuildMocks(expectedBytes);
        await using var sut = CreateSut(browser, playwright);
        var html = Faker.Lorem.Paragraph();
        var options = new PagePdfOptions();

        var result = await sut.ExportAsync(html, options);

        result.Should().BeEquivalentTo(expectedBytes);
    }

    [Fact]
    public async Task ExportAsync_WithInvalidOptionsType_ThrowsArgumentException()
    {
        var (playwright, browser, _) = BuildMocks();
        await using var sut = CreateSut(browser, playwright);

        var act = () => sut.ExportAsync("<html/>", new object());

        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("options");
    }

    [Fact]
    public async Task ExportAsync_WhenDisposed_ThrowsObjectDisposedException()
    {
        var (playwright, browser, _) = BuildMocks();
        var sut = CreateSut(browser, playwright);
        await sut.DisposeAsync();

        var act = () => sut.ExportAsync("<html/>", new PagePdfOptions());

        await act.Should().ThrowAsync<ObjectDisposedException>();
    }

    [Fact]
    public async Task DisposeAsync_WhenBrowserInitialised_DisposesPlaywrightAndBrowser()
    {
        var (playwright, browser, _) = BuildMocks();
        var sut = CreateSut(browser, playwright);
        await sut.ExportAsync(Faker.Lorem.Paragraph(), new PagePdfOptions());

        await sut.DisposeAsync();

        await browser.Received(1).DisposeAsync();
        playwright.Received(1).Dispose();
    }

    [Fact]
    public async Task DisposeAsync_WhenBrowserNotInitialised_DoesNotThrow()
    {
        var (playwright, browser, _) = BuildMocks();
        var sut = CreateSut(browser, playwright);

        var act = () => sut.DisposeAsync().AsTask();

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task DisposeAsync_CalledTwice_DisposesOnlyOnce()
    {
        var (playwright, browser, _) = BuildMocks();
        var sut = CreateSut(browser, playwright);
        await sut.ExportAsync(Faker.Lorem.Paragraph(), new PagePdfOptions());

        await sut.DisposeAsync();
        await sut.DisposeAsync();

        await browser.Received(1).DisposeAsync();
    }
}
