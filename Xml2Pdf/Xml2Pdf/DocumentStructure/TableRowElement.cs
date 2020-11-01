using System;
using System.Text;

namespace Xml2Pdf.DocumentStructure
{
    public class TableRowElement : TextElement
    {
        private static readonly Type[] PossibleChildren = {typeof(TableCellElement)};

        public ElementProperty<bool> IsHeader { get; } = new ElementProperty<bool>();
        public ElementProperty<float> RowHeight { get; } = new ElementProperty<float>();

        public override bool IsParentType => true;
        public override Type[] AllowedChildrenTypes => PossibleChildren;

        internal override void DumpToStringBuilder(StringBuilder dumpBuilder, int indent)
        {
            base.DumpToStringBuilder(dumpBuilder, indent);
            DumpElementProperty(dumpBuilder, indent, nameof(IsHeader), IsHeader);
            DumpElementProperty(dumpBuilder, indent, nameof(RowHeight), RowHeight);

            foreach (var child in Children)
            {
                child.DumpToStringBuilder(dumpBuilder, indent + 2);
            }
        }
    }
}