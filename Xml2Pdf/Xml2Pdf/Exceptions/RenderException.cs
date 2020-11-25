using System;
using System.Linq;

namespace Xml2Pdf.Exceptions
{
    public class RenderException : System.Exception
    {
        public RenderException(string message) : base(message) { }

        internal static RenderException WrongPdfParent(string methodName, Type expected, Type actual) =>
            new($"Wrong PDF parent object in '{methodName}()'. " +
                $"Expected type: '{expected.Name}' but got '{actual.Name}'");

        internal static RenderException WrongPdfParent(string methodName, Type[] expected, Type actual) =>
            new($"Wrong PDF parent object in '{methodName}()'. " +
                $"Expected type: '{string.Join(" or ", expected.Select(e => e.Name))}' but got '{actual.Name}'");
    }
}