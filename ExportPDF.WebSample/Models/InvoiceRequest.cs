namespace ExportPDF.WebSample.Models
{
    public sealed class PartyRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public sealed class LineItemRequest
    {
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }
    }

    public sealed class InvoiceRequest
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; } = DateTime.UtcNow;
        public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(30);
        public PartyRequest BillFrom { get; set; } = new();
        public PartyRequest BillTo { get; set; } = new();
        public List<LineItemRequest> Items { get; set; } = [];
        public decimal TaxRatePercent { get; set; } = 20;
        public string? Notes { get; set; }
    }
}
