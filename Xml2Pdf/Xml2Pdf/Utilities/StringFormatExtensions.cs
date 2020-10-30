using System.Globalization;
using System.Linq;
using iText.Kernel.Colors;

namespace Xml2Pdf.Utilities
{
    internal static class StringFormatExtensions
    {
        internal static string ToPrettyString(this Color color)
        {
            
            return '[' + string.Join(';', color.GetColorValue().Select(v => (v * 255).ToString(CultureInfo.InvariantCulture))) +
                   ']';
        }
    }
}