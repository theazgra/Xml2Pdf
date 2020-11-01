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
                case TableElement tableElement:
                    AssignTableElementProperties(tableElement, propertyBag);
                    break;
                case TableRowElement tableRowElement:
                    AssignTableRowElementProperties(tableRowElement, propertyBag);
                    break;
                case ParagraphElement _:
                case TextElement _:
                case PageElement _: // No properties to be parsed yet.
                    return;
                default:
                    ColorConsole.WriteLine(ConsoleColor.Red,
                                           $"Missing branch in `ParseAndAssignElementProperty` for '{documentElement.GetType().Name}'");
                    break;
            }
        }

        private static void AssignTableRowElementProperties(TableRowElement tableRowElement,
                                                            PropertyBag<string> propertyBag)
        {
            foreach (var pair in propertyBag.UnprocessedPairs())
            {
                switch (pair.Name)
                {
                    case "rowHeight":
                        tableRowElement.RowHeight.Value = ValueParser.ParseFloat(pair.Value);
                        break;
                    case "header":
                        tableRowElement.IsHeader.Value = ValueParser.ParseBool(pair.Value);
                        break;
                    default:
                        throw new InvalidDocumentElementPropertyException(tableRowElement, pair.Name, pair.Value);
                }
            }
        }

        private static void AssignTableElementProperties(TableElement tableElement, PropertyBag<string> propertyBag)
        {
            foreach (var pair in propertyBag.UnprocessedPairs())
            {
                switch (pair.Name)
                {
                    case "columnCount":
                        tableElement.ColumnCount.Value = ValueParser.ParseInt(pair.Value);
                        break;
                    case "columnWidths":
                        tableElement.ColumnWidths.Value = ValueParser.ParseFloatArray(pair.Value);
                        tableElement.ColumnCount.Value = tableElement.ColumnWidths.Value.Length;
                        break;
                    case "largeTable":
                        tableElement.LargeTable.Value = ValueParser.ParseBool(pair.Value);
                        break;
                    case "verticalBorderSpacing":
                        tableElement.VerticalBorderSpacing.Value = ValueParser.ParseFloat(pair.Value);
                        break;
                    case "horizontalBorderSpacing":
                        tableElement.HorizontalBorderSpacing.Value = ValueParser.ParseFloat(pair.Value);
                        break;
                    case "rowHeight":
                        tableElement.RowHeight.Value = ValueParser.ParseFloat(pair.Value);
                        break;
                    default:
                        throw new InvalidDocumentElementPropertyException(tableElement, pair.Name, pair.Value);
                }
            }
        }

        private static void AssignTextElementProperties(TextElement textElement, PropertyBag<string> propertyBag)
        {
            foreach (var pair in propertyBag.UnprocessedPairs())
            {
                switch (pair.Name)
                {
                    case "horizontalAlignment":
                        textElement.HorizontalAlignment.Value = ValueParser.ParseHorizontalAlignment(pair.Value);
                        break;
                    case "verticalAlignment":
                        textElement.VerticalAlignment.Value = ValueParser.ParseVerticalAlignment(pair.Value);
                        break;
                    case "textAlignment":
                        textElement.TextAlignment.Value = ValueParser.ParseTextAlignment(pair.Value);
                        break;
                    case "bold":
                        textElement.Bold.Value = ValueParser.ParseBool(pair.Value);
                        break;
                    case "italic":
                        textElement.Italic.Value = ValueParser.ParseBool(pair.Value);
                        break;
                    case "underline":
                        textElement.Underline.Value = ValueParser.ParseBool(pair.Value);
                        break;
                    case "subscript":
                        textElement.Subscript.Value = ValueParser.ParseBool(pair.Value);
                        break;
                    case "superscript":
                        textElement.Superscript.Value = ValueParser.ParseBool(pair.Value);
                        break;
                    case "fontSize":
                        textElement.FontSize.Value = ValueParser.ParseFloat(pair.Value);
                        break;
                    case "foreground":
                    case "foregroundColor":
                        textElement.ForegroundColor.Value = ValueParser.ParseColor(pair.Value);
                        break;
                    case "background":
                    case "backgroundColor":
                        textElement.BackgroundColor.Value = ValueParser.ParseColor(pair.Value);
                        break;
                    case "property":
                        textElement.Property = pair.Value;
                        break;
                    case "format":
                        textElement.Format = pair.Value;
                        break;
                    case "formatProperties":
                        textElement.FormatProperties = pair.Value.Split(',', ';');
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
                        if (borderedElement.Borders.Value == null)
                        {
                            borderedElement.Borders.Value = new BorderInfo
                            {
                                BorderType = BorderType.Solid
                            };
                        }

                        borderedElement.Borders.Value.Color = ValueParser.ParseColor(pair.Value);
                        break;
                    case "borderWidth":
                        if (borderedElement.Borders.Value == null)
                        {
                            borderedElement.Borders.Value = new BorderInfo
                            {
                                BorderType = BorderType.Solid
                            };
                        }

                        borderedElement.Borders.Value.Width = ValueParser.ParseFloat(pair.Value);

                        break;
                    case "borderType":
                        borderedElement.Borders.Value ??= new BorderInfo();
                        borderedElement.Borders.Value.BorderType = ValueParser.ParseBorderType(pair.Value);
                        break;
                    case "topBorderColor":
                        if (borderedElement.Borders.Value == null)
                        {
                            borderedElement.Borders.Value = new BorderInfo
                            {
                                BorderType = BorderType.Solid
                            };
                        }

                        borderedElement.TopBorder.Value.Color = ValueParser.ParseColor(pair.Value);
                        break;
                    case "topBorderWidth":
                        if (borderedElement.Borders.Value == null)
                        {
                            borderedElement.Borders.Value = new BorderInfo
                            {
                                BorderType = BorderType.Solid
                            };
                        }

                        borderedElement.TopBorder.Value.Width = ValueParser.ParseFloat(pair.Value);
                        break;
                    case "topBorderType":
                        borderedElement.TopBorder.Value ??= new BorderInfo();
                        borderedElement.TopBorder.Value.BorderType = ValueParser.ParseBorderType(pair.Value);
                        break;
                    case "bottomBorderColor":
                        if (borderedElement.Borders.Value == null)
                        {
                            borderedElement.Borders.Value = new BorderInfo
                            {
                                BorderType = BorderType.Solid
                            };
                        }

                        borderedElement.BottomBorder.Value.Color = ValueParser.ParseColor(pair.Value);
                        break;
                    case "bottomBorderWidth":
                        if (borderedElement.Borders.Value == null)
                        {
                            borderedElement.Borders.Value = new BorderInfo
                            {
                                BorderType = BorderType.Solid
                            };
                        }

                        borderedElement.BottomBorder.Value.Width = ValueParser.ParseFloat(pair.Value);
                        break;
                    case "bottomBorderType":
                        borderedElement.BottomBorder.Value ??= new BorderInfo();
                        borderedElement.BottomBorder.Value.BorderType = ValueParser.ParseBorderType(pair.Value);
                        break;
                    case "leftBorderColor":
                        if (borderedElement.Borders.Value == null)
                        {
                            borderedElement.Borders.Value = new BorderInfo
                            {
                                BorderType = BorderType.Solid
                            };
                        }

                        borderedElement.LeftBorder.Value.Color = ValueParser.ParseColor(pair.Value);
                        break;
                    case "leftBorderWidth":
                        if (borderedElement.Borders.Value == null)
                        {
                            borderedElement.Borders.Value = new BorderInfo
                            {
                                BorderType = BorderType.Solid
                            };
                        }

                        borderedElement.LeftBorder.Value.Width = ValueParser.ParseFloat(pair.Value);
                        break;
                    case "leftBorderType":
                        borderedElement.LeftBorder.Value ??= new BorderInfo();
                        borderedElement.LeftBorder.Value.BorderType = ValueParser.ParseBorderType(pair.Value);
                        break;
                    case "rightBorderColor":
                        if (borderedElement.Borders.Value == null)
                        {
                            borderedElement.Borders.Value = new BorderInfo
                            {
                                BorderType = BorderType.Solid
                            };
                        }

                        borderedElement.RightBorder.Value.Color = ValueParser.ParseColor(pair.Value);
                        break;
                    case "rightBorderWidth":
                        if (borderedElement.Borders.Value == null)
                        {
                            borderedElement.Borders.Value = new BorderInfo
                            {
                                BorderType = BorderType.Solid
                            };
                        }

                        borderedElement.RightBorder.Value.Width = ValueParser.ParseFloat(pair.Value);
                        break;
                    case "rightBorderType":
                        borderedElement.RightBorder.Value ??= new BorderInfo();
                        borderedElement.RightBorder.Value.BorderType = ValueParser.ParseBorderType(pair.Value);
                        break;
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