using System;
using iText.Kernel.Colors;
using iText.Layout.Properties;
using Xml2Pdf.DocumentStructure.Geometry;

namespace Xml2Pdf.DocumentStructure
{
    public class LineElement : BorderedDocumentElement
    {
        protected override bool IsParentType => false;
        protected override Type[] AllowedChildrenTypes => Array.Empty<Type>();

        public ElementProperty<UnitValue> Length { get; } = new();
        public ElementProperty<HorizontalAlignment> Alignment { get; } = new();

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