using System;
using System.Text;

namespace Xml2Pdf.DocumentStructure
{
    public class TableElement : TextElement
    {
        private static readonly Type[] PossibleChildren = {typeof(TableRowElement)};

        public ElementProperty<int> ColumnCount { get; set; } = new ElementProperty<int>();
        public ElementProperty<float[]> ColumnWidths { get; set; } = new ElementProperty<float[]>();
        public ElementProperty<bool> LargeTable { get; set; } = new ElementProperty<bool>();
        public ElementProperty<float> VerticalBorderSpacing { get; set; } = new ElementProperty<float>();
        public ElementProperty<float> HorizontalBorderSpacing { get; set; } = new ElementProperty<float>();
        public ElementProperty<float> RowHeight { get; set; } = new ElementProperty<float>();

        public override bool IsParentType => true;
        public override Type[] AllowedChildrenTypes => PossibleChildren;

        internal override void DumpToStringBuilder(StringBuilder dumpBuilder, int indent)
        {
            base.DumpToStringBuilder(dumpBuilder, indent);
            DumpElementProperty(dumpBuilder, indent, nameof(ColumnCount), ColumnCount);
            DumpElementProperty(dumpBuilder, indent, nameof(ColumnWidths), ColumnWidths);
            DumpElementProperty(dumpBuilder, indent, nameof(LargeTable), LargeTable);
            DumpElementProperty(dumpBuilder, indent, nameof(VerticalBorderSpacing), VerticalBorderSpacing);
            DumpElementProperty(dumpBuilder, indent, nameof(HorizontalBorderSpacing), HorizontalBorderSpacing);
            DumpElementProperty(dumpBuilder, indent, nameof(RowHeight), RowHeight);

            foreach (var child in Children)
            {
                child.DumpToStringBuilder(dumpBuilder, indent + 2);
            }
        }
    }
}