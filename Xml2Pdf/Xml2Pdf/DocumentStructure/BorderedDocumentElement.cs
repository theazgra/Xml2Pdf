using System.Collections.Generic;
using System.Text;
using iText.Kernel.Font;
using iText.Layout;
using Xml2Pdf.DocumentStructure.Geometry;
using Xml2Pdf.Renderer;
using Xml2Pdf.Renderer.Mappers;

namespace Xml2Pdf.DocumentStructure
{
    public abstract class BorderedDocumentElement : DocumentElement
    {
        public ElementProperty<BorderInfo> Borders { get; } = new ElementProperty<BorderInfo>();
        public ElementProperty<BorderInfo> TopBorder { get; } = new ElementProperty<BorderInfo>();
        public ElementProperty<BorderInfo> BottomBorder { get; } = new ElementProperty<BorderInfo>();
        public ElementProperty<BorderInfo> LeftBorder { get; } = new ElementProperty<BorderInfo>();
        public ElementProperty<BorderInfo> RightBorder { get; } = new ElementProperty<BorderInfo>();

        protected BorderedDocumentElement() { }

        internal override void DumpToStringBuilder(StringBuilder dumpBuilder, int indent)
        {
            base.DumpToStringBuilder(dumpBuilder, indent);
            PrepareIndent(dumpBuilder, indent).Append("(BorderedDocumentElement)").AppendLine();

            DumpElementProperty(dumpBuilder, indent, nameof(Borders), Borders);
            DumpElementProperty(dumpBuilder, indent, nameof(TopBorder), TopBorder);
            DumpElementProperty(dumpBuilder, indent, nameof(BottomBorder), BottomBorder);
            DumpElementProperty(dumpBuilder, indent, nameof(LeftBorder), LeftBorder);
            DumpElementProperty(dumpBuilder, indent, nameof(RightBorder), RightBorder);
        }

        public override StyleWrapper GetElementStyle(Dictionary<string, PdfFont> customFonts)
        {
            var style = base.GetElementStyle(customFonts);

            // Borders.
            if (Borders.IsInitialized)
            {
                style.SetBorder(Borders.Value.ToITextBorder());
            }
            else
            {
                if (TopBorder.IsInitialized)
                    style.SetBorderTop(TopBorder.Value.ToITextBorder());
                if (BottomBorder.IsInitialized)
                    style.SetBorderBottom(BottomBorder.Value.ToITextBorder());
                if (LeftBorder.IsInitialized)
                    style.SetBorderLeft(LeftBorder.Value.ToITextBorder());
                if (RightBorder.IsInitialized)
                    style.SetBorderRight(RightBorder.Value.ToITextBorder());
            }

            return style;
        }
    }
}