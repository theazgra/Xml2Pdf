using System;
using System.Text;

namespace Xml2Pdf.DocumentStructure
{
    public class PageElement : DocumentElement
    {
        private static readonly Type[] PossibleChildren =
        {
            typeof(HeaderElement),
            typeof(FooterElement),
            typeof(ImageElement),
            typeof(LineElement),
            typeof(ListElement),
            typeof(ParagraphElement),
            typeof(TableElement)
        };

        protected override bool IsParentType => true;
        protected override Type[] AllowedChildrenTypes => PossibleChildren;

        internal override void DumpToStringBuilder(StringBuilder dumpBuilder, int indent)
        {
            base.DumpToStringBuilder(dumpBuilder, indent);
            foreach (var child in Children)
            {
                child.DumpToStringBuilder(dumpBuilder, indent + DumpIndentationOffset);
            }
        }
    }

    public class HeaderElement : TextElement
    {
        private static readonly Type[] PossibleChildren = {typeof(ParagraphElement)};
        protected override bool IsParentType => true;
        protected override Type[] AllowedChildrenTypes => PossibleChildren;
    }

    public class FooterElement : TextElement
    {
        private static readonly Type[] PossibleChildren = {typeof(ParagraphElement)};
        protected override bool IsParentType => true;
        protected override Type[] AllowedChildrenTypes => PossibleChildren;
    }
}