using iText.Kernel.Colors;

namespace Xml2Pdf.DocumentStructure.Geometry
{
    public class UnderlineInfo
    {
        public Color Color { get; set; } = ColorConstants.BLACK;
        public float Opacity { get; set; } = 1.0f;
        public float Thickness { get; set; } = 1.0f;
        public float YPos { get; set; }
    }
}