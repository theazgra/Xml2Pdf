using System;
using System.IO;
using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Org.BouncyCastle.Crypto.Generators;
using Xml2Pdf.DocumentStructure.Geometry;
using Border = iText.Layout.Borders.Border;

namespace Xml2Pdf
{
    public class PdfPlayground
    {
        public static void Play()
        {
            const string testFile = @"D:\tmp\test.pdf";
            if (File.Exists(testFile))
                File.Delete(testFile);

            // Setup
            using var writer = new PdfWriter(testFile);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);
            //document.SetMargins();
            // TODO(Moravec): Pdf size and landscape.
            //new Document(pdf, PageSize.A4.Rotate());

            // Actual document writing

            var customFont = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.TIMES_ROMAN);
            var paragraph = new Paragraph("This is paragraph text")
                .SetFont(customFont)
                .SetBold()
                .SetItalic()
                .SetHorizontalAlignment(HorizontalAlignment.CENTER)
                .SetFontColor(ColorConstants.RED);

            //var b = new SolidBorder

            document.Add(paragraph);


            List pdfList = new List(ListNumberingType.DECIMAL)
                .SetSymbolIndent(12)
                .SetFont(customFont);


            pdfList.Add(new ListItem("Never gonna give you up"))
                .Add(new ListItem("Never gonna let you down"))
                .Add(new ListItem("Never gonna run around and desert you"))
                .Add(new ListItem("Never gonna make you cry"))
                .Add(new ListItem("Never gonna say goodbye"))
                .Add(new ListItem("Never gonna tell a lie and hurt you"));

            document.Add(new Paragraph("Hello World!"));

            document.Add(pdfList);

            document.Add(new AreaBreak());

            document.Add(new Paragraph("This text is on second page."));

            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

            document.Add(new Paragraph("This text is on third page."));

            var t = new Table(new float[] {2, 3, 5, 8}, false);
            var c = new Cell(4, 5);

            document.Add(t);

            var i = new Image(ImageDataFactory.Create(new byte[]{25,64,45}), 5.0f, 5.0f, 6.0f);
            //i.Scale()
            
            document.Add(i);

            // Closing and saving.
            document.Close();
        }
    }
}