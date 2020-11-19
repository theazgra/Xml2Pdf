using System.Collections.Generic;
using iText.Kernel.Font;
using iText.Layout;

namespace Xml2Pdf.DocumentStructure
{
    public class ElementStyle
    {
        public Style ParagraphStyle { get; set; }
        public Style TableCellStyle { get; set; }
        public Style ListItemStyle { get; set; }
        public Style LineStyle { get; set; }
        public Style TableStyle { get; set; }

        public Dictionary<string, PdfFont> CustomFonts { get; set; } = new Dictionary<string, PdfFont>();
    }
}