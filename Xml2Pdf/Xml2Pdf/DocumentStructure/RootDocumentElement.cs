using System;
using System.Text;
using iText.Kernel.Geom;
using Xml2Pdf.DocumentStructure.Geometry;

namespace Xml2Pdf.DocumentStructure
{
    public class RootDocumentElement : DocumentElement
    {
        // TODO(Moravec): Add style element?
        private static readonly Type[] PossibleChildren =
        {
            typeof(PageElement),
        };

        public Margins CustomMargins { get; set; }
        public PageSize PageSize { get; set; } = PageSize.A4;
        public PageOrientation PageOrientation { get; set; } = PageOrientation.Portrait;

        public override bool IsParentType => true;
        public override Type[] AllowedChildrenTypes => PossibleChildren;

        internal override void DumpToStringBuilder(StringBuilder dumpBuilder, int indentationLevel)
        {
            base.DumpToStringBuilder(dumpBuilder, indentationLevel);
            PrepareIndent(dumpBuilder, indentationLevel).Append(" -PageSize=").Append(PageSize).AppendLine();
            PrepareIndent(dumpBuilder, indentationLevel).Append(" -PageOrientation=").Append(PageOrientation).AppendLine();
            if (CustomMargins != null)
                PrepareIndent(dumpBuilder, indentationLevel).Append(" #Margins: ").Append(CustomMargins);

            foreach (var child in Children)
            {
                child.DumpToStringBuilder(dumpBuilder, (indentationLevel + 2));
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