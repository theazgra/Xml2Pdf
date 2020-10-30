using iText.Signatures;

namespace Xml2Pdf.DocumentStructure.Geometry
{
    public class Margins
    {
        public float? Left { get; set; }
        public float? Top { get; set; }
        public float? Right { get; set; }
        public float? Bottom { get; set; }

        public Margins(float left, float top, float right, float bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public bool AreComplete() => (Left.HasValue && Top.HasValue && Right.HasValue && Bottom.HasValue);

        public Margins(float leftRight, float topBottom) : this(leftRight, topBottom, leftRight, topBottom) { }

        public Margins(float margin) : this(margin, margin, margin, margin) { }

        public Margins() { }

        public override string ToString() { return $"L={Left};T={Top};R={Right};B={Bottom}"; }
    }
}