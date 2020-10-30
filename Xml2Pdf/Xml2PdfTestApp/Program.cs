using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Serialization;
using Xml2Pdf;
using Xml2Pdf.DocumentStructure;
using Xml2Pdf.Format.Formatters;
using Xml2Pdf.Parser.Xml;
using Xml2Pdf.Renderer;

namespace Xml2PdfTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // PdfPlayground.Play();
            
            bool dump = args.Length > 0 && args[0] == "-d";
            // PdfPlayground.Play();
            string filePath = "..\\..\\..\\Templates\\Test1.xml";
            XmlDocumentTemplateParser parser = new XmlDocumentTemplateParser();
            var doc = parser.ParseTemplateFile(filePath);
            
            if (dump)
                Console.WriteLine(doc.DumpDocumentTree());
            
            var renderer = new PdfDocumentRenderer();
            renderer.ValueFormatter.AddFormatter(new ToStringFormatter<object>());
            renderer.RenderDocument(doc, "D:\\tmp\\rendered.pdf");
        }
    }
}