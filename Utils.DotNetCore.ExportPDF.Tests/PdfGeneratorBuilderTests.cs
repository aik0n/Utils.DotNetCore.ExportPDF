using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Razor.Templating.Core;
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
    public void AddPdfGenerator_ReturnsPdfGeneratorRendererBuilder()
    {
        var services = CreateServices();

        var result = services.AddPdfGenerator();

        result.Should().BeOfType<PdfGeneratorRendererBuilder>();
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

        services.Should().Contain(sd => sd.ServiceType.Name == nameof(IRazorTemplateEngine));
    }

    [Fact]
    public void WithDefaultContentRenderer_ReturnsPdfGeneratorExporterBuilder()
    {
        var services = CreateServices();

        var result = services.AddPdfGenerator().WithDefaultContentRenderer();

        result.Should().BeOfType<PdfGeneratorExporterBuilder>();
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
    public void WithCustomContentRenderer_ReturnsPdfGeneratorExporterBuilder()
    {
        var services = CreateServices();

        var result = services.AddPdfGenerator().WithCustomContentRenderer<FakeRenderer>();

        result.Should().BeOfType<PdfGeneratorExporterBuilder>();
    }

    [Fact]
    public void WithPuppeteerExporter_RegistersAsSingleton()
    {
        var services = CreateServices();

        services.AddPdfGenerator().WithDefaultContentRenderer().WithPuppeteerExporter();

        services.Should().Contain(sd =>
            sd.ServiceType == typeof(IHtmlPdfExporter) &&
            sd.ImplementationType == typeof(PuppeteerSharpPdfExporter) &&
            sd.Lifetime == ServiceLifetime.Singleton);
    }

    [Fact]
    public void WithPuppeteerExporter_ReturnsServiceCollection()
    {
        var services = CreateServices();

        var result = services.AddPdfGenerator().WithDefaultContentRenderer().WithPuppeteerExporter();

        result.Should().BeSameAs(services);
    }

    [Fact]
    public void WithPlaywrightExporter_RegistersAsSingleton()
    {
        var services = CreateServices();

        services.AddPdfGenerator().WithDefaultContentRenderer().WithPlaywrightExporter();

        services.Should().Contain(sd =>
            sd.ServiceType == typeof(IHtmlPdfExporter) &&
            sd.ImplementationType == typeof(PlaywrightPdfExporter) &&
            sd.Lifetime == ServiceLifetime.Singleton);
    }

    [Fact]
    public void WithCustomExporter_RegistersCustomTypeAsSingleton()
    {
        var services = CreateServices();

        services.AddPdfGenerator().WithDefaultContentRenderer().WithCustomExporter<FakeExporter>();

        services.Should().Contain(sd =>
            sd.ServiceType == typeof(IHtmlPdfExporter) &&
            sd.ImplementationType == typeof(FakeExporter) &&
            sd.Lifetime == ServiceLifetime.Singleton);
    }

    [Fact]
    public void Chaining_AllMethods_ReturnsServiceCollection()
    {
        var services = CreateServices();

        var result = services
            .AddPdfGenerator()
            .WithDefaultContentRenderer()
            .WithPuppeteerExporter();

        result.Should().BeSameAs(services);
    }

    private sealed class FakeRenderer : IHtmlContentRenderer
    {
        public Task<string> RenderAsync<TModel>(string templateKey, TModel model) where TModel : class
        {
            return Task.FromResult(string.Empty);
        }
    }

    private sealed class FakeExporter : IHtmlPdfExporter
    {
        public Task<byte[]> ExportAsync(string html, object options)
        {
            return Task.FromResult(Array.Empty<byte>());
        }
    }
}
