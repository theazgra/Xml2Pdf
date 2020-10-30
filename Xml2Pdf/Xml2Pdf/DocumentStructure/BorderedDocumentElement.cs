using System.Text;
using iText.StyledXmlParser.Css.Resolve.Shorthand.Impl;
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

        internal override void DumpToStringBuilder(StringBuilder dumpBuilder, int indentationLevel)
        {
            base.DumpToStringBuilder(dumpBuilder, indentationLevel);
            PrepareIndent(dumpBuilder, indentationLevel).Append("(BorderedDocumentElement)").AppendLine();
            
            if (Borders != null)
                PrepareIndent(dumpBuilder, indentationLevel).Append(" -Borders: ").Append(Borders).AppendLine();
            
            if (TopBorder != null)
                PrepareIndent(dumpBuilder, indentationLevel).Append(" -TopBorder: ").Append(TopBorder).AppendLine();
            
            if (BottomBorder != null)
                PrepareIndent(dumpBuilder, indentationLevel).Append(" -BottomBorder: ").Append(BottomBorder).AppendLine();
            
            if (LeftBorder != null)
                PrepareIndent(dumpBuilder, indentationLevel).Append(" -LeftBorder: ").Append(LeftBorder).AppendLine();
            
            if (RightBorder != null)
                PrepareIndent(dumpBuilder, indentationLevel).Append(" -RightBorder: ").Append(RightBorder).AppendLine();
        }
    }
}