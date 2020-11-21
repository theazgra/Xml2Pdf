using System;

namespace Xml2Pdf.DocumentStructure
{
    public class SpacerElement : DocumentElement
    {
        protected override bool IsParentType => false;
        protected override Type[] AllowedChildrenTypes => Array.Empty<Type>();

    }
}