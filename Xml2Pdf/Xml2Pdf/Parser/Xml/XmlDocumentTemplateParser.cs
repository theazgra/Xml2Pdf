using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            string templateDirectoryPath = new FileInfo(filePath).Directory?.FullName;
            using var fileStream = File.Open(path: filePath, FileMode.Open);
            return ParseTemplate(fileStream, templateDirectoryPath);
        }

        private RootDocumentElement ParseTemplate(Stream inputStream, string templateDirectory)
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

            RootDocumentElement root = (RootDocumentElement) ParseDocumentElement(xmlReader, null);
            if (root.StyleFile.IsInitialized)
            {
                new StyleParser().ParseStyle(Path.Combine(templateDirectory, root.StyleFile.Value), root.Style);
            }

            var parentElements = new Stack<DocumentElement>();
            parentElements.Push(root);

            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (xmlReader.Name == "Style")
                        {
                            new StyleParser().ParseStyle(xmlReader, root.Style);
                            continue;
                        }

                        parentElements.Push(ParseDocumentElement(xmlReader, parentElements.Peek()));
                        if (xmlReader.IsEmptyElement)
                        {
                            parentElements.Pop();
                        }

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
                textElement.Text = xmlReader.Value;
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
                // This will check if current element can be child of parentElement.
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