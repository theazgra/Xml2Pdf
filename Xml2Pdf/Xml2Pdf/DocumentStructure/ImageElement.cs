using System;
using System.Text;
using iText.Layout.Properties;
using IntPoint = Xml2Pdf.DocumentStructure.Geometry.IntPoint;

namespace Xml2Pdf.DocumentStructure
{
    public class ImageElement : DocumentElement
    {
        public ElementProperty<string> Path { get; } = new ElementProperty<string>();
        public ElementProperty<string> SourceProperty { get; } = new ElementProperty<string>();
        public ElementProperty<IntPoint> FixedPosition { get; } = new ElementProperty<IntPoint>();
        public ElementProperty<UnitValue> Width { get; } = new ElementProperty<UnitValue>();
        public ElementProperty<float> HorizontalScaling { get; } = new ElementProperty<float>();
        public ElementProperty<float> VerticalScaling { get; } = new ElementProperty<float>();

        protected override bool IsParentType => false;
        protected override Type[] AllowedChildrenTypes => Array.Empty<Type>();

        internal override void DumpToStringBuilder(StringBuilder dumpBuilder, int indent)
        {
            base.DumpToStringBuilder(dumpBuilder, indent);
            DumpElementProperty(dumpBuilder, indent, nameof(Path), Path);
            DumpElementProperty(dumpBuilder, indent, nameof(SourceProperty), SourceProperty);
            DumpElementProperty(dumpBuilder, indent, nameof(FixedPosition), FixedPosition);
            DumpElementProperty(dumpBuilder, indent, nameof(Width), Width);
            DumpElementProperty(dumpBuilder, indent, nameof(HorizontalScaling), HorizontalScaling);
            DumpElementProperty(dumpBuilder, indent, nameof(VerticalScaling), VerticalScaling);
        }
    }
}