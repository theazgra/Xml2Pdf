using System;
using System.IO;
using System.Xml;
using Xml2Pdf.DocumentStructure;
using Xml2Pdf.Exceptions;
using Xml2Pdf.Parser.Interface;
using static Xml2Pdf.Parser.Xml.XmlDocumentElementFactory;

namespace Xml2Pdf.Parser.Xml
{
    public class XmlDocumentTemplateParser : IDocumentTemplateParser
    {
        public RootDocumentElement ParseTemplateFile(string filePath)
        {
            using var fileStream = File.Open(path: filePath, FileMode.Open);
            return ParseTemplate(fileStream);
        }

        private RootDocumentElement ParseTemplate(Stream inputStream)
        {
            var xmlReaderSettings = new XmlReaderSettings
            {
                IgnoreComments = true,
                IgnoreWhitespace = true
            };
            using var xmlReader = XmlReader.Create(inputStream, xmlReaderSettings);

            while (!xmlReader.EOF && (xmlReader.NodeType != XmlNodeType.Element &&
                                      xmlReader.Name != Constants.PdfDocumentRootElement))
            {
                xmlReader.Read();
            }

            RootDocumentElement root = ParseDocumentElement(xmlReader, null) as RootDocumentElement;
            DocumentElement lastParentElement = root;

            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        lastParentElement = ParseDocumentElement(xmlReader, lastParentElement);
                        break;
                    case XmlNodeType.Text:
                        ParseXmlText(xmlReader, lastParentElement);
                        break;
                    case XmlNodeType.XmlDeclaration:
                    case XmlNodeType.EndElement:
                        break;
                    default:
                        Console.WriteLine($"Unhandled node type: {xmlReader.NodeType}");
                        break;
                }
            }

            Console.WriteLine("Finished parsing.");
            return root;
        }

        private void ParseXmlText(XmlReader xmlReader, DocumentElement lastParsedElement)
        {
            if (lastParsedElement is ParagraphElement paragraph)
            {
                paragraph.Text = xmlReader.Value;
            }
            else
            {
                throw TextException.WrongTypeForRawText(lastParsedElement.GetType());
            }
        }

        private DocumentElement ParseDocumentElement(XmlReader xmlReader, DocumentElement parentElement)
        {
            var currentElement = CreateDocumentElement(xmlReader.Name);
            if (parentElement == null)
            {
                if (currentElement.GetType() != typeof(RootDocumentElement))
                {
                    throw UnexpectedDocumentElementException.WrongDocumentElement(currentElement.GetType(),
                        typeof(RootDocumentElement));
                }
            }
            else
            {
                // This will check if current element can be child of lastParsedElement.
                parentElement.AddChild(currentElement);
            }

            if (xmlReader.HasAttributes)
            {
                ParseXmlAttributes(xmlReader, currentElement);
            }

            return currentElement.IsParentType ? currentElement : parentElement;
        }

        private void ParseXmlAttributes(XmlReader xmlReader, DocumentElement currentElement)
        {
            return;
            // if (xmlReader.HasAttributes)
            // {
            //     for (int i = 0; i < xmlReader.AttributeCount; i++)
            //     {
            //         xmlReader.MoveToAttribute(i);
            //         Console.WriteLine($"Attribute: {xmlReader.Name}; Value: {xmlReader.Value}");
            //     }
            //
            //     xmlReader.MoveToElement();
            // }
        }
    }
}