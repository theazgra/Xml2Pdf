using Xml2Pdf.Exceptions;
using Xml2Pdf.DocumentStructure;
using Xml2Pdf.DocumentStructure.Form;

namespace Xml2Pdf.Parser.Xml
{
    internal static class DocumentElementFactory
    {
        internal static DocumentElement CreateDocumentElement(string elementName)
        {
            return elementName switch
            {
                "Paragraph" => new ParagraphElement(),
                "Text" => new LeafTextElement(),
                "PdfDocument" => new RootDocumentElement(),
                "Page" => new PageElement(),
                "Line" => new LineElement(),
                "List" => new ListElement(),
                "ListItem" => new ListItemElement(),
                "Image" => new ImageElement(),
                "Table" => new TableElement(),
                "TableRow" => new TableRowElement(),
                "Cell" => new TableCellElement(),
                "TableDataRow" => new TableDataRowElement(),
                "Header" => new HeaderElement(),
                "Footer" => new FooterElement(),
                "Spacer" => new SpacerElement(),
                "TextField" => new TextFieldElement(),
                _ => throw new InvalidDocumentElementException(elementName)
            };
        }
    }
}