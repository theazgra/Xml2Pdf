using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using iText.Kernel.Font;
using iText.Layout;
using Xml2Pdf.DocumentStructure;
using Xml2Pdf.Exceptions;
using Xml2Pdf.Renderer;
using Xml2Pdf.Utilities;

namespace Xml2Pdf.Parser.Xml
{
    internal class StyleParser
    {
        public void ParseStyle(XmlReader xmlReader, ElementStyle result)
        {
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
                            case "Font":
                            case "CustomFont":
                                ParseAndInjectCustomFont(xmlReader, result.CustomFonts);
                                break;
                            case "ParagraphStyle":
                                result.ParagraphStyle = ParseTextStyle(xmlReader.Name, xmlReader, result.CustomFonts);
                                break;
                            case "TableCellStyle":
                                result.TableCellStyle = ParseTextStyle(xmlReader.Name, xmlReader, result.CustomFonts);
                                break;
                            case "ListItemStyle":
                                result.ListItemStyle = ParseTextStyle(xmlReader.Name, xmlReader, result.CustomFonts);
                                break;
                            case "TableStyle":
                                result.TableStyle = ParseBorderedElementStyle(xmlReader.Name, xmlReader);
                                break;
                            case "LineStyle":
                                result.LineStyle = ParseBorderedElementStyle(xmlReader.Name, xmlReader);
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
        }

        private void ParseAndInjectCustomFont(XmlReader xmlReader, Dictionary<string, PdfFont> customFontsMap)
        {
            string fontName = xmlReader.GetAttribute("name");
            string fontPath = xmlReader.GetAttribute("path");

            if (string.IsNullOrEmpty(fontName) || string.IsNullOrEmpty(fontPath))
            {
                throw new ValueParseException("Unable to parse custom font. Specify both name and path.");
            }

            PdfFont loadedFont;
            try
            {
                loadedFont = PdfFontFactory.CreateFont(fontPath, true);
            }
            catch (Exception e)
            {
                throw new ValueParseException($"Failed to load custom font '{fontName}' from path: '{fontPath}'", e);
            }

            customFontsMap.Add(fontName, loadedFont);
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

        private StyleWrapper ParseTextStyle(string enclosingNodeName, XmlReader xmlReader, Dictionary<string, PdfFont> customFonts)
        {
            var propertyBag = ReadWhileInEnclosingNode(xmlReader, enclosingNodeName);
            TextElement textElement = new LeafTextElement();
            ElementPropertyParser.ParseAndAssignElementProperties(textElement, propertyBag);
            return textElement.GetElementStyle(customFonts);
        }

        private StyleWrapper ParseBorderedElementStyle(string enclosingNodeName, XmlReader xmlReader)
        {
            var propertyBag = ReadWhileInEnclosingNode(xmlReader, enclosingNodeName);
            BorderedDocumentElement borderedElement = new LineElement();
            ElementPropertyParser.ParseAndAssignElementProperties(borderedElement, propertyBag);
            return borderedElement.GetElementStyle(null);
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