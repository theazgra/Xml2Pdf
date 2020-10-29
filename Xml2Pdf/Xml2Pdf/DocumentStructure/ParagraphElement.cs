using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Xml2Pdf.Exceptions;
using Xml2Pdf.Format;

namespace Xml2Pdf.DocumentStructure
{
    public class ParagraphElement : TextElement
    {
        public string Text { get; set; }
        public string Property { get; set; }
        public string Format { get; set; }
        public string[] FormatProperties { get; set; }

        public override bool IsParentType => false;
        public override Type[] AllowedChildrenTypes => Array.Empty<Type>();

        [SuppressMessage("ReSharper", "CoVariantArrayConversion")]
        public string GetTextToRender(IDictionary<string, object> objectPropertyMap, ValueFormatter formatter)
        {
            string GetAndFormatProperty(string property) => formatter.FormatValue(objectPropertyMap[property]);
            if (Text != null)
                return Text;
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

        internal override void DumpToStringBuilder(StringBuilder dumpBuilder, int indentationLevel)
        {
            PrepareIndent(dumpBuilder, indentationLevel).Append("<Paragraph>").AppendLine();
            if (Text != null)
                PrepareIndent(dumpBuilder, indentationLevel).Append(" -Text=").Append(Text).AppendLine();

            if (Property != null)
                PrepareIndent(dumpBuilder, indentationLevel).Append(" -Property=").Append(Property).AppendLine();

            if (Format != null)
                PrepareIndent(dumpBuilder, indentationLevel).Append(" -Format=").Append(Format).AppendLine();
            if (FormatProperties != null)
            {
                PrepareIndent(dumpBuilder, indentationLevel)
                    .Append(" -FormatProperties=")
                    .Append(FormatProperties)
                    .AppendLine();
            }

            foreach (var child in Children)
            {
                child.DumpToStringBuilder(dumpBuilder, indentationLevel + 2);
            }
        }
    }
}