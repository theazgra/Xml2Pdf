using System;
using System.Diagnostics;
using System.Xml;
using Microsoft.Extensions.DependencyModel.Resolution;
using Xml2Pdf.DocumentStructure;
using Xml2Pdf.Exceptions;
using Xml2Pdf.Utilities;

namespace Xml2Pdf.Parser.Xml
{
    internal class StyleParser
    {
        public StyleElement ParseStyle(XmlReader xmlReader)
        {
            StyleElement result = new StyleElement();
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