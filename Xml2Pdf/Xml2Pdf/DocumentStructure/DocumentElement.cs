using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using Xml2Pdf.DocumentStructure.Geometry;
using Xml2Pdf.Exceptions;
using Xml2Pdf.Renderer;

namespace Xml2Pdf.DocumentStructure
{
    public abstract class DocumentElement
    {
#region TreeStructureProperties

        protected const int DumpIndentationOffset = 2;

        /// <summary>
        /// Children of given element.
        /// </summary>
        private List<DocumentElement> _children;

        /// <summary>
        /// Delegate information about new child being added.
        /// </summary>
        /// <param name="child">Child added to current element.</param>
        protected delegate void ChildAdded(DocumentElement child);

        /// <summary>
        /// Event fired, when new childr is added.
        /// </summary>
        protected event ChildAdded OnChildAdded;

        /// <summary>
        /// Get the first Children.
        /// </summary>
        public DocumentElement FirstChild => _children?.Count > 0 ? _children[0] : null;

        /// <summary>
        /// Get number of children.
        /// </summary>
        public int ChildrenCount => _children.Count;

        /// <summary>
        /// Get enumeration of children.
        /// </summary>
        public IEnumerable<DocumentElement> Children => _children ?? Enumerable.Empty<DocumentElement>();

        /// <summary>
        /// True if element has some child.
        /// </summary>
        public bool HasChildren => (_children != null && _children.Count > 0);

        /// <summary>
        /// Flag whether element can have children.
        /// </summary>
        protected abstract bool IsParentType { get; }

        /// <summary>
        /// Possible type of children.
        /// </summary>
        protected abstract Type[] AllowedChildrenTypes { get; }

#endregion

#region DocumentElementProperties

        public ElementProperty<Margins> Margins { get; } = new ElementProperty<Margins>();
        public ElementProperty<FixedPosition> FixedPosition { get; } = new ElementProperty<FixedPosition>();

#endregion

        protected DocumentElement() { }

        public DocumentElement GetChildrenAtIndex(int index) { return _children[index]; }

        /// <summary>
        /// Check if this element can have child of given type.
        /// </summary>
        /// <param name="childType">Type of the child.</param>
        /// <returns>True if given element can have child of given type.</returns>
        private bool CanHaveChildOfType(Type childType) => AllowedChildrenTypes.Contains(childType);

        /// <summary>
        /// Virtual method used to dump current document element to string builder.
        /// </summary>
        /// <param name="dumpBuilder">String builder.</param>
        /// <param name="indent">Indentation level.</param>
        internal virtual void DumpToStringBuilder(StringBuilder dumpBuilder, int indent)
        {
            PrepareIndent(dumpBuilder, indent).Append('<').Append(GetType().Name).Append('>').AppendLine();
            DumpElementProperty(dumpBuilder, indent, nameof(Margins), Margins);
            DumpElementProperty(dumpBuilder, indent, nameof(FixedPosition), FixedPosition);
        }

        /// <summary>
        /// Add current element properties to style. This is virtual function so the call is propagated all the way to the base class.
        /// </summary>
        /// <returns>Style with element properties.</returns>
        [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
        public virtual StyleWrapper GetElementStyle(Dictionary<string, PdfFont> customFonts,
                                                    PageSize page)
        {
            StyleWrapper style = new StyleWrapper();
            if (Margins.IsInitialized)
            {
                if (Margins.Value.AreComplete())
                {
                    style.SetMargins(Margins.Value.Top.Value,
                                     Margins.Value.Right.Value,
                                     Margins.Value.Bottom.Value,
                                     Margins.Value.Left.Value);
                }
                else
                {
                    if (Margins.Value.Top.HasValue)
                        style.SetMarginTop(Margins.Value.Top.Value);
                    if (Margins.Value.Bottom.HasValue)
                        style.SetMarginBottom(Margins.Value.Bottom.Value);
                    if (Margins.Value.Left.HasValue)
                        style.SetMarginLeft(Margins.Value.Left.Value);
                    if (Margins.Value.Right.HasValue)
                        style.SetMarginRight(Margins.Value.Right.Value);
                }
            }

            // TODO(Moravec):   Fixed position is now from bottom-left corner, if we want to transform
            //                  it to top-left corner, we have to pass page rectangle to this function.

            // TODO(Moravec):   Add page index.

            if (FixedPosition.IsInitialized)
            {
                style.SetFixedPosition(FixedPosition.Value.X, FixedPosition.Value.Y, FixedPosition.Value.Width);
            }

            return style;
        }

        /// <summary>
        /// Dump <see cref="ElementProperty{T}"/> name and its value if initialized. Uninitialized property is skipped.
        /// </summary>
        /// <param name="dumpBuilder">String builder.</param>
        /// <param name="indentationLevel">Indentation level.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="elementProperty">Document element property.</param>
        /// <typeparam name="T">Type of the document element property value.</typeparam>
        protected void DumpElementProperty<T>(StringBuilder dumpBuilder,
                                              in int indentationLevel,
                                              string propertyName,
                                              ElementProperty<T> elementProperty)
        {
            if (!elementProperty.IsInitialized)
                return;

            string prettyValue;
            if (elementProperty.Value is string[] stringArray)
                prettyValue = '[' + string.Join(", ", stringArray) + ']';
            else
                prettyValue = elementProperty.Value.ToString();


            //if (typeof(T))


            PrepareIndent(dumpBuilder, indentationLevel)
                .Append(" -")
                .Append(propertyName)
                .Append(": '")
                .Append(prettyValue)
                .Append('\'')
                .AppendLine();
        }

        /// <summary>
        /// Prepare element dump indentation.
        /// </summary>
        /// <param name="dumpBuilder">String builder.</param>
        /// <param name="indentationLevel">Indentation level.</param>
        /// <returns>used string builder.</returns>
        protected StringBuilder PrepareIndent(StringBuilder dumpBuilder,
                                              in int indentationLevel)
        {
            for (int i = 0; i < indentationLevel; i++)
            {
                dumpBuilder.Append(' ');
            }

            return dumpBuilder;
        }

        /// <summary>
        /// Tries to add child to this document element.
        /// If current element can't gave child of child type <see cref="UnexpectedDocumentElementException"/> is thrown.
        /// </summary>
        /// <param name="child">Child to be added to this parent.</param>
        /// <exception cref="UnexpectedDocumentElementException">is thrown if current element is not parenting type or can't have child if given type.</exception>
        public void AddChild(DocumentElement child)
        {
            if (IsParentType && !CanHaveChildOfType(child.GetType()))
            {
                throw UnexpectedDocumentElementException.WrongDocumentElement(child.GetType(), AllowedChildrenTypes);
            }

            _children ??= new List<DocumentElement>();
            _children.Add(child);
            OnChildAdded?.Invoke(child);
        }
    }
}