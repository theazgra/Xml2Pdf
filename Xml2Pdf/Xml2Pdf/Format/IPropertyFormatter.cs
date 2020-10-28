using System;

namespace Xml2Pdf.Format
{
    public interface IPropertyFormatter
    {
        Type[] RegisteredTypes { get; }
        string Format(object value);
    }
}