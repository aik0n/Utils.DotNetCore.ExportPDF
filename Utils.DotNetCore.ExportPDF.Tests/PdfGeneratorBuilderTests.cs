using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Utils.DotNetCore.ExportPDF.Extensions;
using Xunit;

namespace Utils.DotNetCore.ExportPDF.Tests;

[Trait("Category", "Unit")]
public class PdfGeneratorBuilderTests
{
    private static ServiceCollection CreateServices()
    {
        return new ServiceCollection();
    }

    [Fact]
    public void AddPdfGenerator_RegistersIPdfDocumentGeneratorAsScoped()
    {
        var services = CreateServices();

        services.AddPdfGenerator();

        services.Should().Contain(sd =>
            sd.ServiceType == typeof(IPdfDocumentGenerator) &&
            sd.ImplementationType == typeof(PdfDocumentGenerator) &&
            sd.Lifetime == ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddPdfGenerator_ReturnsBuilder()
    {
        var services = CreateServices();

        var result = services.AddPdfGenerator();

        result.Should().BeOfType<PdfGeneratorBuilder>();
    }

    [Fact]
    public void WithDefaultContentRenderer_RegistersHtmlContentRendererAsScoped()
    {
        var services = CreateServices();

        services.AddPdfGenerator().WithDefaultContentRenderer();

        services.Should().Contain(sd =>
            sd.ServiceType == typeof(IHtmlContentRenderer) &&
            sd.ImplementationType == typeof(HtmlContentRenderer) &&
            sd.Lifetime == ServiceLifetime.Scoped);
    }

    [Fact]
    public void WithDefaultContentRenderer_RegistersRazorTemplating()
    {
        var services = CreateServices();

        services.AddPdfGenerator().WithDefaultContentRenderer();

        services.Should().Contain(sd => sd.ServiceType.Name == "IRazorTemplateEngine");
    }

    [Fact]
    public void WithCustomContentRenderer_RegistersCustomTypeAsScoped()
    {
        var services = CreateServices();

        services.AddPdfGenerator().WithCustomContentRenderer<FakeRenderer>();

        services.Should().Contain(sd =>
            sd.ServiceType == typeof(IHtmlContentRenderer) &&
            sd.ImplementationType == typeof(FakeRenderer) &&
            sd.Lifetime == ServiceLifetime.Scoped);
    }

    [Fact]
    public void WithPuppeteerExporter_RegistersAsSingleton()
    {
        var services = CreateServices();

        services.AddPdfGenerator().WithPuppeteerExporter();

        services.Should().Contain(sd =>
            sd.ServiceType == typeof(IHtmlPdfExporter) &&
            sd.ImplementationType == typeof(PuppeteerSharpPdfExporter) &&
            sd.Lifetime == ServiceLifetime.Singleton);
    }

    [Fact]
    public void WithPlaywrightExporter_RegistersAsSingleton()
    {
        var services = CreateServices();

        services.AddPdfGenerator().WithPlaywrightExporter();

        services.Should().Contain(sd =>
            sd.ServiceType == typeof(IHtmlPdfExporter) &&
            sd.ImplementationType == typeof(PlaywrightPdfExporter) &&
            sd.Lifetime == ServiceLifetime.Singleton);
    }

    [Fact]
    public void WithCustomExporter_RegistersCustomTypeAsSingleton()
    {
        var services = CreateServices();

        services.AddPdfGenerator().WithCustomExporter<FakeExporter>();

        services.Should().Contain(sd =>
            sd.ServiceType == typeof(IHtmlPdfExporter) &&
            sd.ImplementationType == typeof(FakeExporter) &&
            sd.Lifetime == ServiceLifetime.Singleton);
    }

    [Fact]
    public void WithDefaultContentRenderer_WhenCalledTwice_DoesNotDuplicateDescriptor()
    {
        var services = CreateServices();
        var builder = services.AddPdfGenerator();

        builder.WithDefaultContentRenderer().WithDefaultContentRenderer();

        services.Count(sd => sd.ServiceType == typeof(IHtmlContentRenderer)).Should().Be(1);
    }

    [Fact]
    public void WithPuppeteerExporter_WhenCalledTwice_DoesNotDuplicateDescriptor()
    {
        var services = CreateServices();
        var builder = services.AddPdfGenerator();

        builder.WithPuppeteerExporter().WithPuppeteerExporter();

        services.Count(sd => sd.ServiceType == typeof(IHtmlPdfExporter)).Should().Be(1);
    }

    [Fact]
    public void Chaining_AllMethods_DoesNotThrow()
    {
        var services = CreateServices();

        var act = () => services
            .AddPdfGenerator()
            .WithDefaultContentRenderer()
            .WithPuppeteerExporter()
            .Build();

        act.Should().NotThrow();
    }

    private sealed class FakeRenderer : IHtmlContentRenderer
    {
        public Task<string> RenderAsync<TModel>(string templateKey, TModel model) where TModel : class
            => Task.FromResult(string.Empty);
    }

    private sealed class FakeExporter : IHtmlPdfExporter
    {
        public Task<byte[]> ExportAsync(string html, object options)
            => Task.FromResult(Array.Empty<byte>());
    }
}
