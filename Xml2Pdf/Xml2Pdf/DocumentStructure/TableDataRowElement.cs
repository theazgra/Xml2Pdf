using System;

namespace Xml2Pdf.DocumentStructure
{
    public class TableDataRowElement : TextElement
    {
        private static readonly Type[] PossibleChildren = {typeof(TableCellElement)};

        public string DataSourceProperty { get; set; }
        public string[] ColumnCellProperties { get; set; }

        protected override bool IsParentType => true;
        protected override Type[] AllowedChildrenTypes => PossibleChildren;
    }
}