using System;
using System.Text;

namespace Xml2Pdf.DocumentStructure
{
    public class TableDataRowElement : TableRowElement
    {
        private static readonly Type[] PossibleChildren = {typeof(TableCellElement)};

        public ElementProperty<string> DataSource { get; } = new();
        public ElementProperty<string[]> ColumnCellProperties { get; } = new();

        protected override bool IsParentType => true;
        protected override Type[] AllowedChildrenTypes => PossibleChildren;

        internal override void DumpToStringBuilder(StringBuilder dumpBuilder, int indent)
        {
            base.DumpToStringBuilder(dumpBuilder, indent);
            DumpElementProperty(dumpBuilder, indent, nameof(DataSource), DataSource);
            DumpElementProperty(dumpBuilder, indent, nameof(ColumnCellProperties), ColumnCellProperties);

            foreach (var child in Children)
            {
                child.DumpToStringBuilder(dumpBuilder, indent + DumpIndentationOffset);
            }
        }
    }
}