using iText.Kernel.Colors;
using iText.Layout.Properties;
using Xml2Pdf.DocumentStructure.Geometry;

namespace Xml2Pdf.DocumentStructure
{
    public abstract class TextElement : BorderedDocumentElement
    {
        public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.LEFT;
        public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.TOP;
        public bool Bold { get; set; } = false;
        public bool Italic { get; set; } = false;
        public bool Superscript { get; set; } = false;
        public bool Subscript { get; set; } = false;
        public UnderlineInfo Underline { get; set; }
        public float FontSize { get; set; }
        public Color ForegroundColor { get; set; }
        public Color BackgroundColor { get; set; }
    }
}