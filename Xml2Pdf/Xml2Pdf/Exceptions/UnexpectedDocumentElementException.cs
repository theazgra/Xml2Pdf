using System;
using System.Linq;

namespace Xml2Pdf.Exceptions
{
    public class UnexpectedDocumentElementException : Exception
    {
        internal UnexpectedDocumentElementException(string message) : base(message)
        {
        }

        internal static UnexpectedDocumentElementException WrongDocumentElement(Type foundType,
            params Type[] expectedTypes)
        {
            string msg = expectedTypes.Length switch
            {
                0 => $"Found unexpected document element of type: {foundType}",
                1 => $"Found unexpected document element of type: {foundType}, expected: {expectedTypes[0]}",
                _ => string.Join(", ", expectedTypes.Select(t => t.ToString()))
            };
            throw new UnexpectedDocumentElementException(msg);
        }
    }
}