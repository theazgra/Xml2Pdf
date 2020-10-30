using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xml2Pdf.Exceptions;

namespace Xml2Pdf.DocumentStructure
{
    public abstract class DocumentElement
    {
        protected delegate void ChildAdded(DocumentElement child);

        protected event ChildAdded OnChildAdded;

        private List<DocumentElement> _children;

        public IEnumerable<DocumentElement> Children => _children ?? Enumerable.Empty<DocumentElement>();

        public bool HasChildren => (_children != null && _children.Count > 0);

        public abstract bool IsParentType { get; }

        public abstract Type[] AllowedChildrenTypes { get; }

        protected DocumentElement() { }

        private bool CanHaveChildOfType(Type childType) => AllowedChildrenTypes.Contains(childType);

        internal virtual void DumpToStringBuilder(StringBuilder dumpBuilder, int indentationLevel)
        {
            PrepareIndent(dumpBuilder, indentationLevel).Append('<').Append(GetType().Name).Append('>').AppendLine();
        }

        protected StringBuilder PrepareIndent(StringBuilder dumpBuilder,
                                              in int indentationLevel)
        {
            for (int i = 0; i < indentationLevel; i++)
            {
                dumpBuilder.Append(' ');
            }

            return dumpBuilder;
        }

        public void AddChild(DocumentElement child)
        {
            if (!CanHaveChildOfType(child.GetType()))
            {
                throw UnexpectedDocumentElementException.WrongDocumentElement(child.GetType(), AllowedChildrenTypes);
            }

            _children ??= new List<DocumentElement>();
            _children.Add(child);
            OnChildAdded?.Invoke(child);
        }
    }
}