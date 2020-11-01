using System;
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

        public ElementProperty<VerticalAlignment> VerticalAlignment { get; set; } =
            new ElementProperty<VerticalAlignment>();

        public ElementProperty<TextAlignment> TextAlignment { get; set; } = new ElementProperty<TextAlignment>();

        public ElementProperty<HorizontalAlignment> HorizontalAlignment { get; set; } =
            new ElementProperty<HorizontalAlignment>();

        public ElementProperty<bool> Bold { get; set; } = new ElementProperty<bool>();
        public ElementProperty<bool> Italic { get; set; } = new ElementProperty<bool>();
        public ElementProperty<bool> Superscript { get; set; } = new ElementProperty<bool>();
        public ElementProperty<bool> Subscript { get; set; } = new ElementProperty<bool>();
        public ElementProperty<bool> Underline { get; set; } = new ElementProperty<bool>();
        public ElementProperty<float> FontSize { get; set; } = new ElementProperty<float>();
        public ElementProperty<Color> ForegroundColor { get; set; } = new ElementProperty<Color>();
        public ElementProperty<Color> BackgroundColor { get; set; } = new ElementProperty<Color>();

#endregion

        public StringBuilder TextBuilder { get; } = new StringBuilder(0);

        public string Property { get; set; }
        public string Format { get; set; }
        public string[] FormatProperties { get; set; }

        public TextElement()
        {
            // OnChildAdded += child =>
            // {
            //     if (child is TextElement)
            //     {
            //         ColorConsole.WriteLine(ConsoleColor.Blue,
            //                                "Child of TextElement is also TextElement and can inherit properties.");
            //     }
            // };
        }

        public bool IsEmpty()
        {
            return TextBuilder.Length == 0 && Property == null && Format == null && FormatProperties == null;
        }


        internal override void DumpToStringBuilder(StringBuilder dumpBuilder, int indent)
        {
            base.DumpToStringBuilder(dumpBuilder, indent);
            PrepareIndent(dumpBuilder, indent).Append("(TextElement)").AppendLine();

            DumpElementProperty(dumpBuilder, indent, "TextAlignment", TextAlignment);
            DumpElementProperty(dumpBuilder, indent, "HorizontalAlignment", HorizontalAlignment);
            DumpElementProperty(dumpBuilder, indent, "VerticalAlignment", VerticalAlignment);

            DumpElementProperty(dumpBuilder, indent, "Bold", Bold);
            DumpElementProperty(dumpBuilder, indent, "Italic", Italic);
            DumpElementProperty(dumpBuilder, indent, "Superscript", Superscript);
            DumpElementProperty(dumpBuilder, indent, "Subscript", Subscript);
            DumpElementProperty(dumpBuilder, indent, "Underline", Underline);
            DumpElementProperty(dumpBuilder, indent, "FontSize", FontSize);


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