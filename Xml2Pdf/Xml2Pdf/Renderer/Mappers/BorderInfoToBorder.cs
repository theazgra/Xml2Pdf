using System;
using System.Reflection.Metadata.Ecma335;
using iText.Kernel.Colors;
using iText.Layout.Borders;
using Xml2Pdf.DocumentStructure.Geometry;

namespace Xml2Pdf.Renderer.Mappers
{
    public static class BorderInfoToBorder
    {
        public static Border ToITextBorder(this BorderInfo src)
        {
            return src.BorderType switch
            {
                BorderType.Solid => new SolidBorder(src.Color, src.Width, src.Opacity),
                BorderType.Dashed => new DashedBorder(src.Color, src.Width, src.Opacity),
                BorderType.Dotted => new DottedBorder(src.Color, src.Width, src.Opacity),
                BorderType.Double => new DoubleBorder(src.Color, src.Width, src.Opacity),
                BorderType.RoundDots => new RoundDotsBorder(src.Color, src.Width, src.Opacity),
                BorderType.Groove3D => new GrooveBorder(src.Color as DeviceRgb, src.Width, src.Opacity),
                BorderType.Inset3D => new InsetBorder(src.Color as DeviceRgb, src.Width, src.Opacity),
                BorderType.Outset3D => new OutsetBorder(src.Color as DeviceRgb, src.Width, src.Opacity),
                BorderType.Ridge3D => new RidgeBorder(src.Color as DeviceRgb, src.Width, src.Opacity),
                _ => null
            };
        }
    }
}