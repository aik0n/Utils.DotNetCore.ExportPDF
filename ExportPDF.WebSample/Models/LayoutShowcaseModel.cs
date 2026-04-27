namespace ExportPDF.WebSample.Models
{
    public class LayoutShowcaseModel
    {
        public string Title { get; init; } = "Layout Showcase";
        public string WatermarkText { get; init; } = "DRAFT";
        public bool ShowWatermark { get; init; } = true;
        public string? BackgroundImageUrl { get; init; }
        public ColumnContent LeftColumn { get; init; } = new();
        public ColumnContent RightColumn { get; init; } = new();
        public List<ColumnContent> ThreeColumns { get; init; } = [];
    }

    public class ColumnContent
    {
        public string Heading { get; init; } = string.Empty;
        public string Body { get; init; } = string.Empty;
    }

    public class TabItem
    {
        public string Label { get; init; } = string.Empty;
        public string Content { get; init; } = string.Empty;
        public bool IsActive { get; init; }
    }
}
