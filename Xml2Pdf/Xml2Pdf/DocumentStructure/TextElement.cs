using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using iText.Kernel.Colors;
using iText.Layout;
using iText.Layout.Properties;
using Xml2Pdf.Exceptions;
using Xml2Pdf.Format;
using Xml2Pdf.Utilities;

namespace Xml2Pdf.DocumentStructure
{
    public class TextElement : BorderedDocumentElement
    {
        protected override bool IsParentType => false;
        protected override Type[] AllowedChildrenTypes => Array.Empty<Type>();

        #region TextProperties

        public ElementProperty<VerticalAlignment> VerticalAlignment { get; } =
            new ElementProperty<VerticalAlignment>();

        public ElementProperty<TextAlignment> TextAlignment { get; } = new ElementProperty<TextAlignment>();

        public ElementProperty<HorizontalAlignment> HorizontalAlignment { get; } =
            new ElementProperty<HorizontalAlignment>();

        public ElementProperty<string> FontName { get; } = new ElementProperty<string>();
        public ElementProperty<bool> Bold { get; } = new ElementProperty<bool>();
        public ElementProperty<bool> Italic { get; } = new ElementProperty<bool>();
        public ElementProperty<bool> Superscript { get; } = new ElementProperty<bool>();
        public ElementProperty<bool> Subscript { get; } = new ElementProperty<bool>();
        public ElementProperty<bool> Underline { get; } = new ElementProperty<bool>();
        public ElementProperty<float> FontSize { get; } = new ElementProperty<float>();
        public ElementProperty<Color> ForegroundColor { get; } = new ElementProperty<Color>();
        public ElementProperty<Color> BackgroundColor { get; } = new ElementProperty<Color>();

        #endregion

        public string Text { get; set; }

        public string Property { get; set; }
        public string Format { get; set; }
        public string[] FormatProperties { get; set; }

        public TextElement()
        {
            OnChildAdded += child =>
            {
                if (child is TextElement textChild)
                {
                    textChild.InheritFrom(this);
                }
            };
        }

        private void InheritFrom(TextElement other)
        {
            if (!FontName.IsInitialized && other.FontName.IsInitialized)
                FontName.Value = other.FontName.Value;
            if (!VerticalAlignment.IsInitialized && other.VerticalAlignment.IsInitialized)
                VerticalAlignment.Value = other.VerticalAlignment.Value;
            if (!TextAlignment.IsInitialized && other.TextAlignment.IsInitialized)
                TextAlignment.Value = other.TextAlignment.Value;
            if (!HorizontalAlignment.IsInitialized && other.HorizontalAlignment.IsInitialized)
                HorizontalAlignment.Value = other.HorizontalAlignment.Value;
            if (!Bold.IsInitialized && other.Bold.IsInitialized)
                Bold.Value = other.Bold.Value;
            if (!Italic.IsInitialized && other.Italic.IsInitialized)
                Italic.Value = other.Italic.Value;
            if (!Superscript.IsInitialized && other.Superscript.IsInitialized)
                Superscript.Value = other.Superscript.Value;
            if (!Subscript.IsInitialized && other.Subscript.IsInitialized)
                Subscript.Value = other.Subscript.Value;
            if (!Underline.IsInitialized && other.Underline.IsInitialized)
                Underline.Value = other.Underline.Value;
            if (!FontSize.IsInitialized && other.FontSize.IsInitialized)
                FontSize.Value = other.FontSize.Value;
            if (!ForegroundColor.IsInitialized && other.ForegroundColor.IsInitialized)
                ForegroundColor.Value = other.ForegroundColor.Value;
            if (!BackgroundColor.IsInitialized && other.BackgroundColor.IsInitialized)
                BackgroundColor.Value = other.BackgroundColor.Value;
        }


        public bool IsEmpty()
        {
            return Text == null && Property == null && Format == null && FormatProperties == null;
        }


