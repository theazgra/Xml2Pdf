using System;
using System.Linq;

namespace Xml2Pdf.Exceptions
{
    public class UnexpectedDocumentElementException : Exception
    {
        internal UnexpectedDocumentElementException(string message) : base(message) { }

        internal static UnexpectedDocumentElementException WrongDocumentElement(Type foundType,
            params Type[] expectedTypes)
        {
            string msg = expectedTypes.Length switch
            {
                0 => $"Unexpected document element of type: '{foundType.Name}'.",
                1 => $"Unexpected document element of type: '{foundType.Name}', expected: '{expectedTypes[0].Name}'.",
                _ => $"Unexpected document element of type '{foundType.Name}', expected: " +
                     string.Join(" or ", expectedTypes.Select(t => t.Name)) + '.'
            };
            throw new UnexpectedDocumentElementException(msg);
        }
    }
}