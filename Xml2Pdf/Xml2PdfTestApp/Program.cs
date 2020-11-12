using System;
using System.Collections.Generic;
using Xml2Pdf.Format;
using Xml2Pdf.Format.Formatters;
using Xml2Pdf.Parser.Xml;
using Xml2Pdf.Renderer;

namespace Xml2PdfTestApp
{
    class City
    {
        public string Name { get; set; }
        public int Population { get; set; }
        public bool IsGrowing { get; set; }
    }

    class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public uint Age { get; set; }
        public Person Wife { get; set; }
        public List<City> Cities { get; set; }
    }

    class PersonFormatter : IPropertyFormatter
    {
        public int Priority => 10;
        public Type RegisteredType => typeof(Person);

        public string Format(object value)
        {
            if (value is Person person)
            {
                return $"{person.FirstName} {person.LastName} of age {person.Age}";
            }

            throw new FormatException("Unable to format object of type: " + value.GetType().Name);
        }
    }

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
                // renderer.ValueFormatter.AddFormatter(new PersonFormatter());
                renderer.ValueFormatter.AddFormatFunction<bool>(b => b ? "Ano" : "Ne");
                renderer.ValueFormatter.AddFormatFunction<Person>(person =>
                                                                      $"{person.FirstName} {person.LastName} of age {person.Age}");

                List<City> cities = new List<City>
                {
                    new City {Name = "Strahovice", Population = 900, IsGrowing = false},
                    new City {Name = "Hnevosice", Population = 1150, IsGrowing = false},
                    new City {Name = "Rohov", Population = 740, IsGrowing = true},
                };

                var p = new Person
                {
                    FirstName = "Vojtech",
                    LastName = "Moravec",
                    Age = 24,
                    Wife = new Person
                    {
                        FirstName = "Eliska",
                        LastName = "Moravcova",
                        Age = 22,
                    },
                    Cities = cities
                };


                renderer.RenderDocument(doc, renderTarget, p);

                Console.WriteLine("Document is rendered to {0}", renderTarget);
            }
        }
    }
}