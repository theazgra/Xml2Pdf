using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Xml2Pdf.DocumentStructure;
using Xml2Pdf.DocumentStructure.Geometry;
using Xml2Pdf.Exceptions;
using Xml2Pdf.Format;
using Xml2Pdf.Format.Formatters;
using Xml2Pdf.Renderer.Interface;
using Xml2Pdf.Utilities;

namespace Xml2Pdf.Renderer
{
    public class PdfDocumentRenderer : IDocumentRenderer
    {
        private const int FontPropertyId = 20;
        private const float ScriptFontCoefficient = 0.7f;

        private Document _pdfDocument = null;
        private readonly Dictionary<string, object> _objectPropertyMap;

        private Rectangle _effectivePageRectangle;
        private ElementStyle _style = new ElementStyle();

        private float _documentFontSize = 10;
        private PdfFont _documentFont;


        public ValueFormatter ValueFormatter { get; }

        public PdfDocumentRenderer()
        {
            _objectPropertyMap = new Dictionary<string, object>();
            ValueFormatter = new ValueFormatter();
            ValueFormatter.AddFormatter(new ToStringFormatter<object>());
        }

        private PdfFont GetDefaultFont() => PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA);


        private void LoadDataObjectToPropertyMap(object dataObject)
        {
            foreach (var property in dataObject.GetType().GetProperties())
            {
                _objectPropertyMap.Add(property.Name, property.GetValue(dataObject));
            }
        }

        private void InitializeDocumentProperties(RootDocumentElement element)
        {
            if (element.DocumentFont.IsInitialized)
            {
                if (!_style.CustomFonts.ContainsKey(element.DocumentFont.Value))
                {
                    throw new RenderException("DocumentFont wasn't specified as <CustomFont> in <Style> node.");
                }

                _documentFont = _style.CustomFonts[element.DocumentFont.Value];
            }
            else
            {
                _documentFont = GetDefaultFont();
            }

            _documentFontSize = element.DocumentFontSize.ValueOr(_documentFontSize);
        }

        public bool RenderDocument(RootDocumentElement rootDocumentElement, string savePath) =>
            RenderDocument(rootDocumentElement, savePath, null);

        public bool RenderDocument(RootDocumentElement rootDocumentElement, string savePath, object dataObject)
        {
            _style = rootDocumentElement.Style;
            if (dataObject != null)
            {
                LoadDataObjectToPropertyMap(dataObject);
            }

            InitializeDocumentProperties(rootDocumentElement);

            using var writer = new PdfWriter(savePath);
            var pdf = new PdfDocument(writer);

            var pageSize = rootDocumentElement.PageOrientation == PageOrientation.Portrait
                ? rootDocumentElement.PageSize
                : rootDocumentElement.PageSize.Rotate();


            _pdfDocument = new Document(pdf, pageSize);
            _effectivePageRectangle = _pdfDocument.GetPageEffectiveArea(pageSize);

            RenderRootDocumentElement(rootDocumentElement);

            _pdfDocument.Close();

            return true;
        }

        private void RenderDocumentElement(DocumentElement element, DocumentElement parent, object docParent, StyleWrapper style)
        {
            if (style == null)
                style = new StyleWrapper();
            switch (element)
            {
                case PageElement pageElement:
                    RenderPageElement(pageElement, parent, docParent);
                    break;
                case ParagraphElement paragraphElement:
                    RenderParagraphElement(paragraphElement, docParent, style);
                    break;
                case TableElement tableElement:
                    RenderTableElement(tableElement, docParent);
                    break;
                case TableDataRowElement tableDataRowElement:
                    RenderTableDataRowElement(tableDataRowElement, parent as TableElement, docParent as Table);
                    break;
                case TableRowElement tableRowElement:
                    RenderTableRowElement(tableRowElement, parent as TableElement, docParent as Table);
                    break;
                case TableCellElement tableCell:
                    RenderTableCell(tableCell, parent as TableRowElement, docParent as Table);
                    break;
                case ListElement listElement:
                    RenderListElement(listElement, parent, docParent);
                    break;
                case ListItemElement listItemElement:
                    RenderListItemElement(listItemElement, docParent as List);
                    break;
                case LineElement lineElement:
                    RenderLineElement(lineElement, docParent);
                    break;
                default:
                    ColorConsole.WriteLine(ConsoleColor.Red,
                        $"Missing branch in PdfDocumentRenderer::RenderDocumentElement() for {element.GetType().Name}");
                    break;
            }
        }

        private void RenderLineElement(LineElement lineElement, object docParent)
        {
            var emptyParagraph = new Paragraph();

            AddCombinedStylesToElement(emptyParagraph, _style.LineStyle, lineElement.BorderPropertiesToStyle());

            if (lineElement.Length.IsInitialized)
                emptyParagraph.SetWidth(lineElement.Length.Value);
            if (lineElement.Alignment.IsInitialized)
                emptyParagraph.SetHorizontalAlignment(lineElement.Alignment.Value);
            AddParagraphToParent(emptyParagraph, docParent);
        }

