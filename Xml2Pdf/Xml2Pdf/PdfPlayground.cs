using System;
using System.IO;
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

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

            // Closing and saving.
            document.Close();
        }
    }
}