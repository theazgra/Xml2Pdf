﻿using System;
using Xml2Pdf.DocumentStructure;
using Xml2Pdf.DocumentStructure.Form;
using Xml2Pdf.DocumentStructure.Geometry;
using Xml2Pdf.Exceptions;
using Xml2Pdf.Utilities;

namespace Xml2Pdf.Parser.Xml
{
    internal static class ElementPropertyParser
    {
        internal static void ParseAndAssignElementProperties(DocumentElement documentElement, PropertyBag<string> propertyBag)
        {
            AssignBaseDocumentElementProperties(documentElement, propertyBag);

            if (documentElement is BorderedDocumentElement borderedElement)
            {
                AssignBorderedDocumentElementProperties(borderedElement, propertyBag);
            }

            if (documentElement is TextElement textElement)
            {
                AssignTextElementProperties(textElement, propertyBag);
            }

            if (documentElement is FormElement formElement)
            {
                AssignFormElementProperties(formElement, propertyBag);
            }

            switch (documentElement)
            {
                case RootDocumentElement rootDocumentElement:
                    AssignRootDocumentElementProperties(rootDocumentElement, propertyBag);
                    break;
                case TableElement tableElement:
                    AssignTableElementProperties(tableElement, propertyBag);
                    break;
                case TableDataRowElement tableDataRowElement:
                    AssignTableDataRowElementProperties(tableDataRowElement, propertyBag);
                    break;
                case TableRowElement tableRowElement:
                    AssignTableRowElementProperties(tableRowElement, propertyBag);
                    break;
                case TableCellElement cellElement:
                    AssignTableCellElementProperties(cellElement, propertyBag);
                    break;
                case ListElement listElement:
                    AssignListElementProperties(listElement, propertyBag);
                    break;
                case LineElement lineElement:
                    AssignLineElementProperties(lineElement, propertyBag);
                    break;
                case ImageElement imageElement:
                    AssignImageElementProperties(imageElement, propertyBag);
                    break;
                case TextFieldElement textFieldElement:
                    AssignTextFieldElementProperties(textFieldElement, propertyBag);
                    break;
                case ListItemElement _:
                case ParagraphElement _:
                case TextElement _:
                case SpacerElement _:
                case PageElement _: // No properties to be parsed yet.
                    return;
                default:
                    ColorConsole.WriteLine(ConsoleColor.Red,
                                           $"Missing branch in `ParseAndAssignElementProperty` for '{documentElement.GetType().Name}'");
                    break;
            }
        }

        private static void AssignFormElementProperties(FormElement formElement, PropertyBag<string> propertyBag)
        {
            foreach (var pair in propertyBag.UnprocessedPairs())
            {
                switch (pair.Name)
                {
                    case "name":
                        formElement.Name.Value = pair.Value;
                        break;
                    case "value":
                        formElement.Value.Value = pair.Value;
                        break;
                    case "font":
                    case "fontName":
                        formElement.FontName.Value = pair.Value;
                        break;
                    case "fontSize":
                        formElement.FontSize.Value = ValueParser.ParseFloat(pair.Value);
                        break;
                    case "foreground":
                    case "foregroundColor":
                        formElement.ForegroundColor.Value = ValueParser.ParseColor(pair.Value);
                        break;
                    case "background":
                    case "backgroundColor":
                        formElement.BackgroundColor.Value = ValueParser.ParseColor(pair.Value);
                        break;
                    case "borders":
                        formElement.Borders.Value = ValueParser.ParseBorderInfo(pair.Value);
                        break;
                    case "readonly":
                        formElement.ReadOnly.Value = ValueParser.ParseBool(pair.Value);
                        break;
                }
            }
        }

        private static void AssignTextFieldElementProperties(TextFieldElement textFieldElement, PropertyBag<string> propertyBag)
        {
            foreach (var pair in propertyBag.UnprocessedPairs())
            {
                switch (pair.Name)
                {
                    case "multiline":
                    case "isMultiline":
                        textFieldElement.IsMultiline.Value = ValueParser.ParseBool(pair.Value);
                        break;
                    default:
                        throw new InvalidDocumentElementPropertyException(textFieldElement, pair.Name, pair.Value);
                }
            }
        }


