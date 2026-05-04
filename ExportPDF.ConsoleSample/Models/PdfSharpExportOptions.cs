using PdfSharpCore;

namespace ExportPDF.ConsoleSample.Models
{
    public sealed class PdfSharpExportOptions
    {
        private double _marginPt = 42;
        private string _author = string.Empty;
        private PageOrientation _pageOrientation = PageOrientation.Portrait;
        private PageSize _pageSize = PageSize.A4;

        public double MarginPt
        {
            get { return _marginPt; }
            set { _marginPt = value; }
        }

        public string Author
        {
            get { return _author; }
            set { _author = value ?? string.Empty; }
        }

        public PageOrientation PageOrientation
        {
            get { return _pageOrientation; }
            set { _pageOrientation = value; }
        }

        public PageSize PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }
    }
}
