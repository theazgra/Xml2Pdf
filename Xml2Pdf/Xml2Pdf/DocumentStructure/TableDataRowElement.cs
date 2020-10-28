namespace Xml2Pdf.DocumentStructure
{
    public class TableDataRowElement : TextElement
    {
        public string DataSourceProperty { get; set; }
        public string[] ColumnCellProperties { get; set; }
    }
}