        private static void AssignBaseDocumentElementProperties(DocumentElement documentElement, PropertyBag<string> propertyBag)
        {
            foreach (var pair in propertyBag.UnprocessedPairs())
            {
                switch (pair.Name)
                {
                    case "margins":
                        documentElement.Margins.Value = ValueParser.ParseCompleteMargins(pair.Value);

                        break;
                    case "marginTop":
                        documentElement.Margins.Value ??= new Margins();
                        documentElement.Margins.Value.Top = ValueParser.ParseFloat(pair.Value);
                        break;
                    case "marginBottom":
                        documentElement.Margins.Value ??= new Margins();
                        documentElement.Margins.Value.Bottom = ValueParser.ParseFloat(pair.Value);
                        break;
                    case "marginLeft":
                        documentElement.Margins.Value ??= new Margins();
                        documentElement.Margins.Value.Left = ValueParser.ParseFloat(pair.Value);
                        break;
                    case "marginRight":
                        documentElement.Margins.Value ??= new Margins();
                        documentElement.Margins.Value.Right = ValueParser.ParseFloat(pair.Value);
                        break;
                    case "rect":
                    case "position":
                        documentElement.FixedPosition.Value = ValueParser.ParseFixedPosition(pair.Value);
                        break;
                }
            }
        }

        private static void AssignImageElementProperties(ImageElement element, PropertyBag<string> propertyBag)
        {
            foreach (var pair in propertyBag.UnprocessedPairs())
            {
                switch (pair.Name)
                {
                    case "path":
                        element.Path.Value = pair.Value;
                        break;
                    case "property":
                    case "sourceProperty":
                        element.SourceProperty.Value = pair.Value;
                        break;
                    case "hScaling":
                    case "horizontalScaling":
                        element.HorizontalScaling.Value = ValueParser.ParseFloat(pair.Value);
                        break;
                    case "vScaling":
                    case "verticalScaling":
                        element.VerticalScaling.Value = ValueParser.ParseFloat(pair.Value);
                        break;
                    default:
                        throw new InvalidDocumentElementPropertyException(element, pair.Name, pair.Value);
                }
            }
        }

        private static void AssignLineElementProperties(LineElement lineElement, PropertyBag<string> propertyBag)
        {
            foreach (var pair in propertyBag.UnprocessedPairs())
            {
                switch (pair.Name)
                {
                    case "width":
                        lineElement.BottomBorder.Value.Width = ValueParser.ParseFloat(pair.Value);
                        break;
                    case "border":
                        lineElement.BottomBorder.Value = ValueParser.ParseBorderInfo(pair.Value);
                        break;
                    case "length":
                        lineElement.Length.Value = ValueParser.ParseUnitValue(pair.Value);
                        break;
                    case "align":
                    case "alignment":
                        lineElement.Alignment.Value = ValueParser.ParseHorizontalAlignment(pair.Value);
                        break;
                    case "color":
                        lineElement.BottomBorder.Value.Color = ValueParser.ParseColor(pair.Value);
                        break;
                    default:
                        throw new InvalidDocumentElementPropertyException(lineElement, pair.Name, pair.Value);
                }
            }
        }

        private static void AssignTableCellElementProperties(TableCellElement cellElement, PropertyBag<string> propertyBag)
        {
            foreach (var pair in propertyBag.UnprocessedPairs())
            {
                switch (pair.Name)
                {
                    case "columnSpan":
                    case "colSpan":
                        cellElement.ColumnSpan.Value = ValueParser.ParseInt(pair.Value);
                        break;
                    case "rowSpan":
                        cellElement.RowSpan.Value = ValueParser.ParseInt(pair.Value);
                        break;
                    case "enumerate":
                    case "increment":
                        cellElement.Enumerate.Value = ValueParser.ParseBool(pair.Value);
                        break;
                    default:
                        throw new InvalidDocumentElementPropertyException(cellElement, pair.Name, pair.Value);
                }
            }
        }

