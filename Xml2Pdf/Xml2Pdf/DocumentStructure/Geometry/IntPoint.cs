namespace Xml2Pdf.DocumentStructure.Geometry
{
    public readonly struct IntPoint
    {
        public int X { get; }
        public int Y { get; }

        public IntPoint(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}