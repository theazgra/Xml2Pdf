using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using iText.Kernel.Colors;
using iText.Layout.Properties;
using Xml2Pdf.Exceptions;
using Xml2Pdf.Format;
using Xml2Pdf.Utilities;

namespace Xml2Pdf.DocumentStructure
{
    public class TextElement : BorderedDocumentElement
    {
#region TextProperties

        public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.TOP;
        public TextAlignment TextAlignment { get; set; } = TextAlignment.LEFT;
        public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.LEFT;
        public bool Bold { get; set; } = false;
        public bool Italic { get; set; } = false;
        public bool Superscript { get; set; } = false;
        public bool Subscript { get; set; } = false;
        public bool Underline { get; set; }
        public float FontSize { get; set; }
        public Color ForegroundColor { get; set; } = ColorConstants.BLACK;
        public Color BackgroundColor { get; set; } = ColorConstants.WHITE;

#endregion

        public StringBuilder TextBuilder { get; } = new StringBuilder(0);

        public string Property { get; set; }
        public string Format { get; set; }
        public string[] FormatProperties { get; set; }

        public bool IsEmpty()
        {
            return TextBuilder.Length == 0 && Property == null && Format == null && FormatProperties == null;
        }


        public override bool IsParentType => false;
        public override Type[] AllowedChildrenTypes => Array.Empty<Type>();

        internal override void DumpToStringBuilder(StringBuilder dumpBuilder, int indentationLevel)
        {
            base.DumpToStringBuilder(dumpBuilder, indentationLevel);
            PrepareIndent(dumpBuilder, indentationLevel).Append("(TextElement)").AppendLine();

            PrepareIndent(dumpBuilder, indentationLevel)
                .Append(" -TextAlignment: ")
                .Append(TextAlignment)
                .AppendLine();

            PrepareIndent(dumpBuilder, indentationLevel)
                .Append(" -HorizontalAlignment: ")
                .Append(HorizontalAlignment)
                .AppendLine();

            PrepareIndent(dumpBuilder, indentationLevel)
                .Append(" -VerticalAlignment: ")
                .Append(VerticalAlignment)
                .AppendLine();

            PrepareIndent(dumpBuilder, indentationLevel).Append(" -Bold: ").Append(Bold).AppendLine();

            PrepareIndent(dumpBuilder, indentationLevel).Append(" -Italic: ").Append(Italic).AppendLine();

            PrepareIndent(dumpBuilder, indentationLevel).Append(" -Superscript: ").Append(Superscript).AppendLine();

            PrepareIndent(dumpBuilder, indentationLevel).Append(" -Subscript: ").Append(Subscript).AppendLine();

            PrepareIndent(dumpBuilder, indentationLevel).Append(" -Underline: ").Append(Underline).AppendLine();

            PrepareIndent(dumpBuilder, indentationLevel).Append(" -FontSize: ").Append(FontSize).AppendLine();

            PrepareIndent(dumpBuilder, indentationLevel)
                .Append(" -ForegroundColor: ")
                .Append(ForegroundColor.ToPrettyString())
                .AppendLine();

            PrepareIndent(dumpBuilder, indentationLevel)
                .Append(" -BackgroundColor: ")
                .Append(BackgroundColor.ToPrettyString())
                .AppendLine();

            if (TextBuilder.Length != 0)
                PrepareIndent(dumpBuilder, indentationLevel)
                    .Append(" -Text='")
                    .Append(TextBuilder.ToString())
                    .Append('\'')
                    .AppendLine();

            if (Property != null)
                PrepareIndent(dumpBuilder, indentationLevel)
                    .Append(" -Property='")
                    .Append(Property)
                    .Append('\'')
                    .AppendLine();

            if (Format != null)
                PrepareIndent(dumpBuilder, indentationLevel)
                    .Append(" -Format='")
                    .Append(Format)
                    .Append('\'')
                    .AppendLine();
            if (FormatProperties != null)
            {
                PrepareIndent(dumpBuilder, indentationLevel)
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