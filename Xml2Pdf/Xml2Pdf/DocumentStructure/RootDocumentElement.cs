﻿using System;
using System.Diagnostics;
using System.Text;
using iText.Kernel.Geom;
using Xml2Pdf.DocumentStructure.Geometry;

namespace Xml2Pdf.DocumentStructure
{
    public class RootDocumentElement : DocumentElement
    {
        // TODO(Moravec): Add style element?
        private static readonly Type[] PossibleChildren = {typeof(PageElement)};

        public Margins CustomMargins { get; set; }
        public PageSize PageSize { get; set; } = PageSize.A4;
        public PageOrientation PageOrientation { get; set; } = PageOrientation.Portrait;

        public int PageCount { get; private set; }

        protected override bool IsParentType => true;
        protected override Type[] AllowedChildrenTypes => PossibleChildren;

        public RootDocumentElement()
        {
            OnChildAdded += child =>
                            {
                                Debug.Assert(child.GetType() == typeof(PageElement));
                                PageCount++;
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
            if (CustomMargins != null)
                PrepareIndent(dumpBuilder, indent).Append(" #Margins: ").Append(CustomMargins);

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