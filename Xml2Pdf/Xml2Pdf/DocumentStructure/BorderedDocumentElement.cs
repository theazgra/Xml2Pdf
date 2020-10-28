using Xml2Pdf.DocumentStructure.Geometry;

namespace Xml2Pdf.DocumentStructure
{
    public abstract class BorderedDocumentElement : DocumentElement
    {
        public BorderInfo Borders { get; set; }
        public BorderInfo TopBorder { get; set; }
        public BorderInfo BottomBorder { get; set; }
        public BorderInfo LeftBorder { get; set; }
        public BorderInfo RightBorder { get; set; }
        // TODO(Moravec): Border radius.
    }
}