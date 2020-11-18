using System;
using iText.Layout;
using iText.Layout.Properties;

namespace Xml2Pdf.DocumentStructure
{
    public class ElementStyle
    {
        public Style ParagraphStyle { get; set; }

        public ElementStyle()
        {
            ParagraphStyle = new Style();
            ParagraphStyle.SetBold().SetTextAlignment(TextAlignment.RIGHT);
        }
    }
}