using System;

namespace Xml2Pdf.Format
{
    public interface IPropertyFormatter
    {
        int Priority { get; }
        Type RegisteredType { get; }
        string Format(object value);
    }
}