using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using Org.BouncyCastle.Asn1.X509.Qualified;
using Xml2Pdf.Exceptions;

namespace Xml2Pdf.DocumentStructure
{
    public abstract class DocumentElement
    {
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

        protected DocumentElement() { }

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