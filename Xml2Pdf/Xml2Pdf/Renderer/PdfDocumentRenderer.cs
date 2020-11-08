using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
using Xml2Pdf.Renderer.Mappers;
using Xml2Pdf.Utilities;

namespace Xml2Pdf.Renderer
{
    public class PdfDocumentRenderer : IDocumentRenderer
    {
        private Document _pdfDocument = null;
        private readonly Dictionary<string, object> _objectPropertyMap;

        private readonly PdfFont _defaultFont;
        private Rectangle _effectivePageRectangle;

        // TODO(Moravec): This should be configurable in XML.
        private const float DefaultFontSize = 10;
        private const float DefaultSmallFontSize = 6;


        public ValueFormatter ValueFormatter { get; }

        public PdfDocumentRenderer()
        {
            _objectPropertyMap = new Dictionary<string, object>();
            ValueFormatter = new ValueFormatter();
            _defaultFont = GetDefaultFont();
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

        public bool RenderDocument(RootDocumentElement rootDocumentElement, string savePath) =>
            RenderDocument(rootDocumentElement, savePath, null);

        public bool RenderDocument(RootDocumentElement rootDocumentElement, string savePath, object dataObject)
        {
            if (dataObject != null)
            {
                LoadDataObjectToPropertyMap(dataObject);
            }

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

        private void RenderDocumentElement(DocumentElement element, DocumentElement parent, object docParent)
        {
            switch (element)
            {
                case PageElement pageElement:
                    RenderPageElement(pageElement, parent, docParent);
                    break;
                case ParagraphElement paragraphElement:
                    RenderParagraphElement(paragraphElement, parent, docParent);
                    break;
                case TableElement tableElement:
                    RenderTableElement(tableElement, parent, docParent);
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
                    RenderListItemElement(listItemElement, docParent as iText.Layout.Element.List);
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
            SetBorderedElementProperties(emptyParagraph, lineElement);
            if (lineElement.Length.IsInitialized)
                emptyParagraph.SetWidth(lineElement.Length.Value);
            if (lineElement.Alignment.IsInitialized)
                emptyParagraph.SetHorizontalAlignment(lineElement.Alignment.Value);
            AddParagraphToParent(emptyParagraph, docParent);
        }

        private void RenderListElement(ListElement element, DocumentElement parent, object pdfParentObject)
        {
            iText.Layout.Element.List list;
            if (element.Enumeration.IsInitialized && element.Enumeration.Value)
                list = new List(ListNumberingType.DECIMAL);
            else
                list = new List();


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

        private void RenderListItemElement(ListItemElement listChild, List list)
        {
            var listItem = new ListItem(listChild.GetTextToRender(_objectPropertyMap, ValueFormatter));
            listItem.SetFont(_defaultFont);
            SetTextElementProperties(listItem, listChild);
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
                RenderDocumentElement(childElement, null, _pdfDocument);

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
            SetTextElementProperties(cell, tableCell);
            if (tableCell.HasChildren)
                SetTextElementProperties(cell, (tableCell.FirstChild as TextElement));

            if (parent.RowHeight.IsInitialized)
                cell.SetHeight(parent.RowHeight.Value);

            if (tableCell.HasChildren)
            {
                Debug.Assert(tableCell.ChildrenCount == 1);
                foreach (var cellChild in tableCell.Children)
                {
                    // TODO(Moravec): We have to set text properties on cell.
                    RenderDocumentElement(cellChild, tableCell, cell);
                }
            }
            else if (!tableCell.IsEmpty())
            {
                cell.Add(new Paragraph(RenderTextElement(tableCell)));
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
                RenderDocumentElement(tableCell, tableRowElement, pdfTable);
            }
        }

        private void RenderTableElement(TableElement tableElement, DocumentElement parent, object pdfParentObject)
        {
            Table table = new Table(tableElement.GetColumnWidths(),
                                    (tableElement.LargeTable.IsInitialized && tableElement.LargeTable.Value));

            // TODO(Moravec): Customizable.
            table.SetWidth(new UnitValue(UnitValue.PERCENT, 100.0f));

            if (tableElement.VerticalBorderSpacing.IsInitialized)
                table.SetVerticalBorderSpacing(tableElement.VerticalBorderSpacing.Value);
            if (tableElement.HorizontalBorderSpacing.IsInitialized)
                table.SetHorizontalBorderSpacing(tableElement.HorizontalBorderSpacing.Value);

            foreach (var tableRow in tableElement.Children)
            {
                Debug.Assert(tableRow.GetType() == typeof(TableRowElement),
                             "Invalid children of TableElement. Invalid type.");
                RenderDocumentElement(tableRow, tableElement, table);
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

        private void RenderPageElement(PageElement pageElement,
                                       DocumentElement parent,
                                       object pdfParentObject)
        {
            foreach (var childElement in pageElement.Children)
            {
                RenderDocumentElement(childElement, pageElement, pdfParentObject);
            }
        }


        private void RenderParagraphElement(ParagraphElement element,
                                            DocumentElement parent,
                                            object pdfParentObject)
        {
            var paragraph = new Paragraph();
            SetTextElementProperties(paragraph, element);
            if (!element.HasChildren)
            {
                paragraph.Add(RenderTextElement(element));
            }
            else if (element.IsEmpty())
            {
                paragraph.Add(RenderTextElement(element.Children.ElementAt(0) as TextElement));

                for (int i = 1; i < element.ChildrenCount; i++)
                {
                    paragraph.Add(" ").Add(RenderTextElement(element.Children.ElementAt(i) as TextElement));
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

        private Text RenderTextElement(TextElement element, string textToRender = null)
        {
            Text text = null;
            textToRender ??= element.GetTextToRender(_objectPropertyMap, ValueFormatter);

            if (element.Superscript.IsInitialized && element.Superscript.Value)
            {
                text = new Text(textToRender).SetFont(_defaultFont).SetTextRise(7).SetFontSize(DefaultFontSize);
            }
            else if (element.Subscript.IsInitialized && element.Subscript.Value)
            {
                text = new Text(textToRender).SetFont(_defaultFont).SetTextRise(-7).SetFontSize(DefaultFontSize);
            }
            else
            {
                float fontSize = element.FontSize.IsInitialized ? element.FontSize.Value : DefaultFontSize;
                text = new Text(textToRender).SetFont(_defaultFont).SetFontSize(fontSize);
            }

            // Borders.
            if (element.Borders.IsInitialized)
            {
                text.SetBorder(element.Borders.Value.ToITextBorder());
            }
            else
            {
                if (element.TopBorder.IsInitialized)
                    text.SetBorderTop(element.TopBorder.Value.ToITextBorder());
                if (element.BottomBorder.IsInitialized)
                    text.SetBorderBottom(element.BottomBorder.Value.ToITextBorder());
                if (element.LeftBorder.IsInitialized)
                    text.SetBorderLeft(element.LeftBorder.Value.ToITextBorder());
                if (element.RightBorder.IsInitialized)
                    text.SetBorderRight(element.RightBorder.Value.ToITextBorder());
            }

            if (element.HorizontalAlignment.IsInitialized)
                text.SetHorizontalAlignment(element.HorizontalAlignment.Value);
            if (element.TextAlignment.IsInitialized)
                text.SetTextAlignment(element.TextAlignment.Value);
            if (element.ForegroundColor.IsInitialized)
                text.SetFontColor(element.ForegroundColor.Value);
            if (element.BackgroundColor.IsInitialized)
                text.SetBackgroundColor(element.BackgroundColor.Value);

            if (element.Bold.IsInitialized && element.Bold.Value)
                text.SetBold();
            if (element.Italic.IsInitialized && element.Italic.Value)
                text.SetItalic();
            if (element.Underline.IsInitialized && element.Underline.IsInitialized)
                text.SetUnderline();

            return text;
        }

        private void SetBorderedElementProperties<T>(ElementPropertyContainer<T> container,
                                                     BorderedDocumentElement element) where T : IElement
        {
            // Borders.
            if (element.Borders.IsInitialized)
            {
                container.SetBorder(element.Borders.Value.ToITextBorder());
            }
            else
            {
                if (element.TopBorder.IsInitialized)
                    container.SetBorderTop(element.TopBorder.Value.ToITextBorder());
                if (element.BottomBorder.IsInitialized)
                    container.SetBorderBottom(element.BottomBorder.Value.ToITextBorder());
                if (element.LeftBorder.IsInitialized)
                    container.SetBorderLeft(element.LeftBorder.Value.ToITextBorder());
                if (element.RightBorder.IsInitialized)
                    container.SetBorderRight(element.RightBorder.Value.ToITextBorder());
            }
        }


        private void SetTextElementProperties<T>(ElementPropertyContainer<T> container,
                                                 TextElement element) where T : IElement
        {
            Debug.Assert(element != null);
            SetBorderedElementProperties(container, element);

            float fontSize = element.FontSize.IsInitialized ? element.FontSize.Value : DefaultFontSize;
            container.SetFontSize(fontSize);


            if (element.HorizontalAlignment.IsInitialized)
                container.SetHorizontalAlignment(element.HorizontalAlignment.Value);
            if (element.TextAlignment.IsInitialized)
                container.SetTextAlignment(element.TextAlignment.Value);
            if (element.ForegroundColor.IsInitialized)
                container.SetFontColor(element.ForegroundColor.Value);
            if (element.BackgroundColor.IsInitialized)
                container.SetBackgroundColor(element.BackgroundColor.Value);

            if (element.Bold.IsInitialized && element.Bold.Value)
                container.SetBold();
            if (element.Italic.IsInitialized && element.Italic.Value)
                container.SetItalic();
            if (element.Underline.IsInitialized && element.Underline.IsInitialized)
                container.SetUnderline();
        }
    }
}