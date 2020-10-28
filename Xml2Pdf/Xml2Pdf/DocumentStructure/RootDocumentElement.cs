using iText.Kernel.Geom;
using Xml2Pdf.DocumentStructure.Geometry;

namespace Xml2Pdf.DocumentStructure
{
    public class RootDocumentElement : DocumentElement
    {
        public Margins? CustomMargins { get; set; } = null;
        public PageSize PageSize { get; set; } = PageSize.A4;
        public PageOrientation PageOrientation { get; set; } = PageOrientation.Portrait;
    }
}