using ExportPDF.WebSample.Models;

namespace ExportPDF.WebSample.Services
{
    public interface IDocumentSampleFactory
    {
        InvoiceModel BuildInvoiceSample();
        LayoutShowcaseModel BuildLayoutShowcaseSample();
        TypographyModel BuildTypographySample();
        PartialViewModel BuildPartialViewsSample();
    }
}
