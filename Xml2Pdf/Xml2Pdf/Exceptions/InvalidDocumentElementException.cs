using System;

namespace Xml2Pdf.Exceptions
{
    public class InvalidDocumentElementException : Exception
    {
        internal InvalidDocumentElementException(string elementName)
            : base($"Invalid DocumentElement with name {elementName}")
        {
        }
    }
}