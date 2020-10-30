using System;

namespace Xml2Pdf.Format.Formatters
{
    public class ToStringFormatter<T> : IPropertyFormatter
    {
        public int Priority => 0;
        public Type RegisteredType { get; } = typeof(T);
        public string Format(object value) => value.ToString();
    }
}