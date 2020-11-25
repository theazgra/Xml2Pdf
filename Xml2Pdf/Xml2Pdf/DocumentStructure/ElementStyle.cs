using System.Collections.Generic;
using iText.Kernel.Font;
using Xml2Pdf.Renderer;

namespace Xml2Pdf.DocumentStructure
{
    public class ElementStyle
    {
        public StyleWrapper ParagraphStyle { get; set; }
        public StyleWrapper TableCellStyle { get; set; }
        public StyleWrapper ListItemStyle { get; set; }
        public StyleWrapper LineStyle { get; set; }
        public StyleWrapper TableStyle { get; set; }

        public Dictionary<string, PdfFont> CustomFonts { get; } = new();
    }
}