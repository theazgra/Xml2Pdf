namespace Xml2Pdf.DocumentStructure
{
    public class TableCellElement : TextElement
    {
        public int ColumnSpan { get; set; } = 0;
        public int RowSpan { get; set; } = 0;
    }
}