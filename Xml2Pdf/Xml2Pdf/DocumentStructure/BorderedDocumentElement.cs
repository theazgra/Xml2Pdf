using System.Text;
using Xml2Pdf.DocumentStructure.Geometry;

namespace Xml2Pdf.DocumentStructure
{
    public abstract class BorderedDocumentElement : DocumentElement
    {
        public ElementProperty<BorderInfo> Borders { get; set; } = new ElementProperty<BorderInfo>();
        public ElementProperty<BorderInfo> TopBorder { get; set; } = new ElementProperty<BorderInfo>();
        public ElementProperty<BorderInfo> BottomBorder { get; set; } = new ElementProperty<BorderInfo>();
        public ElementProperty<BorderInfo> LeftBorder { get; set; } = new ElementProperty<BorderInfo>();
        public ElementProperty<BorderInfo> RightBorder { get; set; } = new ElementProperty<BorderInfo>();

        // TODO(Moravec): Border radius.

        protected BorderedDocumentElement()
        {
            // OnChildAdded += child =>
            // {
            //     if (child is BorderedDocumentElement)
            //     {
            //         ColorConsole.WriteLine(ConsoleColor.Blue,
            //                                "Child of BorderedDocumentElement is also BorderedDocumentElement and can inherit properties.");
            //     }
            // };
        }

        internal override void DumpToStringBuilder(StringBuilder dumpBuilder, int indent)
        {
            base.DumpToStringBuilder(dumpBuilder, indent);
            PrepareIndent(dumpBuilder, indent).Append("(BorderedDocumentElement)").AppendLine();

            DumpElementProperty(dumpBuilder, indent, "Borders", Borders);
            DumpElementProperty(dumpBuilder, indent, "TopBorder", TopBorder);
            DumpElementProperty(dumpBuilder, indent, "BottomBorder", BottomBorder);
            DumpElementProperty(dumpBuilder, indent, "LeftBorder", LeftBorder);
            DumpElementProperty(dumpBuilder, indent, "RightBorder", RightBorder);
        }
    }
}