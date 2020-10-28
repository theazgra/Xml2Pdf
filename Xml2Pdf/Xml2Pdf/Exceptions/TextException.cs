using System;

namespace Xml2Pdf.Exceptions
{
    public class TextException : Exception
    {
        public TextException(string message) : base(message)
        {
        }

        internal static TextException PropertyNotFound(string propertyName) =>
            new TextException($"Property with name '{propertyName}' wasn't found in the property map.");

        internal static TextException MissingFormatProperties() =>
            new TextException($"Format properties aren't available for format string.");
    }
}