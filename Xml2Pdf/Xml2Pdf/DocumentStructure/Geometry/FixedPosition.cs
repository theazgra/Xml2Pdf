using iText.Kernel.Geom;
using iText.Layout.Properties;

namespace Xml2Pdf.DocumentStructure.Geometry
{
    public readonly struct FixedPosition
    {
        public float X { get; }
        public float Y { get; }
        public UnitValue Width { get; }
        public UnitValue Height { get; }
        public int? Page { get; }


        public FixedPosition(float x, float y, UnitValue width)
        {
            X = x;
            Y = y;
            Width = width;
            Height = null;
            Page = null;
        }

        public FixedPosition(float x, float y, UnitValue width, UnitValue height) : this(x, y, width) { Height = height; }
        public FixedPosition(float x, float y, UnitValue width, UnitValue height, int page) : this(x, y, width, height) { Page = page; }

        public Rectangle ToRectangle() { return new(X, Y, Width.GetValue(), Height.GetValue()); }
    }
}