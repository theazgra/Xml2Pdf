using System;
using System.Text;

namespace Xml2Pdf.DocumentStructure
{
    public class TableCellElement : TextElement
    {
        private static readonly Type[] PossibleChildren = {typeof(ParagraphElement)};

        public ElementProperty<int> ColumnSpan { get; } = new ElementProperty<int>();
        public ElementProperty<int> RowSpan { get; } = new ElementProperty<int>();

        protected override bool IsParentType => true;
        protected override Type[] AllowedChildrenTypes => PossibleChildren;

        internal override void DumpToStringBuilder(StringBuilder dumpBuilder, int indent)
        {
            base.DumpToStringBuilder(dumpBuilder, indent);
            DumpElementProperty(dumpBuilder, indent, nameof(ColumnSpan), ColumnSpan);
            DumpElementProperty(dumpBuilder, indent, nameof(RowSpan), RowSpan);

            foreach (var child in Children)
            {
                child.DumpToStringBuilder(dumpBuilder, indent + DumpIndentationOffset);
            }
        }
    }
}