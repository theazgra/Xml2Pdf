using Xml2Pdf.Exceptions;
using Xml2Pdf.DocumentStructure;

namespace Xml2Pdf.Parser.Xml
{
    internal static class DocumentElementFactory
    {
        internal static DocumentElement CreateDocumentElement(string elementName)
        {
            return elementName switch
            {
                "Paragraph" => new ParagraphElement(),
                "Text" => new TextElement(),
                "PdfDocument" => new RootDocumentElement(),
                "Page" => new PageElement(),
                "Line" => new LineElement(),
                "Image" => new ImageElement(),
                "Table" => new TableElement(),
                "TableRow" => new TableRowElement(),
                "Cell" => new TableCellElement(),
                "TableDataRow" => new TableDataRowElement(),
                "Header" => new HeaderElement(),
                "Footer" => new FooterElement(),
                _ => throw new InvalidDocumentElementException(elementName)
            };
        }
    }
}