using System;

namespace Xml2Pdf.Format.Formatters
{
    public class ToStringFormatter : IPropertyFormatter
    {
        public Type[] RegisteredTypes { get; } = {typeof(object)};
        public string Format(object value) => value.ToString();
    }
}