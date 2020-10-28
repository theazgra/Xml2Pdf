
namespace Xml2Pdf.DocumentStructure
{
    public class TableRowElement : TextElement
    {
        public bool IsHeader { get; set; } = false;
        public float RowHeight { get; set; }
    }
}
