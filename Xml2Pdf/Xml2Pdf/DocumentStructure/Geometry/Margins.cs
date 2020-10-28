namespace Xml2Pdf.DocumentStructure.Geometry
{
    public readonly struct Margins
    {
        public float Left { get; }
        public float Top { get; }
        public float Right { get; }
        public float Bottom { get; }

        public Margins(float left, float top, float right, float bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public Margins(float leftRight, float topBottom) : this(leftRight, topBottom, leftRight, topBottom)
        {
        }

        public Margins(float margin) : this(margin, margin, margin, margin)
        {
        }
    }
}