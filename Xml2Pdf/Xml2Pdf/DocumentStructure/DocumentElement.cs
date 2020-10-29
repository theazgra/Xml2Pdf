using System;
using System.Collections.Generic;
using System.Linq;
using Xml2Pdf.Exceptions;

namespace Xml2Pdf.DocumentStructure
{
    public abstract class DocumentElement
    {
        private List<DocumentElement> _children;

        public IReadOnlyList<DocumentElement> Children => _children.AsReadOnly();
        public bool HasChildren => (_children.Count > 0);

        public abstract bool IsParentType { get; }

        public abstract Type[] AllowedChildrenTypes { get; }

        protected DocumentElement() { }

        protected bool CanHaveChildOfType(Type childType) => AllowedChildrenTypes.Contains(childType);

        public void AddChild(DocumentElement child)
        {
            if (!CanHaveChildOfType(child.GetType()))
            {
                throw UnexpectedDocumentElementException.WrongDocumentElement(child.GetType(), AllowedChildrenTypes);
            }

            _children.Add(child);
        }
    }
}