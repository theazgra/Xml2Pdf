using System;
using System.Text;
using iText.Kernel.Colors;
using Xml2Pdf.DocumentStructure.Geometry;

namespace Xml2Pdf.DocumentStructure.Form
{
    public abstract class FormElement : DocumentElement
    {
        public ElementProperty<string> Name { get; } = new();
        public ElementProperty<string> Value { get; } = new();

        public ElementProperty<string> FontName { get; } = new();
        public ElementProperty<float> FontSize { get; } = new();
        public ElementProperty<Color> ForegroundColor { get; } = new();
        public ElementProperty<Color> BackgroundColor { get; } = new();
        public ElementProperty<BorderInfo> Borders { get; } = new();
        public ElementProperty<bool> ReadOnly { get; } = new();

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
            DumpElementProperty(dumpBuilder, indent, nameof(ReadOnly), ReadOnly);
        }
    }

    public sealed class TextFieldElement : FormElement
    {
        protected override bool IsParentType => false;
        protected override Type[] AllowedChildrenTypes => Array.Empty<Type>();

        public ElementProperty<bool> IsMultiline { get; } = new();

        internal override void DumpToStringBuilder(StringBuilder dumpBuilder, int indent)
        {
            base.DumpToStringBuilder(dumpBuilder, indent);
            DumpElementProperty(dumpBuilder, indent, nameof(IsMultiline), IsMultiline);
        }
    }
}