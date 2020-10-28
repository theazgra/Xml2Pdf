using Xml2Pdf.DocumentStructure;

namespace Xml2Pdf.Parser.Interface
{
    public interface IDocumentTemplateParser
    {
        RootDocumentElement ParseTemplateFile(string filePath);
    }
}