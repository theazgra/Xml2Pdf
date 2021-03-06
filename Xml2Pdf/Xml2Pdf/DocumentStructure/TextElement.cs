﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Layout.Properties;
using Xml2Pdf.Exceptions;
using Xml2Pdf.Format;
using Xml2Pdf.Renderer;
using Xml2Pdf.Utilities;

namespace Xml2Pdf.DocumentStructure
{
    public abstract class TextElement : BorderedDocumentElement
    {
#region TextProperties

        public ElementProperty<VerticalAlignment> VerticalAlignment { get; } = new();

        public ElementProperty<TextAlignment> TextAlignment { get; } = new();

        public ElementProperty<HorizontalAlignment> HorizontalAlignment { get; } = new();

        public ElementProperty<bool> Bold { get; } = new();
        public ElementProperty<bool> Italic { get; } = new();
        public ElementProperty<bool> Superscript { get; } = new();
        public ElementProperty<bool> Subscript { get; } = new();
        public ElementProperty<bool> Underline { get; } = new();
        public ElementProperty<string> FontName { get; } = new();
        public ElementProperty<float> FontSize { get; } = new();
        public ElementProperty<Color> ForegroundColor { get; } = new();
        public ElementProperty<Color> BackgroundColor { get; } = new();

#endregion

        public string Text { get; set; }

        public string Property { get; set; }
        public string Format { get; set; }
        public string[] FormatProperties { get; set; }

        public TextElement() { }

        public bool IsEmpty() { return Text == null && Property == null && Format == null && FormatProperties == null; }


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

        public override StyleWrapper GetElementStyle(Dictionary<string, PdfFont> customFonts, PageSize page)
        {
            var style = base.GetElementStyle(customFonts, page);
            if (FontSize.IsInitialized)
                style.SetFontSize(FontSize.Value);
            if (FontName.IsInitialized && customFonts.ContainsKey(FontName.Value))
                style.SetFont(customFonts[FontName.Value]);
            if (HorizontalAlignment.IsInitialized)
                style.SetHorizontalAlignment(HorizontalAlignment.Value);
            if (TextAlignment.IsInitialized)
                style.SetTextAlignment(TextAlignment.Value);
            if (ForegroundColor.IsInitialized)
                style.SetFontColor(ForegroundColor.Value);
            if (BackgroundColor.IsInitialized)
                style.SetBackgroundColor(BackgroundColor.Value);

            if (Bold.ValueOr(false))
                style.SetBold();
            if (Italic.ValueOr(false))
                style.SetItalic();
            if (Underline.ValueOr(false))
                style.SetUnderline();

            return style;
        }
    }

    /// <summary>
    /// TextElement which is leaf, meaning it can't have any children.
    /// </summary>
    public class LeafTextElement : TextElement
    {
        protected override bool IsParentType => false;
        protected override Type[] AllowedChildrenTypes => Array.Empty<Type>();
    }
}