namespace ExportPDF.WebSample.Models
{
    public class ImageShowcaseModel
    {
        public string ImageBase64 { get; init; } = string.Empty;
        public string ImageMimeType { get; init; } = "image/png";
        public string Title { get; init; } = "Image Embed Showcase";
        public string Caption { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public List<string> FeatureHighlights { get; init; } = [];
        public string QrCodeBase64 { get; init; } = string.Empty;
        public string QrCodeMimeType { get; init; } = "image/png";
        public string QrCodeCaption { get; init; } = string.Empty;
    }
}
