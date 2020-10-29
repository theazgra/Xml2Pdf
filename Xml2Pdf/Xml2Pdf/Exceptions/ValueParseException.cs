using System;

namespace Xml2Pdf.Exceptions
{
    public class ValueParseException : Exception
    {
        public ValueParseException(string message) : base(message) { }
    }
}