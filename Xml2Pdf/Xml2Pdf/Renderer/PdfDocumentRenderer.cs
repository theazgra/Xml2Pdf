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

            if (pdfParentObject is Document doc)
                doc.Add(paragraph);
            else if (pdfParentObject is Cell cell)
                cell.Add(paragraph);
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
                text = new Text(textToRender).SetFontSize(fontSize);
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
    }
}