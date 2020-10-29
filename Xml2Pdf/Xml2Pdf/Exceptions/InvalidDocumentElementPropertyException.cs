using Xml2Pdf.DocumentStructure;

namespace Xml2Pdf.Exceptions
{
    public class InvalidDocumentElementPropertyException : System.Exception
    {
        private InvalidDocumentElementPropertyException(string message) : base(message) { }

        public InvalidDocumentElementPropertyException(DocumentElement documentElement,
                                                       string propertyName,
                                                       string propertyValue)
            : this($"Type: '{documentElement.GetType()}' - Invalid property: '{propertyName}'='{propertyValue}'.")
        {
        }
    }
}