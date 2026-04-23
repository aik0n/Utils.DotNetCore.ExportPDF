namespace ExportPDF.WebSample.Models
{
    public class TypographyModel
    {
        public string Title { get; init; } = "Typography & Fonts";
        public string UnicodeShowcase { get; init; } = "Unicode: éàü ☃ © ♥ αβγ € — ¿¡";
        public string LongParagraph { get; init; } = string.Empty;
        public string PageSizeNote { get; init; } = "This document is rendered at A4 (210 × 297 mm).";
        public List<FontSample> FontSamples { get; init; } = [];
    }

    public class FontSample
    {
        public string Name { get; init; } = string.Empty;
        public string CssStack { get; init; } = string.Empty;
        public string PreviewText { get; init; } = "The quick brown fox jumps over the lazy dog.";
    }
}
