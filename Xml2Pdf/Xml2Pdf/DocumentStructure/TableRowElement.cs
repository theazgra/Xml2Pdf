using System;

namespace Xml2Pdf.DocumentStructure
{
    public class TableRowElement : TextElement
    {
        private static readonly Type[] PossibleChildren = {typeof(TableCellElement)};
        
        public bool IsHeader { get; set; } = false;
        public float RowHeight { get; set; }

        public override bool IsParentType => true;
        public override Type[] AllowedChildrenTypes => PossibleChildren;
    }
}