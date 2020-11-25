using System;
using System.Text;
using iText.Kernel.Geom;

namespace Xml2Pdf.DocumentStructure.Form
{
    public abstract class FormElement : DocumentElement
    {
        public ElementProperty<string> Name { get; } = new ElementProperty<string>();
        public ElementProperty<string> Value { get; } = new ElementProperty<string>();
        // public ElementProperty<Rectangle> Rectangle { get; } = new ElementProperty<Rectangle>();

        internal override void DumpToStringBuilder(StringBuilder dumpBuilder, int indent)
        {
            base.DumpToStringBuilder(dumpBuilder, indent);
            DumpElementProperty(dumpBuilder, indent, nameof(Name), Name);
            DumpElementProperty(dumpBuilder, indent, nameof(Value), Value);
            // DumpElementProperty(dumpBuilder, indent, nameof(Rectangle), Rectangle);
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