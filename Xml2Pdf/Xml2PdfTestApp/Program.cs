using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
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
            // return;
            bool dump = false;
            bool render = false;

            foreach (string arg in args)
            {
                switch (arg)
                {
                    case "-d":
                        dump = true;
                        break;
                    case "-r":
                        render = true;
                        break;
                }
            }

            string filePath = @"D:\codes\Xml2Pdf\Xml2Pdf\Xml2PdfTestApp\Templates\Test1.xml";

            XmlDocumentTemplateParser parser = new XmlDocumentTemplateParser();
            var doc = parser.ParseTemplateFile(filePath);

            if (dump)
                Console.WriteLine(doc.DumpDocumentTree());

            if (render)
            {
                const string renderTarget = "D:\\tmp\\rendered.pdf";
                var renderer = new PdfDocumentRenderer();
                renderer.ValueFormatter.AddFormatter(new ToStringFormatter<object>());
                renderer.RenderDocument(doc, renderTarget);
                Console.WriteLine("Document is rendered to {0}", renderTarget);
            }
        }
    }
}