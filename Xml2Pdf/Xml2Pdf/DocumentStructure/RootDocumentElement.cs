using System;
using System.Text;
using iText.Kernel.Geom;
using Xml2Pdf.DocumentStructure.Geometry;

namespace Xml2Pdf.DocumentStructure
{
    public class RootDocumentElement : DocumentElement
    {
        private static readonly Type[] PossibleChildren = {typeof(PageElement)};

        public ElementStyle Style { get; } = new ElementStyle();

        public PageSize PageSize { get; set; } = PageSize.A4;
        public PageOrientation PageOrientation { get; set; } = PageOrientation.Portrait;

        public ElementProperty<string> StyleFile { get; } = new ElementProperty<string>();
        public ElementProperty<string> DocumentFont { get; } = new ElementProperty<string>();
        public ElementProperty<float> DocumentFontSize { get; } = new ElementProperty<float>();

        public int PageCount { get; private set; }

        protected override bool IsParentType => true;
        protected override Type[] AllowedChildrenTypes => PossibleChildren;

        public RootDocumentElement()
        {
            OnChildAdded += child =>
            {
                if (child.GetType() == typeof(PageElement))
                {
                    PageCount++;
                }
            };
        }

        internal override void DumpToStringBuilder(StringBuilder dumpBuilder, int indent)
        {
            base.DumpToStringBuilder(dumpBuilder, indent);
            PrepareIndent(dumpBuilder, indent).Append(" -PageSize=").Append(PageSize).AppendLine();
            PrepareIndent(dumpBuilder, indent)
                .Append(" -PageOrientation=")
                .Append(PageOrientation)
                .AppendLine();

            DumpElementProperty(dumpBuilder, indent, nameof(DocumentFont), DocumentFont);
            DumpElementProperty(dumpBuilder, indent, nameof(DocumentFontSize), DocumentFontSize);

            foreach (var child in Children)
            {
                child.DumpToStringBuilder(dumpBuilder, (indent + DumpIndentationOffset));
            }
        }


        public string DumpDocumentTree()
        {
            var dumpBuilder = new StringBuilder();
            DumpToStringBuilder(dumpBuilder, 0);
            return dumpBuilder.ToString();
        }
    }
}