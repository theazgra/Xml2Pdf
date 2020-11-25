using System;
using System.Text;
using iText.Kernel.Colors;
using Xml2Pdf.DocumentStructure.Geometry;

namespace Xml2Pdf.DocumentStructure.Form
{
    public abstract class FormElement : DocumentElement
    {
        public ElementProperty<string> Name { get; } = new ElementProperty<string>();
        public ElementProperty<string> Value { get; } = new ElementProperty<string>();

        public ElementProperty<string> FontName { get; } = new ElementProperty<string>();
        public ElementProperty<float> FontSize { get; } = new ElementProperty<float>();
        public ElementProperty<Color> ForegroundColor { get; } = new ElementProperty<Color>();
        public ElementProperty<Color> BackgroundColor { get; } = new ElementProperty<Color>();
        public ElementProperty<BorderInfo> Borders { get; } = new ElementProperty<BorderInfo>();

        internal override void DumpToStringBuilder(StringBuilder dumpBuilder, int indent)
        {
            base.DumpToStringBuilder(dumpBuilder, indent);
            DumpElementProperty(dumpBuilder, indent, nameof(Name), Name);
            DumpElementProperty(dumpBuilder, indent, nameof(Value), Value);
            DumpElementProperty(dumpBuilder, indent, nameof(FontName), FontName);
            DumpElementProperty(dumpBuilder, indent, nameof(FontSize), FontSize);
            DumpElementProperty(dumpBuilder, indent, nameof(ForegroundColor), ForegroundColor);
            DumpElementProperty(dumpBuilder, indent, nameof(BackgroundColor), BackgroundColor);
            DumpElementProperty(dumpBuilder, indent, nameof(Borders), Borders);
        }
    }

    public sealed class TextFieldElement : FormElement
    {
        protected override bool IsParentType => false;
        protected override Type[] AllowedChildrenTypes => Array.Empty<Type>();

        public ElementProperty<bool> IsMultiline { get; } = new ElementProperty<bool>();

        internal override void DumpToStringBuilder(StringBuilder dumpBuilder, int indent)
        {
            base.DumpToStringBuilder(dumpBuilder, indent);
            DumpElementProperty(dumpBuilder, indent, nameof(IsMultiline), IsMultiline);
        }
    }
}