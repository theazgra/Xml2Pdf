using System.Text;
using Xml2Pdf.DocumentStructure.Geometry;

namespace Xml2Pdf.DocumentStructure
{
    public abstract class BorderedDocumentElement : DocumentElement
    {
        public ElementProperty<BorderInfo> Borders { get; } = new ElementProperty<BorderInfo>();
        public ElementProperty<BorderInfo> TopBorder { get; } = new ElementProperty<BorderInfo>();
        public ElementProperty<BorderInfo> BottomBorder { get; } = new ElementProperty<BorderInfo>();
        public ElementProperty<BorderInfo> LeftBorder { get; } = new ElementProperty<BorderInfo>();
        public ElementProperty<BorderInfo> RightBorder { get; } = new ElementProperty<BorderInfo>();

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

            DumpElementProperty(dumpBuilder, indent, nameof(Borders), Borders);
            DumpElementProperty(dumpBuilder, indent, nameof(TopBorder), TopBorder);
            DumpElementProperty(dumpBuilder, indent, nameof(BottomBorder), BottomBorder);
            DumpElementProperty(dumpBuilder, indent, nameof(LeftBorder), LeftBorder);
            DumpElementProperty(dumpBuilder, indent, nameof(RightBorder), RightBorder);
        }
    }
}