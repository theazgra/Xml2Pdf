using System;
using System.Collections.Generic;
using iText.Kernel.Colors;
using Xml2Pdf.DocumentStructure;
using Xml2Pdf.DocumentStructure.Geometry;
using Xml2Pdf.Exceptions;
using Xml2Pdf.Utilities;

namespace Xml2Pdf.Parser.Xml
{
    internal static class ElementPropertyParser
    {
        internal static void ParseAndAssignElementProperties(DocumentElement documentElement,
                                                             KeyValuePair<string, string>[] propertyBag)
        {
            switch (documentElement)
            {
                case RootDocumentElement rootDocumentElement:
                    ColorConsole.WriteLine(ConsoleColor.Green, "Matched RootDocumentElement");
                    AssignRootDocumentElementProperties(rootDocumentElement, propertyBag);
                    break;
                case ParagraphElement paragraphElement:
                    ColorConsole.WriteLine(ConsoleColor.Green, "Matched ParagraphElement");
                    AssignParagraphElementProperties(paragraphElement, propertyBag);
                    break;
                case PageElement _: // No properties to be parsed yet.
                    ColorConsole.WriteLine(ConsoleColor.Green, "Matched PageElement");
                    return;
                default:
                    ColorConsole.WriteLine(ConsoleColor.Red,
                                           $"Missing branch in `ParseAndAssignElementProperty` for '{documentElement.GetType().Name}'");
                    break;
            }
        }

        private static void AssignParagraphElementProperties(ParagraphElement paragraphElement,
                                                             KeyValuePair<string, string>[] propertyBag)
        {
            foreach (var pair in propertyBag)
            {
                switch (pair.Key)
                {
                    case Constants.TextAttribute:
                        paragraphElement.Text = pair.Value;
                        break;
                    case Constants.PropertyAttribute:
                        paragraphElement.Property = pair.Value;
                        break;
                    case Constants.FormatAttribute:
                        paragraphElement.Format = pair.Value;
                        break;
                    case Constants.FormatPropertiesAttribute:
                        paragraphElement.FormatProperties = pair.Value.Split(',', ';');
                        break;
                    default:
                        throw new InvalidDocumentElementPropertyException(paragraphElement, pair.Key, pair.Value);
                }
            }
        }

        private static void AssignRootDocumentElementProperties(RootDocumentElement rootDocumentElement,
                                                                KeyValuePair<string, string>[] propertyBag)
        {
            foreach (var pair in propertyBag)
            {
                switch (pair.Key)
                {
                    case Constants.Margins:
                        rootDocumentElement.CustomMargins = ValueParser.ParseCompleteMargins(pair.Value);
                        break;
                    case Constants.TopMargin:
                        rootDocumentElement.CustomMargins = new Margins {Top = ValueParser.ParseFloat(pair.Value)};
                        break;
                    case Constants.BottomMargin:
                        rootDocumentElement.CustomMargins = new Margins
                            {Bottom = ValueParser.ParseFloat(pair.Value)};
                        break;
                    case Constants.LeftMargin:
                        rootDocumentElement.CustomMargins = new Margins {Left = ValueParser.ParseFloat(pair.Value)};
                        break;
                    case Constants.RightMargin:
                        rootDocumentElement.CustomMargins = new Margins {Right = ValueParser.ParseFloat(pair.Value)};
                        break;
                    case Constants.PageSize:
                        rootDocumentElement.PageSize = ValueParser.ParsePageSize(pair.Value);
                        break;
                    case Constants.PageOrientation:
                        rootDocumentElement.PageOrientation = ValueParser.ParsePageOrientation(pair.Value);
                        break;
                    default:
                        throw new InvalidDocumentElementPropertyException(rootDocumentElement, pair.Key, pair.Value);
                }
            }
        }
    }
}