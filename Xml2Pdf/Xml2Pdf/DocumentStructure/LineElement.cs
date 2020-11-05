using System;
using iText.Kernel.Colors;
using Xml2Pdf.DocumentStructure.Geometry;

namespace Xml2Pdf.DocumentStructure
{
    public class LineElement : BorderedDocumentElement
    {
        public override bool IsParentType => false;
        public override Type[] AllowedChildrenTypes => Array.Empty<Type>();

        public LineElement()
        {
            // Default border.
            BottomBorder.Value = new BorderInfo
            {
                Color = ColorConstants.BLACK,
                BorderType = BorderType.Solid,
                Width = 1.0f,
                Opacity = 1.0f
            };
        }
    }
}