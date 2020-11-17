using System;
using System.Diagnostics;
using System.Xml;
using Microsoft.Extensions.DependencyModel.Resolution;
using Xml2Pdf.DocumentStructure;
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
                        ColorConsole.WriteLine(ConsoleColor.DarkBlue, $"Unhandled style node. '{xmlReader.Name}'");
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
    }
}