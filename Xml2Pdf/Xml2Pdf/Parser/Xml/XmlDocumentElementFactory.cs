using Xml2Pdf.Exceptions;
using Xml2Pdf.DocumentStructure;

namespace Xml2Pdf.Parser.Xml
{
    internal static class XmlDocumentElementFactory
    {
        internal static DocumentElement CreateDocumentElement(string elementName)
        {
            return elementName switch
            {
                Constants.PdfDocumentRootElement => new RootDocumentElement(),
                _ => throw new InvalidDocumentElementException(elementName)
            };
        }
    }
}