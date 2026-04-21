/*
 * Test Matrix — PdfDocumentGenerator
 * ─────────────────────────────────────────────────────────────────────────────
 *  #   Method          State Under Test                 Expected Behavior
 * ─────────────────────────────────────────────────────────────────────────────
 *  1   ctor            htmlRenderer is null             throws ArgumentNullException("htmlRenderer")
 *  2   ctor            pdfExporter is null              throws ArgumentNullException("pdfExporter")
 *  3   ctor            logger is null                   throws ArgumentNullException("logger")
 *  4   GenerateAsync   valid inputs                     calls RenderAsync once with templateKey + model
 *  5   GenerateAsync   valid inputs                     calls ExportAsync once with rendered html + options
 *  6   GenerateAsync   valid inputs                     returns byte[] from ExportAsync
 *  7   GenerateAsync   RenderAsync throws               exception propagates to caller
 *  8   GenerateAsync   ExportAsync throws               exception propagates to caller
 * ─────────────────────────────────────────────────────────────────────────────
 */

using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Utils.DotNetCore.ExportPDF.Tests
{
    [CollectionDefinition(nameof(PdfDocumentGenerator), DisableParallelization = true)]
    public class PdfDocumentGeneratorCollection
    {
    };

    [Trait("Category", "Unit")]
    [Collection(nameof(PdfDocumentGenerator))]
    public class PdfDocumentGeneratorTests
    {
        private static readonly Faker Faker = new Faker("en")
        {
            Random = new Randomizer(42)
        };

        private sealed class TestModel
        {
            public string Value { get; set; } = string.Empty;
        }

        private static (PdfDocumentGenerator sut, IHtmlContentRenderer renderer, IHtmlPdfExporter exporter) CreateSut()
        {
            var renderer = Substitute.For<IHtmlContentRenderer>();
            var exporter = Substitute.For<IHtmlPdfExporter>();
            var logger = Substitute.For<ILogger<PdfDocumentGenerator>>();

            renderer.RenderAsync(Arg.Any<string>(), Arg.Any<TestModel>())
                    .Returns(Task.FromResult("<p>html</p>"));

            exporter.ExportAsync(Arg.Any<string>(), Arg.Any<object>())
                    .Returns(Task.FromResult(new byte[] { 1, 2, 3 }));

            var sut = new PdfDocumentGenerator(renderer, exporter, logger);
            return (sut, renderer, exporter);
        }

        [Fact]
        public void Constructor_WhenHtmlRendererIsNull_ThrowsArgumentNullException()
        {
            var exporter = Substitute.For<IHtmlPdfExporter>();
            var logger = Substitute.For<ILogger<PdfDocumentGenerator>>();

            Action act = () => new PdfDocumentGenerator(null!, exporter, logger);

            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("htmlRenderer");
        }

        [Fact]
        public void Constructor_WhenPdfExporterIsNull_ThrowsArgumentNullException()
        {
            var renderer = Substitute.For<IHtmlContentRenderer>();
            var logger = Substitute.For<ILogger<PdfDocumentGenerator>>();

            Action act = () => new PdfDocumentGenerator(renderer, null!, logger);

            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("pdfExporter");
        }

        [Fact]
        public void Constructor_WhenLoggerIsNull_ThrowsArgumentNullException()
        {
            var renderer = Substitute.For<IHtmlContentRenderer>();
            var exporter = Substitute.For<IHtmlPdfExporter>();

            Action act = () => new PdfDocumentGenerator(renderer, exporter, null!);

            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("logger");
        }

        [Fact]
        public async Task GenerateAsync_WithValidInputs_CallsRenderAsyncWithTemplateKeyAndModel()
        {
            var (sut, renderer, _) = CreateSut();
            var templateKey = Faker.Lorem.Word();
            var model = new TestModel { Value = Faker.Lorem.Sentence() };
            var options = new object();

            await sut.GenerateAsync(templateKey, model, options);

            await renderer.Received(1).RenderAsync(templateKey, model);
        }

        [Fact]
        public async Task GenerateAsync_WithValidInputs_CallsExportAsyncWithRenderedHtmlAndOptions()
        {
            var (sut, _, exporter) = CreateSut();
            var options = new object();

            await sut.GenerateAsync(Faker.Lorem.Word(), new TestModel(), options);

            await exporter.Received(1).ExportAsync("<p>html</p>", options);
        }

        [Fact]
        public async Task GenerateAsync_WithValidInputs_ReturnsBytesFromExportAsync()
        {
            var (sut, _, _) = CreateSut();

            var result = await sut.GenerateAsync(Faker.Lorem.Word(), new TestModel(), new object());

            result.Should().BeEquivalentTo(new byte[] { 1, 2, 3 });
        }

        [Fact]
        public async Task GenerateAsync_WhenRenderAsyncThrows_ExceptionPropagates()
        {
            var (sut, renderer, _) = CreateSut();
            renderer.RenderAsync(Arg.Any<string>(), Arg.Any<TestModel>())
                    .Returns<string>(_ => throw new InvalidOperationException("render failed"));

            var act = () => sut.GenerateAsync(Faker.Lorem.Word(), new TestModel(), new object());

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("render failed");
        }

        [Fact]
        public async Task GenerateAsync_WhenExportAsyncThrows_ExceptionPropagates()
        {
            var (sut, _, exporter) = CreateSut();
            exporter.ExportAsync(Arg.Any<string>(), Arg.Any<object>())
                    .Returns<byte[]>(_ => throw new InvalidOperationException("export failed"));

            var act = () => sut.GenerateAsync(Faker.Lorem.Word(), new TestModel(), new object());

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("export failed");
        }
    }
}
