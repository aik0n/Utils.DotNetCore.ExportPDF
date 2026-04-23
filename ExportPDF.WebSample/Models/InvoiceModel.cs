namespace ExportPDF.WebSample.Models
{
    public sealed class InvoiceModel
    {
        public string InvoiceNumber { get; init; } = string.Empty;
        public DateTime IssueDate { get; init; } = DateTime.UtcNow;
        public DateTime DueDate { get; init; }

        public PartyInfo BillFrom { get; init; } = new();
        public PartyInfo BillTo { get; init; } = new();

        public List<LineItem> Items { get; init; } = [];

        public decimal Subtotal => Items.Sum(i => i.Total);
        public decimal TaxAmount => Math.Round(Subtotal * TaxRate, 2);
        public decimal GrandTotal => Subtotal + TaxAmount;

        public decimal TaxRate { get; init; } = 0.20m;

        public string? Notes { get; init; }
    }

    public sealed class PartyInfo
    {
        public string Name { get; init; } = string.Empty;
        public string Address { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
    }

    public sealed class LineItem
    {
        public string Description { get; init; } = string.Empty;
        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
        public decimal Total => Quantity * UnitPrice;
    }
}
