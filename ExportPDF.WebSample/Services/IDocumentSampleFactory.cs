using ExportPDF.WebSample.Models;

namespace ExportPDF.WebSample.Services
{
    public interface IDocumentSampleFactory
    {
        InvoiceModel BuildInvoiceSample(int? numberOfRows = null);
        LayoutShowcaseModel BuildLayoutShowcaseSample();
        TypographyModel BuildTypographySample();
        PartialViewModel BuildPartialViewsSample();
    }
}
