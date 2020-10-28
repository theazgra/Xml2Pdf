namespace Xml2Pdf.DocumentStructure
{
    public class TableElement : TextElement
    {
        public int ColumnCount { get; set; }
        public float[] ColumnWidths { get; set; }
        public bool LargeTable { get; set; }
        public float VerticalBorderSpacing { get; set; }
        public float HorizontalBorderSpacing { get; set; }
        public float RowHeight { get; set; }
    }
}