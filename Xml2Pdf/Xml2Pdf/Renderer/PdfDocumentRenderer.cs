using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using iText.Forms;
using iText.Forms.Fields;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Xml2Pdf.DocumentStructure;
using Xml2Pdf.DocumentStructure.Form;
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
        private static readonly PdfFont DefaultFont = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA);

        private const int FontPropertyId = 20;
        private const float ScriptFontCoefficient = 0.7f;

        private Document _pdfDocument = null;
        private readonly Dictionary<string, object> _objectPropertyMap;

        // private Rectangle _effectivePageRectangle;
        private ElementStyle _style = new ElementStyle();

        private PdfFont _docFont;
        private float _docFontSize = 10;


        public ValueFormatter ValueFormatter { get; }

        public PdfDocumentRenderer()
        {
            _objectPropertyMap = new Dictionary<string, object>();
            ValueFormatter = new ValueFormatter();
            ValueFormatter.AddFormatter(new ToStringFormatter<object>());
        }

        private T CheckedReadProperty<T>(string name)
        {
            if (!_objectPropertyMap.ContainsKey(name))
                throw new ArgumentException($"Property with name {name} wasn't found in the data object.");
            object value = _objectPropertyMap[name];

            if (value == null)
                throw new ArgumentException($"Property with name {name} is null.");

            var tType = typeof(T);
            var valueType = value.GetType();

            if (valueType != tType)
            {
                throw new ArgumentException($"Property with name {name} has invalid type. " +
                                            $"Requested type: '{tType.Name}'. Read type: '{valueType.Name}'");
            }

            return (T) value;
        }


        /// <summary>
        /// Load all object properties to lookup dictionary with its values.
        /// </summary>
        /// <param name="dataObject">Object which properties to load.</param>
        private void LoadDataObjectToPropertyMap(object dataObject)
        {
            if (dataObject == null)
                return;
            foreach (var property in dataObject.GetType().GetProperties())
            {
                _objectPropertyMap.Add(property.Name, property.GetValue(dataObject));
            }
        }

        /// <summary>
        /// Initialize document properties, which are used for rendering.
        /// </summary>
        /// <param name="element">Root element.</param>
        /// <exception cref="RenderException">is thrown if document font wasn't specified.</exception>
        private void InitializeDocumentProperties(RootDocumentElement element)
        {
            if (element.DocumentFont.IsInitialized)
            {
                if (!_style.CustomFonts.ContainsKey(element.DocumentFont.Value))
                {
                    throw new RenderException("DocumentFont wasn't specified as <CustomFont> in <Style> node.");
                }

                _docFont = _style.CustomFonts[element.DocumentFont.Value];
            }
            else
            {
                _docFont = DefaultFont;
            }

            _docFontSize = element.DocumentFontSize.ValueOr(_docFontSize);
        }

        public bool RenderDocument(RootDocumentElement rootDocumentElement, string savePath) =>
            RenderDocument(rootDocumentElement, savePath, null);

        public bool RenderDocument(RootDocumentElement rootDocumentElement, string savePath, object dataObject)
        {
            _style = rootDocumentElement.Style;
            LoadDataObjectToPropertyMap(dataObject);
            InitializeDocumentProperties(rootDocumentElement);

            using var writer = new PdfWriter(savePath);
            var pdf = new PdfDocument(writer);

            var pageSize = rootDocumentElement.PageOrientation == PageOrientation.Portrait
                ? rootDocumentElement.PageSize
                : rootDocumentElement.PageSize.Rotate();


            _pdfDocument = new Document(pdf, pageSize);
            // _effectivePageRectangle = _pdfDocument.GetPageEffectiveArea(pageSize);

            RenderRootDocumentElement(rootDocumentElement);

            _pdfDocument.Close();

            return true;
        }

        /// <summary>
        /// Get default font style with document font and document font size.
        /// </summary>
        /// <returns>Default page style.</returns>
        private StyleWrapper GetDefaultPageStyle()
        {
            StyleWrapper defaultStyle = new StyleWrapper();
            defaultStyle.SetFont(_docFont);
            defaultStyle.SetFontSize(_docFontSize);
            return defaultStyle;
        }

        [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
        private void RenderRootDocumentElement(RootDocumentElement rootElement)
        {
            // Document margins.
            if (rootElement.Margins.IsInitialized)
            {
                if (rootElement.Margins.Value.AreComplete())
                {
                    _pdfDocument.SetMargins(rootElement.Margins.Value.Top.Value,
                                            rootElement.Margins.Value.Right.Value,
                                            rootElement.Margins.Value.Bottom.Value,
                                            rootElement.Margins.Value.Left.Value);
                }
                else
                {
                    if (rootElement.Margins.Value.Top.HasValue)
                        _pdfDocument.SetTopMargin(rootElement.Margins.Value.Top.Value);
                    if (rootElement.Margins.Value.Bottom.HasValue)
                        _pdfDocument.SetBottomMargin(rootElement.Margins.Value.Bottom.Value);
                    if (rootElement.Margins.Value.Left.HasValue)
                        _pdfDocument.SetLeftMargin(rootElement.Margins.Value.Left.Value);
                    if (rootElement.Margins.Value.Right.HasValue)
                        _pdfDocument.SetRightMargin(rootElement.Margins.Value.Right.Value);
                }
            }

            for (int i = 0; i < rootElement.ChildrenCount; i++)
            {
                var childElement = rootElement.GetChildrenAtIndex(i);
                var defaultStyle = GetDefaultPageStyle();

                RenderDocumentElement(childElement, null, _pdfDocument, defaultStyle);

                if (i < (rootElement.ChildrenCount - 1))
                    _pdfDocument.Add(new AreaBreak());
            }
        }


        private void RenderDocumentElement(DocumentElement element,
                                           DocumentElement parent,
                                           object pdfParent,
                                           StyleWrapper inheritedStyle)
        {
            Debug.Assert(inheritedStyle != null, "Inherited style should always be set.");

            switch (element)
            {
                case PageElement pageElement:
                    RenderPageElement(pageElement, parent, pdfParent, inheritedStyle);
                    break;
                case ParagraphElement paragraphElement:
                    RenderParagraphElement(paragraphElement, parent, pdfParent, inheritedStyle);
                    break;
                case LineElement lineElement:
                    RenderLineElement(lineElement, pdfParent);
                    break;
                case ListElement listElement:
                    RenderListElement(listElement, parent, pdfParent, inheritedStyle);
                    break;
                case ListItemElement listItemElement:
                    RenderListItemElement(listItemElement, pdfParent as List, inheritedStyle);
                    break;
                case TableElement tableElement:
                    RenderTableElement(tableElement, pdfParent, inheritedStyle);
                    break;
                case TableDataRowElement tableDataRowElement:
                    RenderTableDataRowElement(tableDataRowElement, parent as TableElement, pdfParent as Table, inheritedStyle);
                    break;
                case TableRowElement tableRowElement:
                    RenderTableRowElement(tableRowElement, parent as TableElement, pdfParent as Table, inheritedStyle);
                    break;
                case TableCellElement tableCell:
                    RenderTableCell(tableCell, parent as TableRowElement, pdfParent as Table, inheritedStyle);
                    break;
                case ImageElement imageElement:
                    RenderImageElement(imageElement, parent, pdfParent, inheritedStyle);
                    break;
                case SpacerElement spacerElement:
                    RenderSpacerElement(spacerElement, parent, pdfParent, inheritedStyle);
                    break;
                case TextFieldElement textFieldElement:
                    RenderTextFieldElement(textFieldElement, parent, pdfParent, inheritedStyle);
                    break;
                default:
                    ColorConsole.WriteLine(ConsoleColor.Red,
                                           $"Missing branch in PdfDocumentRenderer::RenderDocumentElement() for {element.GetType().Name}");
                    break;
            }
        }


        private void RenderImageElement(ImageElement element, DocumentElement parent, object pdfParent, StyleWrapper inheritedStyle)
        {
            ImageData imgData = null;
            if (element.Path.IsInitialized)
            {
                imgData = ImageDataFactory.Create(element.Path.Value);
            }
            else if (element.SourceProperty.IsInitialized)
            {
                byte[] data = CheckedReadProperty<byte[]>(element.SourceProperty.Value);
                imgData = ImageDataFactory.Create(data);
            }

            Image image = new Image(imgData);
            image.AddStyle(inheritedStyle);

            if (element.FixedPosition.IsInitialized)
            {
                image.SetFixedPosition(element.FixedPosition.Value.X, element.FixedPosition.Value.Y);
                image.SetWidth(element.FixedPosition.Value.Width);
            }

            image.Scale(element.HorizontalScaling.ValueOr(1.0f), element.VerticalScaling.ValueOr(1.0f));

            if (pdfParent is Document pdfDoc)
                pdfDoc.Add(image);
            else if (pdfParent is Cell cell)
                cell.Add(image);
            else if (pdfParent is Paragraph p)
                p.Add(image);
        }

        private void RenderPageElement(PageElement element, DocumentElement parent, object pdfParent, StyleWrapper inheritedStyle)
        {
            foreach (var childElement in element.Children)
            {
                RenderDocumentElement(childElement, element, pdfParent, inheritedStyle);
            }
        }

        private void RenderParagraphElement(ParagraphElement element,
                                            DocumentElement parent,
                                            object pdfParent,
                                            StyleWrapper inheritedStyle)
        {
            var paragraph = new Paragraph();

            StyleWrapper paragraphStyle = inheritedStyle.CombineStyles(_style.ParagraphStyle)
                                                        .CombineStyles(element.GetElementStyle(_style.CustomFonts));

            paragraph.AddStyle(paragraphStyle);

            if (!element.HasChildren)
            {
                paragraph.Add(RenderTextElement(element, paragraphStyle));
            }
            else if (element.IsEmpty())
            {
                paragraph.Add(RenderTextElement(element.Children.ElementAt(0) as TextElement, paragraphStyle));

                for (int i = 1; i < element.ChildrenCount; i++)
                {
                    paragraph.Add(" ").Add(RenderTextElement(element.Children.ElementAt(i) as TextElement, paragraphStyle));
                }
            }
            else
            {
                throw new NotImplementedException("Mix of raw text and <Text> elements is not supported yet. " +
                                                  "Use only raw text or multiple <Text> elements.");
            }

            AddParagraphToParent(paragraph, pdfParent);
        }

        private Text RenderTextElement(TextElement element, StyleWrapper inheritedStyle, string rawText = null)
        {
            Text text;
            rawText ??= element.GetTextToRender(_objectPropertyMap, ValueFormatter);

            StyleWrapper textStyle = inheritedStyle.CombineStyles(element.GetElementStyle(_style.CustomFonts));


            if (element.Superscript.ValueOr(false))
            {
                text = new Text(rawText).SetTextRise(4).SetFontSize(textStyle.GetFontSize() * ScriptFontCoefficient);
            }
            else if (element.Subscript.ValueOr(false))
            {
                text = new Text(rawText).SetTextRise(-4).SetFontSize(textStyle.GetFontSize() * ScriptFontCoefficient);
            }
            else
            {
                text = new Text(rawText);
            }

            text.AddStyle(textStyle);
            return text;
        }

        private void RenderLineElement(LineElement lineElement, object docParent)
        {
            var emptyParagraph = new Paragraph();

            StyleWrapper lineStyle = _style.LineStyle != null
                ? _style.LineStyle.CombineStyles(lineElement.GetElementStyle(_style.CustomFonts))
                : lineElement.GetElementStyle(_style.CustomFonts);


            if (lineElement.Length.IsInitialized)
                lineStyle.SetWidth(lineElement.Length.Value);
            if (lineElement.Alignment.IsInitialized)
                lineStyle.SetHorizontalAlignment(lineElement.Alignment.Value);

            emptyParagraph.AddStyle(lineStyle);
            AddParagraphToParent(emptyParagraph, docParent);
        }

        private void RenderListElement(ListElement element, DocumentElement parent, object pdfParentObject, StyleWrapper inheritedStyle)
        {
            var list = element.Enumeration.ValueOr(false) ? new List(ListNumberingType.DECIMAL) : new List();

            StyleWrapper listStyle = inheritedStyle.CombineStyles(element.GetElementStyle(_style.CustomFonts));

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
                RenderDocumentElement(listChild, element, list, listStyle);
            }

            if (pdfParentObject is Document document)
            {
                document.Add(list);
            }
            else
            {
                throw RenderException.WrongPdfParent(nameof(RenderListElement), typeof(Document), pdfParentObject.GetType());
            }
        }

        private void RenderListItemElement(ListItemElement element, List list, StyleWrapper inheritedStyle)
        {
            var listItem = new ListItem(element.GetTextToRender(_objectPropertyMap, ValueFormatter));

            StyleWrapper listItemStyle = inheritedStyle.CombineStyles(_style.ListItemStyle)
                                                       .CombineStyles(element.GetElementStyle(_style.CustomFonts));


            listItem.AddStyle(listItemStyle);
            list.Add(listItem);
        }

        private void RenderTableElement(TableElement element, object pdfParent, StyleWrapper inheritedStyle)
        {
            Table table = new Table(element.GetColumnWidths(), element.LargeTable.ValueOr(false));


            var tableStyle = inheritedStyle.CombineStyles(_style.TableStyle)
                                           .CombineStyles(element.GetElementStyle(_style.CustomFonts));

            table.SetWidth(element.TableWidth.ValueOr(UnitValue.CreatePercentValue(100.0f)));

            if (element.VerticalBorderSpacing.IsInitialized)
                table.SetVerticalBorderSpacing(element.VerticalBorderSpacing.Value);
            if (element.HorizontalBorderSpacing.IsInitialized)
                table.SetHorizontalBorderSpacing(element.HorizontalBorderSpacing.Value);

            table.AddStyle(tableStyle);

            foreach (var tableRow in element.Children)
            {
                RenderDocumentElement(tableRow, element, table, tableStyle);
            }

            if (pdfParent is Document document)
            {
                document.Add(table);
            }
            else
            {
                throw RenderException.WrongPdfParent(nameof(RenderTableElement),
                                                     typeof(Document),
                                                     pdfParent.GetType());
            }
        }

        private void RenderTableRowElement(TableRowElement tableRowElement,
                                           TableElement parent,
                                           Table pdfTable,
                                           StyleWrapper inheritedStyle)
        {
            Debug.Assert(parent != null, "DocumentElement parent is null.");
            Debug.Assert(pdfTable != null, "Pdf parent is null.");

            if (!tableRowElement.IsHeader.ValueOr(false) && !tableRowElement.IsFooter.ValueOr(false))
                pdfTable.StartNewRow();

            var tableRowStyle = inheritedStyle.CombineStyles(tableRowElement.GetElementStyle(_style.CustomFonts));

            if (!tableRowElement.RowHeight.IsInitialized && parent.RowHeight.IsInitialized)
            {
                tableRowElement.RowHeight.Value = parent.RowHeight.Value;
                tableRowStyle.SetHeight(parent.RowHeight.Value);
            }

            foreach (var tableCell in tableRowElement.Children)
            {
                Debug.Assert(tableCell.GetType() == typeof(TableCellElement));
                RenderDocumentElement(tableCell, tableRowElement, pdfTable, tableRowStyle);
            }
        }

        private void RenderTableCell(TableCellElement tableCell, TableRowElement parent, Table pdfParentObject, StyleWrapper inheritedStyle)
        {
            Debug.Assert(parent != null, "DocumentElement parent is null.");
            Debug.Assert(pdfParentObject != null, "Pdf parent is null.");

            int rowSpan = tableCell.RowSpan.ValueOr(1);
            int colSpan = tableCell.ColumnSpan.ValueOr(1);

            Cell cell = new Cell(rowSpan, colSpan);

            var cellStyle = inheritedStyle
                            .CombineStyles(_style.TableCellStyle)
                            .CombineStyles(tableCell.GetElementStyle(_style.CustomFonts));


            if (tableCell.HasChildren)
            {
                cellStyle = cellStyle.CombineStyles(((TextElement) tableCell.FirstChild).GetElementStyle(_style.CustomFonts));
            }

            if (parent.RowHeight.IsInitialized)
                cell.SetHeight(parent.RowHeight.Value);

            cell.AddStyle(cellStyle);

            if (tableCell.HasChildren)
            {
                Debug.Assert(tableCell.ChildrenCount == 1);
                foreach (var cellChild in tableCell.Children)
                {
                    RenderDocumentElement(cellChild, tableCell, cell, cellStyle.RemoveBorderProperties());
                }
            }
            else if (!tableCell.IsEmpty())
            {
                cell.Add(new Paragraph(RenderTextElement(tableCell, cellStyle.RemoveBorderProperties())));
            }

            if (parent.IsHeader.ValueOr(false))
                pdfParentObject.AddHeaderCell(cell);
            else if (parent.IsFooter.ValueOr(false))
                pdfParentObject.AddFooterCell(cell);
            else
                pdfParentObject.AddCell(cell);
        }


        private void RenderTableDataRowElement(TableDataRowElement element,
                                               TableElement parent,
                                               Table pdfTable,
                                               StyleWrapper inheritedStyle)
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

            var rowStyle = inheritedStyle.CombineStyles(element.GetElementStyle(_style.CustomFonts));
            // TODO(Moravec): Finish down here.

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
                        RenderTableCell(cell, element, pdfTable, rowStyle);
                        rowIndex++;
                    }

                    int offset = enumerationAsFirstColumn ? 1 : 0;
                    for (int cellIndex = 0; cellIndex < objectProperties.Length; cellIndex++)
                    {
                        TableCellElement cell = (TableCellElement) element.GetChildrenAtIndex(cellIndex + offset);
                        cell.Text = ValueFormatter.FormatValue(objectProperties[cellIndex].GetValue(rowObject));
                        RenderTableCell(cell, element, pdfTable, rowStyle);
                    }
                }
                else
                {
                    foreach (PropertyInfo cellProperty in objectProperties)
                    {
                        TableCellElement cell = new TableCellElement
                        {
                            Text = ValueFormatter.FormatValue(cellProperty.GetValue(rowObject))
                        };
                        RenderTableCell(cell, element, pdfTable, rowStyle);
                    }
                }
            }
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
                var possibleTypes = new[]
                {
                    typeof(Document), typeof(Cell)
                };
                throw RenderException.WrongPdfParent(nameof(AddParagraphToParent), possibleTypes, pdfParent.GetType());
            }
        }

        private void RenderSpacerElement(SpacerElement spacerElement, DocumentElement parent, object pdfParent, StyleWrapper inheritedStyle)
        {
            var spacerParagraph = new Paragraph();
            spacerParagraph.AddStyle(spacerElement.GetElementStyle(_style.CustomFonts));
            AddParagraphToParent(spacerParagraph, pdfParent);
        }

        private PdfAcroForm GetDocumentForm() => PdfAcroForm.GetAcroForm(_pdfDocument.GetPdfDocument(), true);

        private void RenderTextFieldElement(TextFieldElement element,
                                            DocumentElement parent,
                                            object pdfParent,
                                            StyleWrapper inheritedStyle)
        {
            // PdfTextFormField textField = element.IsMultiline.ValueOr(false)
            //     ? PdfFormField.CreateMultilineText(_pdfDocument.GetPdfDocument(),
            //                                        element.Rectangle.Value,
            //                                        element.Name.ValueOr(string.Empty),
            //                                        element.Value.ValueOr(string.Empty))
            //     : PdfFormField.CreateText(_pdfDocument.GetPdfDocument(),
            //                               element.Rectangle.Value,
            //                               element.Name.ValueOr(string.Empty),
            //                               element.Value.ValueOr(string.Empty));
            //
            // // TODO(Moravec): Add custom properties to FormElement.
            // GetDocumentForm().AddField(textField);
        }
    }
}