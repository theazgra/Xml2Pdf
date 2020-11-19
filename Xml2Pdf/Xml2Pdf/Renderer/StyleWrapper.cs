using iText.Layout;

namespace Xml2Pdf.Renderer
{
    /// <summary>
    /// iText Style wrapper, which allow access to properties dictionary.
    /// </summary>
    public sealed class StyleWrapper : Style
    {
        public StyleWrapper()
        {
        }

        /// <summary>
        /// Combine properties of two object. Second object can override first object's properties.
        /// </summary>
        /// <param name="a">First properties.</param>
        /// <param name="b">Second properties.</param>
        public StyleWrapper(StyleWrapper a, StyleWrapper b)
        {
            properties = a.properties;
            CombineAndOverrideProperties(b);
        }

        /// <summary>
        /// Add missing properties and override existing properties from other style
        /// </summary>
        /// <param name="otherStyle">Other style object.</param>
        public void CombineAndOverrideProperties(StyleWrapper otherStyle)
        {
            foreach (var pair in otherStyle.properties)
            {
                if (!properties.ContainsKey(pair.Key))
                {
                    properties.Add(pair);
                }
                else
                {
                    properties[pair.Key] = pair.Value;
                }
            }
        }
    }
}