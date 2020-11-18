using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using Common.Logging.Configuration;
using iText.Layout;
using Microsoft.Extensions.DependencyModel.Resolution;
using Xml2Pdf.DocumentStructure;
using Xml2Pdf.Exceptions;
using Xml2Pdf.Utilities;

namespace Xml2Pdf.Parser.Xml
{
    internal class StyleParser
    {
        private const string EntryName = "Entry";

        public ElementStyle ParseStyle(XmlReader xmlReader)
        {
            ElementStyle result = new ElementStyle();
            bool isStyleElementClosed = false;
            while (!isStyleElementClosed && xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (xmlReader.Name)
                        {
                            case "Color":
                                ParseAndInjectColor(xmlReader);
                                break;
                            case "ParagraphStyle":
                                result.ParagraphStyle = ParseTextStyle(xmlReader.Name, xmlReader);
                                break;
                            default:
                                ColorConsole.WriteLine(ConsoleColor.DarkBlue,
                                    $"Unhandled style node. '{xmlReader.Name}'");
                                break;
                        }

                        break;
                    case XmlNodeType.XmlDeclaration:
                    case XmlNodeType.EndElement:
                        if (xmlReader.Name == "Style")
                        {
                            isStyleElementClosed = true;
                        }

                        break;
                    default:
                        Console.WriteLine($"Unhandled style node type: {xmlReader.NodeType}");
                        break;
                }
            }

            Debug.Assert(isStyleElementClosed);
            return result;
        }

        private PropertyBag<string> ReadWhileInEnclosingNode(XmlReader xmlReader, string enclosingNodeName)
        {
            List<PropertyPair<string>> pairs = new List<PropertyPair<string>>();
            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        pairs.Add(new PropertyPair<string>(GetNameValueAttributePair(xmlReader)));
                        break;
                    case XmlNodeType.EndElement:
                        if (xmlReader.Name == enclosingNodeName)
                        {
                            return new PropertyBag<string>(pairs.ToArray());
                        }

                        break;
                }
            }

            Debug.Assert(false, "You should never reach this.");
            return null;
        }

        private Style ParseTextStyle(string enclosingNodeName, XmlReader xmlReader)
        {
            var propertyBag = ReadWhileInEnclosingNode(xmlReader, enclosingNodeName);
            TextElement textElement = new TextElement();
            ElementPropertyParser.ParseAndAssignElementProperties(textElement, propertyBag);
            // StringBuilder sb = new StringBuilder();
            // textElement.DumpToStringBuilder(sb, 0);
            // Console.WriteLine("-------------------------------------------");
            // Console.WriteLine(sb.ToString());
            // Console.WriteLine("-------------------------------------------");
            return textElement.TextPropertiesToStyle();
        }

        private (string name, string value) GetNameValueAttributePair(XmlReader xmlReader)
        {
            return (xmlReader.GetAttribute("name"), xmlReader.GetAttribute("value"));
        }

        private void ParseAndInjectColor(XmlReader xmlReader)
        {
            if (xmlReader.AttributeCount != 2)
                throw new ValueParseException("Custom color in style must have two attributes, name and value.");

            (string name, string value) = GetNameValueAttributePair(xmlReader);
            var parsedColor = ValueParser.ParseColor(value);
            ValueParser.InjectNewColor(name, parsedColor);
        }
    }
}