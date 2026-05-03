using Bogus;
using ExportPDF.ConsoleSample.Models;

namespace ExportPDF.ConsoleSample.Services
{
    public static class PurchaseOrderFactory
    {
        public static PurchaseOrderModel Build()
        {
            var faker = new Faker("en");

            var orderDate = faker.Date.Recent(90);
            var requiredDate = orderDate.AddDays(faker.Random.Int(14, 60));

            var vendor = new VendorInfo
            {
                Name = faker.Company.CompanyName(),
                Address = faker.Address.StreetAddress(),
                City = $"{faker.Address.City()}, {faker.Address.StateAbbr()} {faker.Address.ZipCode()}",
                Phone = faker.Phone.PhoneNumber(),
                Email = faker.Internet.Email(),
                ContactPerson = faker.Name.FullName()
            };

            var shipTo = new ShipToInfo
            {
                CompanyName = faker.Company.CompanyName(),
                Address = faker.Address.StreetAddress(),
                City = $"{faker.Address.City()}, {faker.Address.StateAbbr()} {faker.Address.ZipCode()}",
                ContactPerson = faker.Name.FullName(),
                Phone = faker.Phone.PhoneNumber()
            };

            var units = new[] { "EA", "PCS", "BOX", "PKG", "SET", "KG", "LB" };
            var itemCount = faker.Random.Int(3, 8);
            var items = new List<PurchaseOrderLineItem>(itemCount);

            for (int i = 0; i < itemCount; i++)
            {
                items.Add(new PurchaseOrderLineItem
                {
                    ItemCode = $"ITM-{faker.Random.AlphaNumeric(6).ToUpper()}",
                    Description = faker.Commerce.ProductName(),
                    Unit = faker.PickRandom(units),
                    Quantity = faker.Random.Int(1, 50),
                    UnitPrice = faker.Random.Decimal(10m, 500m)
                });
            }

            return new PurchaseOrderModel
            {
                PurchaseOrderNumber = $"PO-{DateTime.UtcNow.Year}-{faker.Random.Int(1000, 9999)}",
                OrderDate = orderDate,
                RequiredDate = requiredDate,
                PaymentTerms = faker.PickRandom(new[] { "Net 30", "Net 60", "Net 15", "Due on Receipt", "2/10 Net 30" }),
                ShippingMethod = faker.PickRandom(new[] { "Ground", "Express", "Overnight", "2-Day Air", "Freight" }),
                Vendor = vendor,
                ShipTo = shipTo,
                Items = items,
                TaxRate = faker.PickRandom(new[] { 0.06m, 0.07m, 0.08m, 0.10m }),
                Notes = faker.Lorem.Sentence(12)
            };
        }
    }
}
