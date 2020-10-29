using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Serialization;
using Xml2Pdf;
using Xml2Pdf.DocumentStructure;
using Xml2Pdf.Parser.Xml;

namespace Xml2PdfTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // PdfPlayground.Play();
            string filePath = "..\\..\\..\\Templates\\Test1.xml";
            XmlDocumentTemplateParser parser = new XmlDocumentTemplateParser();
            var doc = parser.ParseTemplateFile(filePath);
            Console.WriteLine(doc.DumpDocumentTree());
        }
    }
}