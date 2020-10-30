using System;
using System.Text;

namespace Xml2Pdf.DocumentStructure
{
    public class ParagraphElement : TextElement
    {
        private static readonly Type[] PossibleChildren = {typeof(TextElement)};

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
}