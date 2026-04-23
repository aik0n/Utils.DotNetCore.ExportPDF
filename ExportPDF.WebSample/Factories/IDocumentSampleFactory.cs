using ExportPDF.WebSample.Models;

namespace ExportPDF.WebSample.Factories
{
    public interface IDocumentSampleFactory
    {
        InvoiceModel BuildInvoiceSample();
        LayoutShowcaseModel BuildLayoutShowcaseSample();
        TypographyModel BuildTypographySample();
        PartialViewModel BuildPartialViewsSample();
    }
}