        /// <summary>
        /// Add combined styles to the iText pdf element.
        /// </summary>
        /// <param name="element">Pdf element.</param>
        /// <param name="styles">Styles, later styles override the previous ones.</param>
        private void AddCombinedStylesToElement<T>(AbstractElement<T> element, params StyleWrapper[] styles) where T : IElement
        {
            if (styles.Length == 0)
                return;

            // Find first not null.
            int index = 0;
            StyleWrapper style = null;
            for (index = 0; index < styles.Length; index++)
            {
                if (styles[index] != null)
                {
                    style = styles[index];
                    break;
                }
            }

            if (style == null)
                return;

            for (; index < styles.Length; index++)
            {
                style.CombineAndOverrideProperties(styles[index]);
            }

            element.AddStyle(style);
        }

        private void RenderListElement(ListElement element, DocumentElement parent, object pdfParentObject)
        {
            var list = element.Enumeration.ValueOr(false) ? new List(ListNumberingType.DECIMAL) : new List();


            if (element.Indentation.IsInitialized)
                list.SetSymbolIndent(element.Indentation.Value);
            if (element.ListSymbol.IsInitialized)
                list.SetListSymbol(element.ListSymbol.Value);
            if (element.StartIndex.IsInitialized)
                list.SetItemStartIndex(element.StartIndex.Value);
            if (element.PreSymbolText.IsInitialized)
                list.SetPreSymbolText(element.PreSymbolText.Value);
            if (element.PostSymbolText.IsInitialized)
                list.SetPostSymbolText(element.PostSymbolText.Value);

            foreach (var listChild in element.Children)
            {
                RenderListItemElement(listChild as ListItemElement, list);
            }

            if (pdfParentObject is Document document)
            {
                document.Add(list);
            }
            else
            {
                throw RenderException.WrongPdfParent(nameof(RenderListElement),
                    typeof(Document),
                    pdfParentObject.GetType());
            }
        }

        private void RenderListItemElement(ListItemElement element, List list)
        {
            var listItem = new ListItem(element.GetTextToRender(_objectPropertyMap, ValueFormatter));

            var style = element.TextPropertiesToStyle(_style.CustomFonts);
            style.CombineAndOverrideProperties(_style.ListItemStyle);
            if (element.FontName.IsInitialized && _style.CustomFonts.ContainsKey(element.FontName.Value))
            {
                style.SetProperty(FontPropertyId, _style.CustomFonts[element.FontName.Value]);
            }

            listItem.AddStyle(style);
            list.Add(listItem);
        }

