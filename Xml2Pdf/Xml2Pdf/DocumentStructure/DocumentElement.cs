using System.Collections.Generic;

namespace Xml2Pdf.DocumentStructure
{
    public abstract class DocumentElement
    {
        protected List<DocumentElement> children;

        public IReadOnlyList<DocumentElement> Children => children.AsReadOnly();
        public bool HasChildren => (children.Count > 0);

        protected DocumentElement()
        {
        }
    }
}