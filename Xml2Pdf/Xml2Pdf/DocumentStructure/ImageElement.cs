namespace Xml2Pdf.DocumentStructure
{
    public class ImageElement : BorderedDocumentElement
    {
        public string ImageDataSourceProperty { get; set; }
        public float Left { get; set; }
        public float Bottom { get; set; }
        public float Width { get; set; }
        public float HorizontalScaling { get; set; } = 1.0f;
        public float VerticalScaling { get; set; } = 1.0f;
    }
}