using System;

namespace Xml2Pdf.DocumentStructure
{
    public class TableCellElement : TextElement
    {
        private static readonly Type[] PossibleChildren = {typeof(ParagraphElement)};
        
        public int ColumnSpan { get; set; } = 0;
        public int RowSpan { get; set; } = 0;
        
        public override bool IsParentType => true;
        public override Type[] AllowedChildrenTypes => PossibleChildren;
    }
}