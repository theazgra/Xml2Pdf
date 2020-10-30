using iText.Kernel.Geom;
using iText.Layout.Properties;

namespace Xml2Pdf
{
    internal static class Constants
    {
        internal const char XpathDelimiter = '/';

#region Style

        internal const string PdfDocumentStyleRootElement = "PdfDocumentStyle";
        internal const string ColorsElement = "Colors";
        internal const string ColorElement = "Color";
        internal const string UnitsElement = "Units";
        internal const string UnitElement = "Unit";
        internal const string StylesElement = "Styles";
        internal const string StyleElement = "Style";
        internal const string Font = "Font";
        internal const string KeyAttribute = "key";
        internal const string ValueAttribute = "value";
        internal const string NameAttribute = "name";
        internal const string SizeAttribute = "size";
        internal const string ColorAttribute = "color";
        internal const string RgbaAttribute = "rgba";
        internal const string BasedOnAttribute = "basedOn";
        internal const string ParagraphFormat = "ParagraphFormat";
        internal const string AlignmentAttribute = "alignment";
        internal const string PageBreakBeforeAttribute = "pageBreakBefore";
        internal const string SpaceAfterAttribute = "spaceAfter";
        internal const string SpaceBeforeAttribute = "spaceBefore";
        internal const string Shading = "Shading";
        internal const string WidthAttribute = "width";
        internal const string HeightAttribute = "height";
        internal const string ScaleAttribute = "scale";
        internal const string DistanceAttribute = "distance";
        internal const string Border = "Border";

#endregion

#region Document

        internal const string StyleAttribute = "style";
        internal const string DistanceFromBordersAttribute = "distanceFromBorders";
        internal const string TopDistanceFromBorderAttribute = "topDistanceFromBorder";
        internal const string BottomDistanceFromBorderAttribute = "bottomDistanceFromBorder";
        internal const string LeftDistanceFromBorderAttribute = "leftDistanceFromBorder";
        internal const string RightDistanceFromBorderAttribute = "rightDistanceFromBorder";

        internal const string ColumnCountAttribute = "columnCount";
        internal const string OnlyOutsideBorderAttribute = "onlyOutsideBorder";
        internal const string OutsideBorderWidthAttribute = "outsideBorderWidth";
        internal const string ColumnsWidthAttribute = "columnsWidth";
        internal const string RowHeightAttribute = "rowHeight";
        internal const string TableAllBordersAttribute = "allBorders";
        internal const string RowDataTemplate = "RowDataTemplate";
        internal const string HeadingFormatAttribute = "headingFormat";
        internal const string ItemsSourceAttribute = "itemsSource";
        internal const string XAttribute = "x";
        internal const string YAttribute = "y";
        internal const string MergeDownAttribute = "mergeDown";
        internal const string MergeRightAttribute = "mergeRight";
        internal const string BackgroundColorAttribute = "background";
        internal const string FontColorAttribute = "fontColor";

#endregion
    }
}