using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Xml2Pdf.DocumentStructure;
using Xml2Pdf.DocumentStructure.Geometry;
using Xml2Pdf.Format;
using Xml2Pdf.Renderer.Interface;
using Xml2Pdf.Renderer.Mappers;

namespace Xml2Pdf.Renderer
{
    public class PdfDocumentRenderer : IDocumentRenderer
    {
        private Document _pdfDocument = null;
        private Dictionary<string, object> _objectPropertyMap;

        private readonly PdfFont _defaultFont;

        // TODO(Moravec): This should be configurable in XML.
        private const float DefaultFontSize = 10;
        private const float DefaultSmallFontSize = 6;


        public ValueFormatter ValueFormatter { get; }

        public PdfDocumentRenderer()
        {
            _objectPropertyMap = new Dictionary<string, object>();
            ValueFormatter = new ValueFormatter();
            _defaultFont = GetDefaultFont();
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

            RenderRootDocumentElement(rootDocumentElement);

            _pdfDocument.Close();

            return true;
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

            foreach (var childElement in rootElement.Children)
            {
                Debug.Assert(childElement is PageElement, "Invalid child element for RootElement");
                RenderDocumentElement(childElement, null, _pdfDocument);
            }

            // End the page. Move to RenderRootDocumentElement?
            _pdfDocument.Add(new AreaBreak());
        }

        private void RenderDocumentElement(DocumentElement element, DocumentElement parent, object pdfParentObject)
        {
            switch (element)
            {
                case PageElement pageElement:
                    RenderPageElement(pageElement, parent, pdfParentObject);
                    break;
                case ParagraphElement paragraphElement:
                    RenderParagraphElement(paragraphElement, parent, pdfParentObject);
                    break;
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
            if (!element.HasChildren)
            {
                paragraph.Add(RenderTextElement(element));
            }
            else if (element.IsEmpty())
            {
                foreach (var child in element.Children)
                {
                    paragraph.Add(RenderTextElement(child as TextElement));
                }
            }
            else
            {
                throw new NotImplementedException("Mix of raw text and <Text> elements is not implemented yet.");
            }

            if (pdfParentObject is Document doc)
                doc.Add(paragraph);
            else if (pdfParentObject is Cell cell)
                cell.Add(paragraph);
        }

        private Text RenderTextElement(TextElement element)
        {
            Text text = null;
            var textToRender = element.GetTextToRender(_objectPropertyMap, ValueFormatter);

            if (element.Superscript)
            {
                text = new Text(textToRender).SetFont(_defaultFont).SetTextRise(7).SetFontSize(DefaultFontSize);
            }
            else if (element.Subscript)
            {
                text = new Text(textToRender).SetFont(_defaultFont).SetTextRise(-7).SetFontSize(DefaultFontSize);
            }
            else
            {
                text = new Text(textToRender).SetFontSize(element.FontSize != 0 ? element.FontSize : DefaultFontSize);
            }

            // Borders.
            if (element.Borders != null)
            {
                text.SetBorder(element.Borders.ToITextBorder());
            }
            else
            {
                if (element.TopBorder != null)
                    text.SetBorderTop(element.TopBorder.ToITextBorder());
                if (element.BottomBorder != null)
                    text.SetBorderBottom(element.BottomBorder.ToITextBorder());
                if (element.LeftBorder != null)
                    text.SetBorderLeft(element.LeftBorder.ToITextBorder());
                if (element.RightBorder != null)
                    text.SetBorderRight(element.RightBorder.ToITextBorder());
            }

            text.SetHorizontalAlignment(element.HorizontalAlignment)
                .SetTextAlignment(element.TextAlignment)
                .SetFontColor(element.ForegroundColor)
                .SetBackgroundColor(element.BackgroundColor);

            if (element.Bold)
                text.SetBold();
            if (element.Italic)
                text.SetItalic();
            if (element.Underline)
                text.SetUnderline();

            return text;
        }
    }
}