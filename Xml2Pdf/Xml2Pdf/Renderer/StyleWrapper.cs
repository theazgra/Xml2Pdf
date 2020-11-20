using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using iText.Layout;
using iText.Layout.Properties;

namespace Xml2Pdf.Renderer
{
    /// <summary>
    /// iText Style wrapper, which allow access to properties dictionary.
    /// </summary>
    public sealed class StyleWrapper : Style
    {
        private static int[] BorderPropertyKeys =
        {
            9, // AllBorder
            10, // BottomBorder
            11, // LeftBorder
            12, // RightBorder
            13 // TopBorder
        };

        public StyleWrapper() { }

        // /// <summary>
        // /// Combine properties of two object. Second object can override first object's properties.
        // /// </summary>
        // /// <param name="a">First properties.</param>
        // /// <param name="b">Second properties.</param>
        // public StyleWrapper(StyleWrapper a, StyleWrapper b)
        // {
        //     properties = a.properties;
        //     CombineAndOverrideProperties(b);
        // }

        /// <summary>
        /// Combine this with <see cref="overridingStyle"/> and create new StyleWrapper.
        /// <see cref="overridingStyle"/> can override properties.
        /// </summary>
        /// <param name="overridingStyle">Style.</param>
        /// <returns>New style wrapper with combined properties.</returns>
        public StyleWrapper CombineStyles(StyleWrapper overridingStyle)
        {
            // Create new style.
            StyleWrapper newStyle = new StyleWrapper();

            // Copy original style.
            foreach (KeyValuePair<int, object> pair in properties)
            {
                newStyle.properties.Add(pair.Key, pair.Value);
            }

            if (overridingStyle == null)
                return newStyle;

            // Override with overridingStyle.
            foreach (KeyValuePair<int, object> pair in overridingStyle.properties)
            {
                if (newStyle.properties.ContainsKey(pair.Key))
                {
                    newStyle.properties[pair.Key] = pair.Value;
                }
                else
                {
                    newStyle.properties.Add(pair.Key, pair.Value);
                }
            }

            return newStyle;
        }

        public float GetFontSize()
        {
            const int fontSizeKey = 24;
            Debug.Assert(properties.ContainsKey(fontSizeKey));
            UnitValue fontSize = GetProperty<UnitValue>(fontSizeKey);
            Debug.Assert(fontSize.IsPointValue());
            return fontSize.GetValue();
        }

        public StyleWrapper RemoveBorderProperties()
        {
            StyleWrapper newStyle = new StyleWrapper();
            // Copy original style except border properties.
            foreach (KeyValuePair<int, object> pair in properties)
            {
                if (BorderPropertyKeys.Contains(pair.Key))
                    continue;
                newStyle.properties.Add(pair.Key, pair.Value);
            }

            return newStyle;
        }

        // /// <summary>
        // /// Add missing properties and override existing properties from other style
        // /// </summary>
        // /// <param name="otherStyle">Other style object.</param>
        // public void CombineAndOverrideProperties(StyleWrapper otherStyle)
        // {
        //     foreach (var pair in otherStyle.properties)
        //     {
        //         if (!properties.ContainsKey(pair.Key))
        //         {
        //             properties.Add(pair);
        //         }
        //         else
        //         {
        //             properties[pair.Key] = pair.Value;
        //         }
        //     }
        // }
    }
}