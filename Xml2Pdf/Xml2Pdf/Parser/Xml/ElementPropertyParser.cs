using System;
using Xml2Pdf.DocumentStructure;
using Xml2Pdf.DocumentStructure.Geometry;
using Xml2Pdf.Exceptions;
using Xml2Pdf.Utilities;

namespace Xml2Pdf.Parser.Xml
{
    internal static class ElementPropertyParser
    {
        internal static void ParseAndAssignElementProperties(DocumentElement documentElement,
                                                             PropertyBag<string> propertyBag)
        {
            if (documentElement is BorderedDocumentElement borderedElement)
            {
                AssignBorderedDocumentElementProperties(borderedElement, propertyBag);
            }

            if (documentElement is TextElement textElement)
            {
                AssignTextElementProperties(textElement, propertyBag);
            }

            switch (documentElement)
            {
                case RootDocumentElement rootDocumentElement:
                    AssignRootDocumentElementProperties(rootDocumentElement, propertyBag);
                    break;
                case ParagraphElement paragraphElement:
                    AssignParagraphElementProperties(paragraphElement, propertyBag);
                    break;
                case PageElement _: // No properties to be parsed yet.
                    return;
                default:
                    ColorConsole.WriteLine(ConsoleColor.Red,
                                           $"Missing branch in `ParseAndAssignElementProperty` for '{documentElement.GetType().Name}'");
                    break;
            }
        }

        private static void AssignTextElementProperties(TextElement textElement, PropertyBag<string> propertyBag)
        {
            foreach (var pair in propertyBag.UnprocessedPairs())
            {
                switch (pair.Name)
                {
                    case "horizontalAlignment":
                        textElement.HorizontalAlignment = ValueParser.ParseHorizontalAlignment(pair.Value);
                        break;
                    case "verticalAlignment":
                        textElement.VerticalAlignment = ValueParser.ParseVerticalAlignment(pair.Value);
                        break;
                    case "textAlignment":
                        textElement.TextAlignment = ValueParser.ParseTextAlignment(pair.Value);
                        break;
                    case "bold":
                        textElement.Bold = ValueParser.ParseBool(pair.Value);
                        break;
                    case "italic":
                        textElement.Italic = ValueParser.ParseBool(pair.Value);
                        break;
                    case "underline":
                        textElement.Underline = ValueParser.ParseBool(pair.Value);
                        break;
                    case "subscript":
                        textElement.Subscript = ValueParser.ParseBool(pair.Value);
                        break;
                    case "superscript":
                        textElement.Superscript = ValueParser.ParseBool(pair.Value);
                        break;
                    case "fontSize":
                        textElement.FontSize = ValueParser.ParseFloat(pair.Value);
                        break;
                    case "foreground":
                    case "foregroundColor":
                        textElement.ForegroundColor = ValueParser.ParseColor(pair.Value);
                        break;
                    case "background":
                    case "backgroundColor":
                        textElement.BackgroundColor = ValueParser.ParseColor(pair.Value);
                        break;
                }
            }
        }

