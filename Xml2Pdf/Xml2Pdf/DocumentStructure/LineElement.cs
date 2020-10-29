using System;

namespace Xml2Pdf.DocumentStructure
{
    public class LineElement : BorderedDocumentElement
    {
        public float Width { get; set; }

        public override bool IsParentType => false;
        public override Type[] AllowedChildrenTypes => Array.Empty<Type>();
    }
}