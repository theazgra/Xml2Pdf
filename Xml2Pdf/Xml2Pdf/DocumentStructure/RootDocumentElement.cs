using System;
using iText.Kernel.Geom;
using Xml2Pdf.DocumentStructure.Geometry;

namespace Xml2Pdf.DocumentStructure
{
    public class RootDocumentElement : DocumentElement
    {
        // TODO(Moravec): Add style element?
        private static readonly Type[] PossibleChildren = {typeof(PageElement)};
        
        public Margins? CustomMargins { get; set; } = null;
        public PageSize PageSize { get; set; } = PageSize.A4;
        public PageOrientation PageOrientation { get; set; } = PageOrientation.Portrait;

        public override bool IsParentType => true;
        public override Type[] AllowedChildrenTypes => AllowedChildrenTypes;
    }
}