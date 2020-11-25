using System;
using System.Text;
using iText.Layout.Properties;

namespace Xml2Pdf.DocumentStructure
{
    public class ImageElement : DocumentElement
    {
        public ElementProperty<string> Path { get; } = new();
        public ElementProperty<string> SourceProperty { get; } = new();
        public ElementProperty<float> HorizontalScaling { get; } = new();
        public ElementProperty<float> VerticalScaling { get; } = new();

        protected override bool IsParentType => false;
        protected override Type[] AllowedChildrenTypes => Array.Empty<Type>();

        internal override void DumpToStringBuilder(StringBuilder dumpBuilder, int indent)
        {
            base.DumpToStringBuilder(dumpBuilder, indent);
            DumpElementProperty(dumpBuilder, indent, nameof(Path), Path);
            DumpElementProperty(dumpBuilder, indent, nameof(SourceProperty), SourceProperty);
            DumpElementProperty(dumpBuilder, indent, nameof(FixedPosition), FixedPosition);
            DumpElementProperty(dumpBuilder, indent, nameof(HorizontalScaling), HorizontalScaling);
            DumpElementProperty(dumpBuilder, indent, nameof(VerticalScaling), VerticalScaling);
        }
    }
}