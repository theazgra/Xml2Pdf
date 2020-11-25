using System;

namespace Xml2Pdf.Exceptions
{
    public class TextException : Exception
    {
        private TextException(string message) : base(message)
        {
        }

        internal static TextException PropertyNotFound(string propertyName) =>
            new($"Property with name '{propertyName}' wasn't found in the property map.");

        internal static TextException MissingFormatProperties() =>
            new($"Format properties aren't available for format string.");

        internal static TextException WrongTypeForRawText(Type wrongType) =>
            new(
                $"Trying to assign text to Non-Paragraph DocumentElement. Actual element type={wrongType}");
    }
}