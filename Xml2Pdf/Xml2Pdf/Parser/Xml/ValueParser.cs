using System.Globalization;
using iText.Kernel.Geom;
using Xml2Pdf.DocumentStructure.Geometry;
using Xml2Pdf.Exceptions;

namespace Xml2Pdf.Parser.Xml
{
    internal static class ValueParser
    {
        internal static Margins ParseCompleteMargins(string value)
        {
            string[] parts = value.Split(',', ';');
            return parts.Length switch
            {
                1 => new Margins(ParseFloat(parts[0])),
                2 => new Margins(ParseFloat(parts[0]), float.Parse(parts[1])),
                4 => new Margins(ParseFloat(parts[0]),
                                 ParseFloat(parts[1]),
                                 ParseFloat(parts[2]),
                                 ParseFloat(parts[3])),
                _ => throw new
                         ValueParseException("Unable to parse margins. Expected 1, 2 or 4 floats separated by ';' or ','")
            };
        }

        internal static float ParseFloat(string propertyValue) =>
            float.Parse(propertyValue, CultureInfo.InvariantCulture);

        internal static PageOrientation ParsePageOrientation(string propertyValue)
        {
            return propertyValue switch
            {
                Constants.Portrait => PageOrientation.Portrait,
                Constants.Landscape => PageOrientation.Landscape,
                _ => throw new ValueParseException($"Invalid value '{propertyValue}' for page orientation. " +
                                                   $"Valid values are {Constants.Portrait} and {Constants.Landscape}.")
            };
        }

        internal static PageSize ParsePageSize(string propertyValue)
        {
            return propertyValue switch
            {
                "A0" => PageSize.A0,
                "A1" => PageSize.A1,
                "A2" => PageSize.A2,
                "A3" => PageSize.A3,
                "A4" => PageSize.A4,
                "A5" => PageSize.A5,
                "A6" => PageSize.A6,
                "A7" => PageSize.A7,
                "A8" => PageSize.A8,
                "A9" => PageSize.A9,
                "A10" => PageSize.A10,
                "B0" => PageSize.B0,
                "B1" => PageSize.B1,
                "B2" => PageSize.B2,
                "B3" => PageSize.B3,
                "B4" => PageSize.B4,
                "B5" => PageSize.B5,
                "B6" => PageSize.B6,
                "B7" => PageSize.B7,
                "B8" => PageSize.B8,
                "B9" => PageSize.B9,
                "B10" => PageSize.B10,
                "Letter" => PageSize.LETTER,
                "Legal" => PageSize.LEGAL,
                "Tabloid" => PageSize.TABLOID,
                "Ledger" => PageSize.LEDGER,
                "Executive" => PageSize.EXECUTIVE,
                _ => throw new ValueParseException("Invalid value for page size.")
            };
        }
    }
}