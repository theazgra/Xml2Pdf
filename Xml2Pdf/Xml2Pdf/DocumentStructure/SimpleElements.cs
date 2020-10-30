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
            typeof(ParagraphElement),
            typeof(TableElement)
        };

        public override bool IsParentType => true;
        public override Type[] AllowedChildrenTypes => PossibleChildren;

        internal override void DumpToStringBuilder(StringBuilder dumpBuilder, int indentationLevel)
        {
            base.DumpToStringBuilder(dumpBuilder, indentationLevel);
            foreach (var child in Children)
            {
                child.DumpToStringBuilder(dumpBuilder, indentationLevel + 2);
            }
        }
    }

    public class HeaderElement : TextElement
    {
        private static readonly Type[] PossibleChildren = {typeof(ParagraphElement)};
        public override bool IsParentType => true;
        public override Type[] AllowedChildrenTypes => PossibleChildren;
    }

    public class FooterElement : TextElement
    {
        private static readonly Type[] PossibleChildren = {typeof(ParagraphElement)};
        public override bool IsParentType => true;
        public override Type[] AllowedChildrenTypes => PossibleChildren;
    }
}