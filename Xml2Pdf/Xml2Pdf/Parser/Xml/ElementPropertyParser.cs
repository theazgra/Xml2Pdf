using System;
using Xml2Pdf.DocumentStructure;
using Xml2Pdf.DocumentStructure.Geometry;
using Xml2Pdf.Exceptions;

namespace Xml2Pdf.Parser.Xml
{
    internal static class ElementPropertyParser
    {
        internal static void ParseAndAssignElementProperty(DocumentElement documentElement,
                                                           string propertyName,
                                                           string propertyValue)
        {
            if (documentElement is RootDocumentElement rootDocumentElement)
                AssignRootDocumentElementProperties(rootDocumentElement, propertyName, propertyValue);
            else if (documentElement is ParagraphElement paragraphElement)
                AssignParagraphElementProperties(paragraphElement, propertyName, propertyValue);
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error
                       .WriteLine($"Missing branch in `ParseAndAssignElementProperty` for '{documentElement.GetType().Name}'");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private static void AssignParagraphElementProperties(ParagraphElement paragraphElement,
                                                             string propertyName,
                                                             string propertyValue)
        {
            switch (propertyName)
            {
                case Constants.TextAttribute:
                    paragraphElement.Text = propertyValue;
                    break;
                case Constants.PropertyAttribute:
                    paragraphElement.Property = propertyValue;
                    break;
                case Constants.FormatAttribute:
                    paragraphElement.Format = propertyValue;
                    break;
                case Constants.FormatPropertiesAttribute:
                    paragraphElement.FormatProperties = propertyValue.Split(',', ';');
                    break;
                default:
                    throw new InvalidDocumentElementPropertyException(paragraphElement, propertyName, propertyValue);
            }
        }

        private static void AssignRootDocumentElementProperties(RootDocumentElement rootDocumentElement,
                                                                string propertyName,
                                                                string propertyValue)
        {
            switch (propertyName)
            {
                case Constants.Margins:
                    rootDocumentElement.CustomMargins = ValueParser.ParseCompleteMargins(propertyValue);
                    break;
                case Constants.TopMargin:
                    rootDocumentElement.CustomMargins = new Margins {Top = ValueParser.ParseFloat(propertyValue)};
                    break;
                case Constants.BottomMargin:
                    rootDocumentElement.CustomMargins = new Margins {Bottom = ValueParser.ParseFloat(propertyValue)};
                    break;
                case Constants.LeftMargin:
                    rootDocumentElement.CustomMargins = new Margins {Left = ValueParser.ParseFloat(propertyValue)};
                    break;
                case Constants.RightMargin:
                    rootDocumentElement.CustomMargins = new Margins {Right = ValueParser.ParseFloat(propertyValue)};
                    break;
                case Constants.PageSize:
                    rootDocumentElement.PageSize = ValueParser.ParsePageSize(propertyValue);
                    break;
                case Constants.PageOrientation:
                    rootDocumentElement.PageOrientation = ValueParser.ParsePageOrientation(propertyValue);
                    break;
                default:
                    throw new InvalidDocumentElementPropertyException(rootDocumentElement, propertyName, propertyValue);
            }
        }
    }
}