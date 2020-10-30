using Xml2Pdf.DocumentStructure;

namespace Xml2Pdf.Renderer.Interface
{
    interface IDocumentRenderer
    {
        bool RenderDocument(RootDocumentElement rootDocumentElement, string savePath, object dataObject);
        bool RenderDocument(RootDocumentElement rootDocumentElement, string savePath);
    }
}