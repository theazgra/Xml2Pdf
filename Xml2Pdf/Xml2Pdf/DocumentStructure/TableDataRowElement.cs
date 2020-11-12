using System;
using System.Text;

namespace Xml2Pdf.DocumentStructure
{
    public class TableDataRowElement : TextElement
    {
        private static readonly Type[] PossibleChildren = {typeof(TableCellElement)};

        public ElementProperty<float> RowHeight { get; } = new ElementProperty<float>();
        public ElementProperty<string> DataSource { get; } = new ElementProperty<string>();
        public ElementProperty<string[]> ColumnCellProperties { get; } = new ElementProperty<string[]>();

        protected override bool IsParentType => true;
        protected override Type[] AllowedChildrenTypes => PossibleChildren;

        internal override void DumpToStringBuilder(StringBuilder dumpBuilder, int indent)
        {
            base.DumpToStringBuilder(dumpBuilder, indent);
            DumpElementProperty(dumpBuilder, indent, nameof(RowHeight), RowHeight);
            DumpElementProperty(dumpBuilder, indent, nameof(DataSource), DataSource);
            DumpElementProperty(dumpBuilder, indent, nameof(ColumnCellProperties), ColumnCellProperties);

            foreach (var child in Children)
            {
                child.DumpToStringBuilder(dumpBuilder, indent + DumpIndentationOffset);
            }
        }
    }
}