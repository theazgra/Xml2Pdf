﻿using System;
using System.Text;

namespace Xml2Pdf.DocumentStructure
{
    public class ParagraphElement : TextElement
    {
        private static readonly Type[] PossibleChildren = {typeof(LeafTextElement), typeof(ImageElement)};

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
}