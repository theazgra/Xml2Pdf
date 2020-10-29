using System;

namespace Xml2Pdf.DocumentStructure
{
    public class TableElement : TextElement
    {
        private static readonly Type[] PossibleChildren = {typeof(TableRowElement)};
        
        public int ColumnCount { get; set; }
        public float[] ColumnWidths { get; set; }
        public bool LargeTable { get; set; }
        public float VerticalBorderSpacing { get; set; }
        public float HorizontalBorderSpacing { get; set; }
        public float RowHeight { get; set; }
        
        public override bool IsParentType => true;
        public override Type[] AllowedChildrenTypes => PossibleChildren;
    }
}