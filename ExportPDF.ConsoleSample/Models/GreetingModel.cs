namespace ExportPDF.ConsoleSample.Models
{
    public sealed class GreetingModel
    {
        public string RecipientName { get; init; } = string.Empty;
        public string SenderName { get; init; } = string.Empty;
        public DateTime GeneratedAt { get; init; } = DateTime.UtcNow;
    }
}
