namespace ExportPDF.WebSample.Models
{
    public class PartialViewModel
    {
        public string Title { get; init; } = "Partial Views & Composition";
        public DocumentHeader Header { get; init; } = new();
        public DocumentFooter Footer { get; init; } = new();
        public List<ContentSection> Sections { get; init; } = [];
        public bool IsLandscape { get; init; } = false;
    }

    public class DocumentHeader
    {
        public string CompanyName { get; init; } = string.Empty;
        public string DocumentTitle { get; init; } = string.Empty;
        public DateTime GeneratedAt { get; init; } = DateTime.UtcNow;
    }

    public class DocumentFooter
    {
        public string ConfidentialityNote { get; init; } = string.Empty;
        public int PageNumber { get; init; } = 1;
    }

    public class ContentSection
    {
        public string SectionTitle { get; init; } = string.Empty;
        public string Body { get; init; } = string.Empty;
        public bool IsHighlighted { get; init; }
    }
}
