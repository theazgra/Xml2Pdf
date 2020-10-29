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
                Constants.RootDocumentElement => new RootDocumentElement(),
                Constants.Page => new PageElement(),
                Constants.Line => new LineElement(),
                Constants.Image => new ImageElement(),
                Constants.Table => new TableElement(),
                Constants.Row => new TableRowElement(),
                Constants.Cell => new TableCellElement(),
                Constants.RowDataTemplate => new TableDataRowElement(),
                Constants.Paragraph => new ParagraphElement(),
                Constants.Header => new HeaderElement(),
                Constants.Footer => new FooterElement(),
                _ => throw new InvalidDocumentElementException(elementName)
            };
        }
    }
}