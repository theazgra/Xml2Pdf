using System;
using System.Text;

namespace Xml2Pdf.DocumentStructure
{
    public class ParagraphElement : TextElement
    {
        private static readonly Type[] PossibleChildren = {typeof(TextElement)};

        public override bool IsParentType => true;
        public override Type[] AllowedChildrenTypes => PossibleChildren;

        protected override bool CanInheritBorderProperties => false;
    
        internal override void DumpToStringBuilder(StringBuilder dumpBuilder, int indent)
        {
            base.DumpToStringBuilder(dumpBuilder, indent);

            foreach (var child in Children)
            {
                child.DumpToStringBuilder(dumpBuilder, indent + DumpIndentationOffset);
            }
        }
    }
}