        [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
        private void RenderRootDocumentElement(RootDocumentElement rootElement)
        {
            // Document margins.
            if (rootElement.CustomMargins != null)
            {
                if (rootElement.CustomMargins.AreComplete())
                {
                    _pdfDocument.SetMargins(rootElement.CustomMargins.Top.Value,
                        rootElement.CustomMargins.Right.Value,
                        rootElement.CustomMargins.Bottom.Value,
                        rootElement.CustomMargins.Left.Value);
                }
                else
                {
                    if (rootElement.CustomMargins.Top.HasValue)
                        _pdfDocument.SetTopMargin(rootElement.CustomMargins.Top.Value);
                    if (rootElement.CustomMargins.Bottom.HasValue)
                        _pdfDocument.SetBottomMargin(rootElement.CustomMargins.Bottom.Value);
                    if (rootElement.CustomMargins.Left.HasValue)
                        _pdfDocument.SetLeftMargin(rootElement.CustomMargins.Left.Value);
                    if (rootElement.CustomMargins.Right.HasValue)
                        _pdfDocument.SetRightMargin(rootElement.CustomMargins.Right.Value);
                }
            }

            for (int i = 0; i < rootElement.ChildrenCount; i++)
            {
                var childElement = rootElement.Children.ElementAt(i);
                Debug.Assert(childElement is PageElement, "Invalid child element for RootElement");
                RenderDocumentElement(childElement, null, _pdfDocument, null);

                if (i < (rootElement.ChildrenCount - 1))
                    _pdfDocument.Add(new AreaBreak());
            }
        }

        private void RenderTableCell(TableCellElement tableCell, TableRowElement parent, Table pdfParentObject)
        {
            Debug.Assert(parent != null, "DocumentElement parent is null.");
            Debug.Assert(pdfParentObject != null, "Pdf parent is null.");

            int rowSpan = tableCell.RowSpan.ValueOr(1);
            int colSpan = tableCell.ColumnSpan.ValueOr(1);

            Cell cell = new Cell(rowSpan, colSpan);

            StyleWrapper cellStyle = tableCell.TextPropertiesToStyle(_style.CustomFonts);
            if (_style.TableCellStyle != null)
                cellStyle.CombineAndOverrideProperties(_style.TableCellStyle);

            if (tableCell.HasChildren)
            {
                cellStyle.CombineAndOverrideProperties(((TextElement) tableCell.FirstChild).TextPropertiesToStyle(_style.CustomFonts));
            }

            if (parent.RowHeight.IsInitialized)
                cell.SetHeight(parent.RowHeight.Value);

            AddCombinedStylesToElement(cell, cellStyle);

            if (tableCell.HasChildren)
            {
                Debug.Assert(tableCell.ChildrenCount == 1);
                foreach (var cellChild in tableCell.Children)
                {
                    RenderDocumentElement(cellChild, tableCell, cell, cellStyle);
                }
            }
            else if (!tableCell.IsEmpty())
            {
                cell.Add(new Paragraph(RenderTextElement(tableCell, cellStyle)));
            }

            if (parent.IsHeader.ValueOr(false))
                pdfParentObject.AddHeaderCell(cell);
            else if (parent.IsFooter.ValueOr(false))
                pdfParentObject.AddFooterCell(cell);
            else
                pdfParentObject.AddCell(cell);
        }

        private void RenderTableRowElement(TableRowElement tableRowElement, TableElement parent, Table pdfTable)
        {
            Debug.Assert(parent != null, "DocumentElement parent is null.");
            Debug.Assert(pdfTable != null, "Pdf parent is null.");
            if (!tableRowElement.IsHeader.ValueOr(false) && !tableRowElement.IsFooter.ValueOr(false))
                pdfTable.StartNewRow();


            if (!tableRowElement.RowHeight.IsInitialized && parent.RowHeight.IsInitialized)
            {
                tableRowElement.RowHeight.Value = parent.RowHeight.Value;
            }

            foreach (var tableCell in tableRowElement.Children)
            {
                Debug.Assert(tableCell.GetType() == typeof(TableCellElement));
                RenderDocumentElement(tableCell, tableRowElement, pdfTable, null);
            }
        }

        private void RenderTableDataRowElement(TableDataRowElement element, TableElement parent, Table pdfTable)
        {
            Debug.Assert(parent != null, "DocumentElement parent is null.");
            Debug.Assert(pdfTable != null, "Pdf parent is null.");

            if (!element.RowHeight.IsInitialized && parent.RowHeight.IsInitialized)
            {
                element.RowHeight.Value = parent.RowHeight.Value;
            }

            // Check that data source properties are initialized.
            if (!element.DataSource.IsInitialized)
            {
                throw new RenderException("dataSource property must be set for TableDataRowElement.");
            }


            if (!(_objectPropertyMap[element.DataSource.Value] is IEnumerable<object> tableDataSource))
                throw new RenderException("DataSource must be convertible to IEnumerable<object>.");

            var tableDataSourceArray = tableDataSource as object[] ?? tableDataSource.ToArray();
            if (!tableDataSourceArray.Any())
                return;

            Type rowObjectType = tableDataSourceArray[0].GetType();

            PropertyInfo[] objectProperties;
            bool enumerationAsFirstColumn = false;
            if (element.HasChildren)
            {
                if (element.FirstChild is TableCellElement cell && cell.Enumerate.ValueOr(false))
                    enumerationAsFirstColumn = true;

                objectProperties = element.Children.OfType<TableCellElement>()
                    .Where(c => !string.IsNullOrEmpty(c.Property))
                    .Select(c => rowObjectType.GetProperty(c.Property))
                    .ToArray();
            }
            else
            {
                objectProperties = element.ColumnCellProperties.Value
                    .Select(p => rowObjectType.GetProperty(p))
                    .ToArray();
            }


            int rowIndex = 1;
            foreach (var rowObject in tableDataSourceArray)
            {
                if (element.HasChildren)
                {
                    pdfTable.StartNewRow();
                    if (enumerationAsFirstColumn)
                    {
                        TableCellElement cell = (TableCellElement) element.FirstChild;
                        cell.Text = rowIndex.ToString();
                        RenderTableCell(cell, element, pdfTable);
                        rowIndex++;
                    }

                    int offset = enumerationAsFirstColumn ? 1 : 0;
                    for (int cellIndex = 0; cellIndex < objectProperties.Length; cellIndex++)
                    {
                        TableCellElement cell = (TableCellElement) element.GetChildrenAtIndex(cellIndex + offset);
                        cell.Text = ValueFormatter.FormatValue(objectProperties[cellIndex].GetValue(rowObject));
                        RenderTableCell(cell, element, pdfTable);
                    }
                }
                else
                {
                    foreach (PropertyInfo cellProperty in objectProperties)
                    {
                        string text = ValueFormatter.FormatValue(cellProperty.GetValue(rowObject));
                        pdfTable.AddCell(new Cell().Add(new Paragraph(RenderTextElement(element,
                            element.TextPropertiesToStyle(_style.CustomFonts), text))));
                    }
                }
            }
        }

        private void RenderTableElement(TableElement element, object pdfParentObject)
        {
            Table table = new Table(element.GetColumnWidths(), element.LargeTable.ValueOr(false));

            if (_style.TableStyle != null)
                table.AddStyle(_style.TableStyle);

            table.SetWidth(element.TableWidth.ValueOr(UnitValue.CreatePercentValue(100.0f)));

            if (element.VerticalBorderSpacing.IsInitialized)
                table.SetVerticalBorderSpacing(element.VerticalBorderSpacing.Value);
            if (element.HorizontalBorderSpacing.IsInitialized)
                table.SetHorizontalBorderSpacing(element.HorizontalBorderSpacing.Value);

            foreach (var tableRow in element.Children)
            {
                RenderDocumentElement(tableRow, element, table, _style.TableStyle);
            }

            if (pdfParentObject is Document document)
            {
                document.Add(table);
            }
            else
            {
                throw RenderException.WrongPdfParent(nameof(RenderTableElement),
                    typeof(Document),
                    pdfParentObject.GetType());
            }
        }

        private void RenderPageElement(PageElement pageElement, DocumentElement parent, object pdfParentObject)
        {
            foreach (var childElement in pageElement.Children)
            {
                RenderDocumentElement(childElement, pageElement, pdfParentObject, new StyleWrapper());
            }
        }


        private void RenderParagraphElement(ParagraphElement element, object pdfParentObject, StyleWrapper style)
        {
            var paragraph = new Paragraph();

            if (_style.ParagraphStyle != null)
                style.CombineAndOverrideProperties(_style.ParagraphStyle);

            if (!element.HasChildren)
            {
                paragraph.Add(RenderTextElement(element, style));
            }
            else if (element.IsEmpty())
            {
                paragraph.Add(RenderTextElement(element.Children.ElementAt(0) as TextElement, style));

                for (int i = 1; i < element.ChildrenCount; i++)
                {
                    paragraph.Add(" ").Add(RenderTextElement(element.Children.ElementAt(i) as TextElement, style));
                }
            }
            else
            {
                throw new NotImplementedException("Mix of raw text and <Text> elements is not supported yet. " +
                                                  "Use only raw text or multiple <Text> elements.");
            }

            AddParagraphToParent(paragraph, pdfParentObject);
        }

        private void AddParagraphToParent(Paragraph paragraph, object pdfParent)
        {
            if (pdfParent is Document doc)
            {
                doc.Add(paragraph);
            }
            else if (pdfParent is Cell cell)
            {
                cell.Add(paragraph);
            }
            else
            {
                throw RenderException.WrongPdfParent(nameof(AddParagraphToParent),
                    new[]
                    {
                        typeof(Document),
                        typeof(Cell)
                    },
                    pdfParent.GetType());
            }
        }

        /// <summary>
        /// Get the custom font by its name or return document font.
        /// </summary>
        /// <param name="fontNameProperty">Font name property.</param>
        /// <returns>PDF font.</returns>
        private PdfFont GetFontByNameOrDefaultFont(ElementProperty<string> fontNameProperty)
        {
            if (fontNameProperty.IsInitialized && _style != null && _style.CustomFonts.ContainsKey(fontNameProperty.Value))
                return _style.CustomFonts[fontNameProperty.Value];
            return _documentFont;
        }

        private Text RenderTextElement(TextElement element, StyleWrapper style, string textToRender = null)
        {
            Text text;
            textToRender ??= element.GetTextToRender(_objectPropertyMap, ValueFormatter);
            style.CombineAndOverrideProperties(element.TextPropertiesToStyle(_style.CustomFonts));
            if (element.FontName.IsInitialized && _style.CustomFonts.ContainsKey(element.FontName.Value))
                style.SetFont(_style.CustomFonts[element.FontName.Value]);
            if (element.FontSize.IsInitialized)
                style.SetFontSize(element.FontSize.Value);

            // TODO(Moravec): This is hack to computer script size. FIX IT.
            float fontSize = element.FontSize.ValueOr(_documentFontSize);

            if (element.Superscript.ValueOr(false))
            {
                text = new Text(textToRender).SetTextRise(4).SetFontSize(fontSize * ScriptFontCoefficient);
            }
            else if (element.Subscript.ValueOr(false))
            {
                text = new Text(textToRender).SetTextRise(-4).SetFontSize(fontSize * ScriptFontCoefficient);
            }
            else
            {
                text = new Text(textToRender);
            }


            text.AddStyle(style);
            return text;
        }
    }
}