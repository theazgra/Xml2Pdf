using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Xml;
using Xml2Pdf.DocumentStructure;
using Xml2Pdf.Exceptions;
using Xml2Pdf.Parser.Interface;
using static Xml2Pdf.Parser.Xml.DocumentElementFactory;

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

            while (!xmlReader.EOF && (xmlReader.NodeType != XmlNodeType.Element && xmlReader.Name != "PdfDocument"))
            {
                xmlReader.Read();
            }

            RootDocumentElement root = ParseDocumentElement(xmlReader, null) as RootDocumentElement;

            var parentElements = new Stack<DocumentElement>();
            parentElements.Push(root);

            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        parentElements.Push(ParseDocumentElement(xmlReader, parentElements.Peek()));
                        // lastParentElement = ;
                        break;
                    case XmlNodeType.Text:
                        ParseXmlText(xmlReader, parentElements.Peek());
                        break;
                    case XmlNodeType.XmlDeclaration:
                    case XmlNodeType.EndElement:
                        parentElements.Pop();
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
            if (lastParsedElement is TextElement textElement)
            {
                textElement.TextBuilder.Append(xmlReader.Value);
            }
            else
            {
                throw TextException.WrongTypeForRawText(lastParsedElement.Children.Last().GetType());
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

            return currentElement;
        }

        private void ParseXmlAttributes(XmlReader xmlReader, DocumentElement currentElement)
        {
            System.Diagnostics.Debug.Assert(xmlReader.HasAttributes);
            PropertyBag<string> propertyBag = new PropertyBag<string>(xmlReader.AttributeCount);

            for (int attributeIndex = 0; attributeIndex < xmlReader.AttributeCount; attributeIndex++)
            {
                xmlReader.MoveToAttribute(attributeIndex);
                propertyBag[attributeIndex] = new PropertyPair<string>(xmlReader.Name, xmlReader.Value);
            }

            ElementPropertyParser.ParseAndAssignElementProperties(currentElement, propertyBag);

            xmlReader.MoveToElement();
        }
    }
}