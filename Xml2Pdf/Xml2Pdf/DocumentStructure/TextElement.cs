using System.Text;
using iText.Kernel.Colors;
using iText.Layout.Properties;
using Xml2Pdf.DocumentStructure.Geometry;
using Xml2Pdf.Utilities;

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
        public Color ForegroundColor { get; set; } = ColorConstants.BLACK;
        public Color BackgroundColor { get; set; } = ColorConstants.WHITE;

        internal override void DumpToStringBuilder(StringBuilder dumpBuilder, int indentationLevel)
        {
            base.DumpToStringBuilder(dumpBuilder, indentationLevel);
            PrepareIndent(dumpBuilder, indentationLevel).Append("(TextElement)").AppendLine();


            PrepareIndent(dumpBuilder, indentationLevel)
                .Append(" -HorizontalAlignment: ")
                .Append(HorizontalAlignment)
                .AppendLine();

            PrepareIndent(dumpBuilder, indentationLevel)
                .Append(" -VerticalAlignment: ")
                .Append(VerticalAlignment)
                .AppendLine();

            PrepareIndent(dumpBuilder, indentationLevel).Append(" -Bold: ").Append(Bold).AppendLine();

            PrepareIndent(dumpBuilder, indentationLevel).Append(" -Italic: ").Append(Italic).AppendLine();

            PrepareIndent(dumpBuilder, indentationLevel).Append(" -Superscript: ").Append(Superscript).AppendLine();

            PrepareIndent(dumpBuilder, indentationLevel).Append(" -Subscript: ").Append(Subscript).AppendLine();

            if (Underline != null)
                PrepareIndent(dumpBuilder, indentationLevel).Append(" -Underline: ").Append(Underline).AppendLine();

            PrepareIndent(dumpBuilder, indentationLevel).Append(" -FontSize: ").Append(FontSize).AppendLine();

            PrepareIndent(dumpBuilder, indentationLevel)
                .Append(" -ForegroundColor: ")
                .Append(ForegroundColor.ToPrettyString())
                .AppendLine();

            PrepareIndent(dumpBuilder, indentationLevel)
                .Append(" -BackgroundColor: ")
                .Append(BackgroundColor.ToPrettyString())
                .AppendLine();
        }
    }
}