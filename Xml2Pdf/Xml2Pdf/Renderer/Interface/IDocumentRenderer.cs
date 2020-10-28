using Xml2Pdf.DocumentStructure;

namespace Xml2Pdf.Renderer.Interface
{
     interface IDocumentRenderer
     {
          bool RenderDocument(RootDocumentElement document, string savePath);
     }
}