        private static void AssignListElementProperties(ListElement listElement, PropertyBag<string> propertyBag)
        {
            foreach (var pair in propertyBag.UnprocessedPairs())
            {
                switch (pair.Name)
                {
                    case "startIndex":
                        listElement.StartIndex.Value = ValueParser.ParseInt(pair.Value);
                        break;
                    case "indent":
                    case "indentation":
                        listElement.Indentation.Value = ValueParser.ParseFloat(pair.Value);
                        break;
                    case "symbol":
                    case "listSymbol":
                        listElement.ListSymbol.Value = pair.Value;
                        break;
                    case "preText":
                    case "preSymbolText":
                        listElement.PreSymbolText.Value = pair.Value;
                        break;
                    case "postText":
                    case "postSymbolText":
                        listElement.PostSymbolText.Value = pair.Value;
                        break;
                    case "enumerate":
                    case "enumeration":
                        listElement.Enumeration.Value = ValueParser.ParseBool(pair.Value);
                        break;
                    default:
                        throw new InvalidDocumentElementPropertyException(listElement, pair.Name, pair.Value);
                }
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
                    case "footer":
                        tableRowElement.IsFooter.Value = ValueParser.ParseBool(pair.Value);
                        break;
                    default:
                        throw new InvalidDocumentElementPropertyException(tableRowElement, pair.Name, pair.Value);
                }
            }
        }

        private static void AssignTableDataRowElementProperties(TableDataRowElement tableDataRowElement, PropertyBag<string> propertyBag)
        {
            foreach (var pair in propertyBag.UnprocessedPairs())
            {
                switch (pair.Name)
                {
                    case "rowHeight":
                        tableDataRowElement.RowHeight.Value = ValueParser.ParseFloat(pair.Value);
                        break;
                    case "dataSource":
                        tableDataRowElement.DataSource.Value = pair.Value;
                        break;
                    case "cellProperties":
                    case "columnProperties":
                        tableDataRowElement.ColumnCellProperties.Value = ValueParser.ParseStringArray(pair.Value);
                        break;
                    default:
                        throw new InvalidDocumentElementPropertyException(tableDataRowElement, pair.Name, pair.Value);
                }
            }
        }

        private static void AssignTableElementProperties(TableElement tableElement, PropertyBag<string> propertyBag)
        {
            foreach (var pair in propertyBag.UnprocessedPairs())
            {
                switch (pair.Name)
                {
                    case "width":
                    case "tableWidth":
                        tableElement.TableWidth.Value = ValueParser.ParseUnitValue(pair.Value);
                        break;
                    case "columnCount":
                        tableElement.ColumnCount.Value = ValueParser.ParseInt(pair.Value);
                        break;
                    case "columnWidths":
                        tableElement.ColumnWidths.Value = ValueParser.ParseUnitValueArray(pair.Value);
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
                    case "font":
                    case "fontName":
                        textElement.FontName.Value = pair.Value;
                        break;
                    case "horizontalAlignment":
                        textElement.HorizontalAlignment.Value = ValueParser.ParseHorizontalAlignment(pair.Value);
                        break;
                    case "verticalAlignment":
                        textElement.VerticalAlignment.Value = ValueParser.ParseVerticalAlignment(pair.Value);
                        break;
                    case "textAlign":
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
                        textElement.FormatProperties = ValueParser.ParseStringArray(pair.Value);
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
                    case "borders":
                        borderedElement.Borders.Value = ValueParser.ParseBorderInfo(pair.Value);
                        break;
                    case "borderTop":
                        borderedElement.TopBorder.Value = ValueParser.ParseBorderInfo(pair.Value);
                        break;
                    case "borderBottom":
                        borderedElement.BottomBorder.Value = ValueParser.ParseBorderInfo(pair.Value);
                        break;
                    case "borderLeft":
                        borderedElement.LeftBorder.Value = ValueParser.ParseBorderInfo(pair.Value);
                        break;
                    case "borderRight":
                        borderedElement.RightBorder.Value = ValueParser.ParseBorderInfo(pair.Value);
                        break;
                }
            }
        }


        private static void AssignRootDocumentElementProperties(RootDocumentElement rootDocumentElement, PropertyBag<string> propertyBag)
        {
            foreach (var (name, value) in propertyBag.UnprocessedPairs())
            {
                switch (name)
                {
                    case "style":
                        rootDocumentElement.StyleFile.Value = value;
                        break;
                    case "font":
                    case "documentFont":
                        rootDocumentElement.DocumentFont.Value = value;
                        break;
                    case "fontSize":
                        rootDocumentElement.DocumentFontSize.Value = ValueParser.ParseFloat(value);
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