﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using iText.Kernel.Colors;
using iText.Layout.Element;
using iText.Layout.Properties;
using Xml2Pdf.Exceptions;
using Xml2Pdf.Format;
using Xml2Pdf.Utilities;

namespace Xml2Pdf.DocumentStructure
{
    public class TextElement : BorderedDocumentElement
    {
        public override bool IsParentType => false;
        public override Type[] AllowedChildrenTypes => Array.Empty<Type>();

#region TextProperties

        public ElementProperty<VerticalAlignment> VerticalAlignment { get; } =
            new ElementProperty<VerticalAlignment>();

        public ElementProperty<TextAlignment> TextAlignment { get; } = new ElementProperty<TextAlignment>();

        public ElementProperty<HorizontalAlignment> HorizontalAlignment { get; } =
            new ElementProperty<HorizontalAlignment>();

        public ElementProperty<bool> Bold { get; } = new ElementProperty<bool>();
        public ElementProperty<bool> Italic { get; } = new ElementProperty<bool>();
        public ElementProperty<bool> Superscript { get; } = new ElementProperty<bool>();
        public ElementProperty<bool> Subscript { get; } = new ElementProperty<bool>();
        public ElementProperty<bool> Underline { get; } = new ElementProperty<bool>();
        public ElementProperty<float> FontSize { get; } = new ElementProperty<float>();
        public ElementProperty<Color> ForegroundColor { get; } = new ElementProperty<Color>();
        public ElementProperty<Color> BackgroundColor { get; } = new ElementProperty<Color>();

#endregion

        public StringBuilder TextBuilder { get; } = new StringBuilder(0);

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
            return TextBuilder.Length == 0 && Property == null && Format == null && FormatProperties == null;
        }


        internal override void DumpToStringBuilder(StringBuilder dumpBuilder, int indent)
        {
            base.DumpToStringBuilder(dumpBuilder, indent);
            PrepareIndent(dumpBuilder, indent).Append("(TextElement)").AppendLine();

            DumpElementProperty(dumpBuilder, indent, nameof(TextAlignment), TextAlignment);
            DumpElementProperty(dumpBuilder, indent, nameof(HorizontalAlignment), HorizontalAlignment);
            DumpElementProperty(dumpBuilder, indent, nameof(VerticalAlignment), VerticalAlignment);
            DumpElementProperty(dumpBuilder, indent, nameof(Bold), Bold);
            DumpElementProperty(dumpBuilder, indent, nameof(Italic), Italic);
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

            if (TextBuilder.Length != 0)
                PrepareIndent(dumpBuilder, indent)
                    .Append(" -Text='")
                    .Append(TextBuilder.ToString())
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
            if (TextBuilder.Length != 0)
                return TextBuilder.ToString();
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
    }
}