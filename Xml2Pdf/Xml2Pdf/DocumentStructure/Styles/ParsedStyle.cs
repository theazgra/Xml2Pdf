namespace Xml2Pdf.DocumentStructure.Styles
{
    public class ParsedStyle
    {
        // private IReadOnlyDictionary<string, Color> _parsedColors;
        // private IReadOnlyDictionary<string, Unit> _parsedUnits;
        // private IReadOnlyDictionary<string, Style> _parsedStyles;
        //
        // /// <summary>
        // /// All parsed styles.
        // /// </summary>
        // internal IEnumerable<Style> Styles => _parsedStyles.Values;
        //
        // /// <summary>
        // /// Document page format.
        // /// </summary>
        // public PageFormat PageFormat { get; }
        //
        // /// <summary>
        // /// Document page orientation.
        // /// </summary>
        // public Orientation PageOrientation { get; }
        //
        // /// <summary>
        // /// Page width, works for different orientations.
        // /// </summary>
        // public Unit PageWidth => (PageOrientation == Orientation.Portrait) ? Constants.GetPageWidth(PageFormat) : Constants.GetPageHeight(PageFormat);
        //
        // /// <summary>
        // /// /// Page height, works for different orientations.
        // /// </summary>
        // public Unit PageHeight => (PageOrientation == Orientation.Portrait) ? Constants.GetPageHeight(PageFormat) : Constants.GetPageWidth(PageFormat);
        //
        // /// <summary>
        // /// Construct parsed style.
        // /// </summary>
        // /// <param name="colors">Parsed colors.</param>
        // /// <param name="units">Parsed units.</param>
        // /// <param name="styles">Parsed styles.</param>
        // /// <param name="pageFormat">Document page format.</param>
        // /// <param name="pageOrientation">Document page orientation.</param>
        //
        // public ParsedStyle() { }
        //
        // public ParsedStyle(IReadOnlyDictionary<string, Color> colors, IReadOnlyDictionary<string, Unit> units,
        //                     IReadOnlyDictionary<string, Style> styles,
        //                     PageFormat pageFormat,
        //                     Orientation pageOrientation)
        // {
        //     _parsedColors = colors;
        //     _parsedUnits = units;
        //     _parsedStyles = styles;
        //     PageFormat = pageFormat;
        //     PageOrientation = pageOrientation;
        // }
        //
        // /// <summary>
        // /// Try to get parsed style.
        // /// </summary>
        // /// <param name="key">Style key.</param>
        // /// <returns>Parsed style.</returns>
        // public Style GetStyle(string key)
        // {
        //     if (_parsedStyles.ContainsKey(key))
        //         return _parsedStyles[key];
        //
        //     throw new UnknownStyleValueException(typeof(Style), key);
        // }
        //
        // /// <summary>
        // /// Try to get parsed unit.
        // /// </summary>
        // /// <param name="key">Unit key.</param>
        // /// <returns>Parsed unit.</returns>
        // public Unit GetUnit(string key)
        // {
        //     if (_parsedUnits.ContainsKey(key))
        //         return _parsedUnits[key];
        //
        //     throw new UnknownStyleValueException(typeof(Unit), key);
        // }
        //
        // public string GetLoadedInfo()
        // {
        //     StringBuilder sb = new StringBuilder();
        //     sb.AppendLine("\tLoaded from styles: ");
        //     sb.AppendFormat("\t\tColors: {0} colors.\n", _parsedColors.Count);
        //     sb.AppendFormat("\t\tUnits: {0} units.\n", _parsedUnits.Count);
        //     sb.AppendFormat("\t\tStyles: {0} styles.\n", _parsedStyles.Count);
        //
        //     return sb.ToString();
        // }
        //
        // /// <summary>
        // /// Get parsed color.
        // /// </summary>
        // /// <param name="key">Color key.</param>
        // /// <returns>Parsed color.</returns>
        // public Color GetColor(string key)
        // {
        //     if (_parsedColors.ContainsKey(key))
        //         return _parsedColors[key];
        //     throw new UnknownStyleValueException(typeof(Color), key);
        // }
    }
}