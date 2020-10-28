using iText.Kernel.Colors;

namespace Xml2Pdf.DocumentStructure.Geometry
{
    public class BorderInfo
    {
        public float Width { get; set; }
        public Color Color { get; set; }
        public BorderType BorderType { get; set; }
    }
}