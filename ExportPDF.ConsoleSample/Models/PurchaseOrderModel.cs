namespace ExportPDF.ConsoleSample.Models
{
    public sealed class PurchaseOrderLineItem
    {
        public string ItemCode { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string Unit { get; init; } = string.Empty;
        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
        public decimal Total
        {
            get { return Quantity * UnitPrice; }
        }
    }

    public sealed class VendorInfo
    {
        public string Name { get; init; } = string.Empty;
        public string Address { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public string Phone { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string ContactPerson { get; init; } = string.Empty;
    }

    public sealed class ShipToInfo
    {
        public string CompanyName { get; init; } = string.Empty;
        public string Address { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public string ContactPerson { get; init; } = string.Empty;
        public string Phone { get; init; } = string.Empty;
    }

    public sealed class PurchaseOrderModel
    {
        public string PurchaseOrderNumber { get; init; } = string.Empty;
        public DateTime OrderDate { get; init; }
        public DateTime RequiredDate { get; init; }
        public string PaymentTerms { get; init; } = string.Empty;
        public string ShippingMethod { get; init; } = string.Empty;
        public VendorInfo Vendor { get; init; } = new VendorInfo();
        public ShipToInfo ShipTo { get; init; } = new ShipToInfo();
        public List<PurchaseOrderLineItem> Items { get; init; } = new List<PurchaseOrderLineItem>();
        public decimal TaxRate { get; init; }
        public string Notes { get; init; } = string.Empty;
        public decimal Subtotal
        {
            get { return Items.Sum(i => i.Total); }
        }
        public decimal TaxAmount
        {
            get { return Math.Round(Subtotal * TaxRate, 2); }
        }
        public decimal GrandTotal
        {
            get { return Subtotal + TaxAmount; }
        }
    }
}
