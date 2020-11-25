using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly static Dictionary<string, Color> ColorMap = new Dictionary<string, Color>
        {
            {
                "black", ColorConstants.BLACK
            },
            {
                "blue", ColorConstants.BLUE
            },
            {
                "cyan", ColorConstants.CYAN
            },
            {
                "darkGray", ColorConstants.DARK_GRAY
            },
            {
                "gray", ColorConstants.GRAY
            },
            {
                "green", ColorConstants.GREEN
            },
            {
                "lightGray", ColorConstants.LIGHT_GRAY
            },
            {
                "magenta", ColorConstants.MAGENTA
            },
            {
                "orange", ColorConstants.ORANGE
            },
            {
                "pink", ColorConstants.PINK
            },
            {
                "red", ColorConstants.RED
            },
            {
                "white", ColorConstants.WHITE
            },
            {
                "yellow", ColorConstants.YELLOW
            }
        };

        internal static void InjectNewColor(string colorName, Color color) { ColorMap.Add(colorName.ToLower(), color); }

        private static Color GetDefaultColor(string colorName)
        {
            string lc = colorName.ToLower();
            if (ColorMap.ContainsKey(lc))
                return ColorMap[lc];
            throw new ValueParseException($"Invalid color with name '{colorName}'");
        }

        internal static Margins ParseCompleteMargins(string value)
        {
            string[] parts = ParseStringArray(value);
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
            var parts = ParseStringArray(value);
            return parts.Select(ParseFloat).ToArray();
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

        internal static string[] ParseStringArray(string value) => value.Split(',', ';');

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

        internal static BorderInfo ParseBorderInfo(string value)
        {
            if (value == "none")
                return new BorderInfo
                {
                    BorderType = BorderType.NoBorder
                };
            var parts = ParseStringArray(value);
            if (parts.Length < 3)
            {
                throw new ValueParseException("Unable to parse BorderInfo. " +
                                              "Expected values like: [1.5;solid;black] or [1.5,dotted,black,(0.5)]");
            }

            BorderInfo parsed = new BorderInfo
            {
                Width = ParseFloat(parts[0]),
                BorderType = ParseBorderType(parts[1]),
                Color = ParseColor(parts[2]),
                Opacity = parts.Length > 3 ? ParseFloat(parts[3]) : 1.0f
            };
            return parsed;
        }

        private static BorderType ParseBorderType(string value)
        {
            return value.ToLower() switch
            {
                "noborder" => BorderType.NoBorder,
                "solid" => BorderType.Solid,
                "dashed" => BorderType.Dashed,
                "dotted" => BorderType.Dotted,
                "double" => BorderType.Double,
                "rounddots" => BorderType.RoundDots,
                "groove3d" => BorderType.Groove3D,
                "inset3d" => BorderType.Inset3D,
                "outset3d" => BorderType.Outset3D,
                "ridge3d" => BorderType.Ridge3D,
                _ => throw new ValueParseException($"Invalid borded type: '{value}'.")
            };
        }

        internal static byte HexToByte(ReadOnlySpan<char> hexString)
        {
            Debug.Assert(hexString.Length == 2);

            int HexValue(char c)
            {
                if (c >= '0' && c <= '9')
                {
                    return (c - 48);
                }

                if (c >= 'a' && c <= 'f')
                {
                    return (c - 97 + 10);
                }

                if (c >= 'A' && c <= 'F')
                {
                    return (c - 65 + 10);
                }

                throw new ValueParseException($"Invalid hex character '{c}'");
            }

            var h1 = HexValue(hexString[0]);
            var h0 = HexValue(hexString[1]);

            return (byte) ((h1 * 16) + h0);
        }

        internal static Color ParseColor(string value)
        {
            if (value.StartsWith("0x")) // Parse hex color.
            {
                if (value.Length != 8)
                    throw new ValueParseException("Expected hexadecimal value of length = 6, e.g.: 0x######");
                byte r, g, b;
                try
                {
                    r = HexToByte(value[2..4]);
                    g = HexToByte(value[4..6]);
                    b = HexToByte(value[6..8]);
                }
                catch (ValueParseException e)
                {
                    throw new ValueParseException("Failed to parse hexadecimal color.", e);
                }


                return new DeviceRgb(r, g, b);
            }

            var parts = ParseStringArray(value);
            return parts.Length switch
            {
                1 => GetDefaultColor(value),
                3 => new DeviceRgb(byte.Parse(parts[0]), byte.Parse(parts[1]), byte.Parse(parts[2])),
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

        internal static UnitValue ParseUnitValue(string value)
        {
            return value.EndsWith('%')
                ? UnitValue.CreatePercentValue(ParseFloat(value.Substring(0, value.Length - 1)))
                : UnitValue.CreatePointValue(ParseFloat(value));
        }

        internal static UnitValue[] ParseUnitValueArray(string value) => ParseStringArray(value).Select(ParseUnitValue).ToArray();

        public static Rectangle ParseRectangle(string value)
        {
            string[] parts = ParseStringArray(value);
            return parts.Length switch
            {
                2 => new Rectangle(ParseFloat(parts[0]), ParseFloat(parts[1])),
                4 => new Rectangle(ParseFloat(parts[0]), ParseFloat(parts[1]), ParseFloat(parts[2]), ParseFloat(parts[3])),
                _ => throw new
                    ValueParseException("You must specify two values (width,height) or four values (x,y,width,height) for rectangle.")
            };
        }

        public static FixedPosition ParseFixedPosition(string value)
        {
            string[] parts = ParseStringArray(value);
            return parts.Length switch
            {
                3 => new FixedPosition(ParseFloat(parts[0]), ParseFloat(parts[1]), ParseUnitValue(parts[2])),
                4 => new FixedPosition(ParseFloat(parts[0]), ParseFloat(parts[1]), ParseUnitValue(parts[2]), ParseUnitValue(parts[3])),
                _ => throw new
                    ValueParseException("You must specify three values (x,y,width) or four values (x,y,width,height) for fixed position.")
            };
        }
    }
}