        internal override void DumpToStringBuilder(StringBuilder dumpBuilder, int indent)
        {
            base.DumpToStringBuilder(dumpBuilder, indent);
            PrepareIndent(dumpBuilder, indent).Append("(TextElement)").AppendLine();

            DumpElementProperty(dumpBuilder, indent, nameof(FontName), FontName);
            DumpElementProperty(dumpBuilder, indent, nameof(Bold), Bold);
            DumpElementProperty(dumpBuilder, indent, nameof(Italic), Italic);
            DumpElementProperty(dumpBuilder, indent, nameof(TextAlignment), TextAlignment);
            DumpElementProperty(dumpBuilder, indent, nameof(HorizontalAlignment), HorizontalAlignment);
            DumpElementProperty(dumpBuilder, indent, nameof(VerticalAlignment), VerticalAlignment);
            DumpElementProperty(dumpBuilder, indent, nameof(Superscript), Superscript);
            DumpElementProperty(dumpBuilder, indent, nameof(Subscript), Subscript);
            DumpElementProperty(dumpBuilder, indent, nameof(Underline), Underline);
            DumpElementProperty(dumpBuilder, indent, nameof(FontSize), FontSize);


            if (ForegroundColor.IsInitialized)
            {
                PrepareIndent(dumpBuilder, indent)
                    .Append(" -ForegroundColor: ")
                    .Append(ForegroundColor.Value.ToPrettyString())
                    .AppendLine();
            }

            if (BackgroundColor.IsInitialized)
            {
                PrepareIndent(dumpBuilder, indent)
                    .Append(" -BackgroundColor: ")
                    .Append(BackgroundColor.Value.ToPrettyString())
                    .AppendLine();
            }

            if (Text != null)
                PrepareIndent(dumpBuilder, indent)
                    .Append(" -Text='")
                    .Append(Text.ToString())
                    .Append('\'')
                    .AppendLine();

            if (Property != null)
                PrepareIndent(dumpBuilder, indent)
                    .Append(" -Property='")
                    .Append(Property)
                    .Append('\'')
                    .AppendLine();

            if (Format != null)
                PrepareIndent(dumpBuilder, indent)
                    .Append(" -Format='")
                    .Append(Format)
                    .Append('\'')
                    .AppendLine();
            if (FormatProperties != null)
            {
                PrepareIndent(dumpBuilder, indent)
                    .Append(" -FormatProperties='")
                    .Append(FormatProperties)
                    .Append('\'')
                    .AppendLine();
            }
        }

        [SuppressMessage("ReSharper", "CoVariantArrayConversion")]
        public string GetTextToRender(IDictionary<string, object> objectPropertyMap, ValueFormatter formatter)
        {
            string GetAndFormatProperty(string property) => formatter.FormatValue(objectPropertyMap[property]);
            if (Text != null)
                return Text.ToString();
            if (Property != null)
            {
                if (!objectPropertyMap.ContainsKey(Property))
                    throw TextException.PropertyNotFound(Property);

                return GetAndFormatProperty(Property);
            }

            if (Format != null)
            {
                if (FormatProperties == null)
                    throw TextException.MissingFormatProperties();

                string[] values = FormatProperties.Select(GetAndFormatProperty).ToArray();
                return string.Format(Format, values);
            }

            return string.Empty;
        }

        public Style TextPropertiesToStyle()
        {
            Style textStyle = BorderPropertiesToStyle();

            if (HorizontalAlignment.IsInitialized)
                textStyle.SetHorizontalAlignment(HorizontalAlignment.Value);
            if (TextAlignment.IsInitialized)
                textStyle.SetTextAlignment(TextAlignment.Value);
            if (ForegroundColor.IsInitialized)
                textStyle.SetFontColor(ForegroundColor.Value);
            if (BackgroundColor.IsInitialized)
                textStyle.SetBackgroundColor(BackgroundColor.Value);

            if (Bold.ValueOr(false))
                textStyle.SetBold();
            if (Italic.ValueOr(false))
                textStyle.SetItalic();
            if (Underline.ValueOr(false))
                textStyle.SetUnderline();

            return textStyle;
        }
    }

    /// <summary>
    /// TextElement which is leaf, meaning it can't have any children.
    /// </summary>
    public class LeafTextElement : TextElement
    {
        protected override bool IsParentType => false;
        protected override Type[] AllowedChildrenTypes => null;
    }
}