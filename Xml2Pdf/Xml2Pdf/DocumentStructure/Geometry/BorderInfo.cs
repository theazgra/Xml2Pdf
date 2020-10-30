using iText.Kernel.Colors;
using Xml2Pdf.Utilities;

namespace Xml2Pdf.DocumentStructure.Geometry
{
    public class BorderInfo
    {
        public float Width { get; set; } = 0.0f;
        public Color Color { get; set; } = new DeviceRgb(0, 0, 0); // Default black color.
        public BorderType BorderType { get; set; } = BorderType.NoBorder;

        public override string ToString()
        {
            return $"BorderType: {BorderType}; Width: {Width}; Color: {Color.ToPrettyString()}";
        }
    }
}