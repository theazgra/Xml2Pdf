using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Layout.Properties;
using Xml2Pdf.DocumentStructure.Geometry;
using Xml2Pdf.Exceptions;

namespace Xml2Pdf.Parser.Xml
{
    internal static class ValueParser
    {
        private static readonly Dictionary<string, Color> DefaultColorMap = new Dictionary<string, Color>
        {
            {"black", ColorConstants.BLACK},
            {"blue", ColorConstants.BLUE},
            {"cyan", ColorConstants.CYAN},
            {"darkGray", ColorConstants.DARK_GRAY},
            {"gray", ColorConstants.GRAY},
            {"green", ColorConstants.GREEN},
            {"lightGray", ColorConstants.LIGHT_GRAY},
            {"magenta", ColorConstants.MAGENTA},
            {"orange", ColorConstants.ORANGE},
            {"pink", ColorConstants.PINK},
            {"red", ColorConstants.RED},
            {"white", ColorConstants.WHITE},
            {"yellow", ColorConstants.YELLOW}
        };

        private static Color GetDefaultColor(string colorName)
        {
            string lc = colorName.ToLower();
            if (DefaultColorMap.ContainsKey(lc))
                return DefaultColorMap[lc];
            throw new ValueParseException($"Invalid color with name '{colorName}'");
        }

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

        internal static int ParseInt(string value) => int.Parse(value);

        internal static float ParseFloat(string propertyValue) =>
            float.Parse(propertyValue, CultureInfo.InvariantCulture);

        internal static float[] ParseFloatArray(string value)
        {
            var parts = value.Split(';', ',');
            var coeffs = parts.Select(x => ParseFloat(x)).ToArray();
            return coeffs;
        }

        internal static PageOrientation ParsePageOrientation(string propertyValue)
        {
            return propertyValue switch
            {
                "portrait" => PageOrientation.Portrait,
                "landscape" => PageOrientation.Landscape,
                _ => throw new ValueParseException($"Invalid value '{propertyValue}' for page orientation. " +
                                                   "Valid values are portrait and landscape.")
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

        internal static BorderType ParseBorderType(string value)
        {
            return value switch
            {
                "NoBorder" => BorderType.NoBorder,
                "Solid" => BorderType.Solid,
                "Dashed" => BorderType.Dashed,
                "Dotted" => BorderType.Dotted,
                "Double" => BorderType.Double,
                "RoundDots" => BorderType.RoundDots,
                "Groove3D" => BorderType.Groove3D,
                "Inset3D" => BorderType.Inset3D,
                "Outset3D" => BorderType.Outset3D,
                "Ridge3D" => BorderType.Ridge3D,
                _ => throw new ValueParseException($"Invalid borded type: '{value}'.")
            };
        }

        internal static Color ParseColor(string value)
        {
            var parts = value.Split(';', ',');

            return parts.Length switch
            {
                1 => GetDefaultColor(value),
                3 => new DeviceRgb(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2])),
                _ => throw new ValueParseException($"Unknown color '{value}'")
            };
        }

        internal static bool ParseBool(string value) { return (value == "true" || value == "yes"); }

        internal static HorizontalAlignment ParseHorizontalAlignment(string value)
        {
            return value switch
            {
                "left" => HorizontalAlignment.LEFT,
                "center" => HorizontalAlignment.CENTER,
                "right" => HorizontalAlignment.RIGHT,
                _ => throw new
                         ValueParseException("Invalid HorizontalAlignment, valid values are: left, center and right")
            };
        }

        internal static TextAlignment ParseTextAlignment(string value)
        {
            return value switch
            {
                "left" => TextAlignment.LEFT,
                "right" => TextAlignment.RIGHT,
                "center" => TextAlignment.CENTER,
                "justify" => TextAlignment.JUSTIFIED,
                "justifyAll" => TextAlignment.JUSTIFIED_ALL,
                _ => throw new
                         ValueParseException("Invalid HorizontalAlignment, valid values are: left, center and right")
            };
        }

        internal static VerticalAlignment ParseVerticalAlignment(string value)
        {
            return value switch
            {
                "top" => VerticalAlignment.TOP,
                "bottom" => VerticalAlignment.BOTTOM,
                "middle" => VerticalAlignment.MIDDLE,
                _ => throw new
                         ValueParseException("Invalid VerticalAlignment, valid values are: top, bottom and middle.")
            };
        }
    }
}