        private static void AssignBorderedDocumentElementProperties(BorderedDocumentElement borderedElement,
                                                                    PropertyBag<string> propertyBag)
        {
            foreach (var pair in propertyBag.UnprocessedPairs())
            {
                switch (pair.Name)
                {
                    case "borderColor":
                        if (borderedElement.Borders == null)
                        {
                            borderedElement.Borders = new BorderInfo
                            {
                                BorderType = BorderType.Solid
                            };
                        }

                        borderedElement.Borders.Color = ValueParser.ParseColor(pair.Value);
                        break;
                    case "borderWidth":
                        if (borderedElement.Borders == null)
                        {
                            borderedElement.Borders = new BorderInfo
                            {
                                BorderType = BorderType.Solid
                            };
                        }

                        borderedElement.Borders.Width = ValueParser.ParseFloat(pair.Value);

                        break;
                    case "borderType":
                        borderedElement.Borders ??= new BorderInfo();
                        borderedElement.Borders.BorderType = ValueParser.ParseBorderType(pair.Value);
                        break;
                    case "topBorderColor":
                        if (borderedElement.Borders == null)
                        {
                            borderedElement.Borders = new BorderInfo
                            {
                                BorderType = BorderType.Solid
                            };
                        }

                        borderedElement.TopBorder.Color = ValueParser.ParseColor(pair.Value);
                        break;
                    case "topBorderWidth":
                        if (borderedElement.Borders == null)
                        {
                            borderedElement.Borders = new BorderInfo
                            {
                                BorderType = BorderType.Solid
                            };
                        }

                        borderedElement.TopBorder.Width = ValueParser.ParseFloat(pair.Value);
                        break;
                    case "topBorderType":
                        borderedElement.TopBorder ??= new BorderInfo();
                        borderedElement.TopBorder.BorderType = ValueParser.ParseBorderType(pair.Value);
                        break;
                    case "bottomBorderColor":
                        if (borderedElement.Borders == null)
                        {
                            borderedElement.Borders = new BorderInfo
                            {
                                BorderType = BorderType.Solid
                            };
                        }

                        borderedElement.BottomBorder.Color = ValueParser.ParseColor(pair.Value);
                        break;
                    case "bottomBorderWidth":
                        if (borderedElement.Borders == null)
                        {
                            borderedElement.Borders = new BorderInfo
                            {
                                BorderType = BorderType.Solid
                            };
                        }

                        borderedElement.BottomBorder.Width = ValueParser.ParseFloat(pair.Value);
                        break;
                    case "bottomBorderType":
                        borderedElement.BottomBorder ??= new BorderInfo();
                        borderedElement.BottomBorder.BorderType = ValueParser.ParseBorderType(pair.Value);
                        break;
                    case "leftBorderColor":
                        if (borderedElement.Borders == null)
                        {
                            borderedElement.Borders = new BorderInfo
                            {
                                BorderType = BorderType.Solid
                            };
                        }

                        borderedElement.LeftBorder.Color = ValueParser.ParseColor(pair.Value);
                        break;
                    case "leftBorderWidth":
                        if (borderedElement.Borders == null)
                        {
                            borderedElement.Borders = new BorderInfo
                            {
                                BorderType = BorderType.Solid
                            };
                        }

                        borderedElement.LeftBorder.Width = ValueParser.ParseFloat(pair.Value);
                        break;
                    case "leftBorderType":
                        borderedElement.LeftBorder ??= new BorderInfo();
                        borderedElement.LeftBorder.BorderType = ValueParser.ParseBorderType(pair.Value);
                        break;
                    case "rightBorderColor":
                        if (borderedElement.Borders == null)
                        {
                            borderedElement.Borders = new BorderInfo
                            {
                                BorderType = BorderType.Solid
                            };
                        }

                        borderedElement.RightBorder.Color = ValueParser.ParseColor(pair.Value);
                        break;
                    case "rightBorderWidth":
                        if (borderedElement.Borders == null)
                        {
                            borderedElement.Borders = new BorderInfo
                            {
                                BorderType = BorderType.Solid
                            };
                        }

                        borderedElement.RightBorder.Width = ValueParser.ParseFloat(pair.Value);
                        break;
                    case "rightBorderType":
                        borderedElement.RightBorder ??= new BorderInfo();
                        borderedElement.RightBorder.BorderType = ValueParser.ParseBorderType(pair.Value);
                        break;
                }

                /*
        public BorderInfo TopBorder { get; set; }
        public BorderInfo BottomBorder { get; set; }
        public BorderInfo LeftBorder { get; set; }
        public BorderInfo RightBorder { get; set; }
                 * 
                 */
            }
        }

        private static void AssignParagraphElementProperties(ParagraphElement paragraphElement,
                                                             PropertyBag<string> propertyBag)
        {
            foreach (var (name, value) in propertyBag.UnprocessedPairs())
            {
                switch (name)
                {
                    case "text":
                        paragraphElement.TextBuilder.Append(value);
                        break;
                    case "property":
                        paragraphElement.Property = value;
                        break;
                    case "format":
                        paragraphElement.Format = value;
                        break;
                    case "formatProperties":
                        paragraphElement.FormatProperties = value.Split(',', ';');
                        break;
                    default:
                        throw new InvalidDocumentElementPropertyException(paragraphElement, name, value);
                }
            }
        }

        private static void AssignRootDocumentElementProperties(RootDocumentElement rootDocumentElement,
                                                                PropertyBag<string> propertyBag)
        {
            foreach (var (name, value) in propertyBag.UnprocessedPairs())
            {
                switch (name)
                {
                    case "margins":
                        rootDocumentElement.CustomMargins = ValueParser.ParseCompleteMargins(value);
                        break;
                    case "topMargin":
                        rootDocumentElement.CustomMargins = new Margins {Top = ValueParser.ParseFloat(value)};
                        break;
                    case "bottomMargin":
                        rootDocumentElement.CustomMargins = new Margins
                            {Bottom = ValueParser.ParseFloat(value)};
                        break;
                    case "leftMargin":
                        rootDocumentElement.CustomMargins = new Margins {Left = ValueParser.ParseFloat(value)};
                        break;
                    case "rightMargin":
                        rootDocumentElement.CustomMargins = new Margins {Right = ValueParser.ParseFloat(value)};
                        break;
                    case "pageSize":
                        rootDocumentElement.PageSize = ValueParser.ParsePageSize(value);
                        break;
                    case "pageOrientation":
                        rootDocumentElement.PageOrientation = ValueParser.ParsePageOrientation(value);
                        break;
                    default:
                        throw new InvalidDocumentElementPropertyException(rootDocumentElement, name, value);
                }
            }
        }
    }
}