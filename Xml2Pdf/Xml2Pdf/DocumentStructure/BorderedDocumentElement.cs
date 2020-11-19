using System.Text;
using iText.Layout;
using Xml2Pdf.DocumentStructure.Geometry;
using Xml2Pdf.Renderer;
using Xml2Pdf.Renderer.Mappers;

namespace Xml2Pdf.DocumentStructure
{
    public abstract class BorderedDocumentElement : DocumentElement
    {
        public ElementProperty<BorderInfo> Borders { get; } = new ElementProperty<BorderInfo>();
        public ElementProperty<BorderInfo> TopBorder { get; } = new ElementProperty<BorderInfo>();
        public ElementProperty<BorderInfo> BottomBorder { get; } = new ElementProperty<BorderInfo>();
        public ElementProperty<BorderInfo> LeftBorder { get; } = new ElementProperty<BorderInfo>();
        public ElementProperty<BorderInfo> RightBorder { get; } = new ElementProperty<BorderInfo>();

        protected virtual bool CanInheritBorderProperties => true;

        // TODO(Moravec): Border radius.

        protected BorderedDocumentElement()
        {
            OnChildAdded += child =>
            {
                if (child is BorderedDocumentElement borderedChild && borderedChild.CanInheritBorderProperties)
                {
                    borderedChild.InheritFrom(this);
                }
            };
        }

        private void InheritFrom(BorderedDocumentElement parent)
        {
            if (!Borders.IsInitialized && parent.Borders.IsInitialized)
            {
                Borders.Value = parent.Borders.Value;
            }

            if (!TopBorder.IsInitialized && parent.TopBorder.IsInitialized)
            {
                TopBorder.Value = parent.TopBorder.Value;
            }

            if (!BottomBorder.IsInitialized && parent.BottomBorder.IsInitialized)
            {
                BottomBorder.Value = parent.BottomBorder.Value;
            }

            if (!LeftBorder.IsInitialized && parent.LeftBorder.IsInitialized)
            {
                LeftBorder.Value = parent.LeftBorder.Value;
            }

            if (!RightBorder.IsInitialized && parent.RightBorder.IsInitialized)
            {
                RightBorder.Value = parent.RightBorder.Value;
            }
        }

        internal override void DumpToStringBuilder(StringBuilder dumpBuilder, int indent)
        {
            base.DumpToStringBuilder(dumpBuilder, indent);
            PrepareIndent(dumpBuilder, indent).Append("(BorderedDocumentElement)").AppendLine();

            DumpElementProperty(dumpBuilder, indent, nameof(Borders), Borders);
            DumpElementProperty(dumpBuilder, indent, nameof(TopBorder), TopBorder);
            DumpElementProperty(dumpBuilder, indent, nameof(BottomBorder), BottomBorder);
            DumpElementProperty(dumpBuilder, indent, nameof(LeftBorder), LeftBorder);
            DumpElementProperty(dumpBuilder, indent, nameof(RightBorder), RightBorder);
        }

        public StyleWrapper BorderPropertiesToStyle()
        {
            StyleWrapper style = new StyleWrapper();

            // Borders.
            if (Borders.IsInitialized)
            {
                style.SetBorder(Borders.Value.ToITextBorder());
            }
            else
            {
                if (TopBorder.IsInitialized)
                    style.SetBorderTop(TopBorder.Value.ToITextBorder());
                if (BottomBorder.IsInitialized)
                    style.SetBorderBottom(BottomBorder.Value.ToITextBorder());
                if (LeftBorder.IsInitialized)
                    style.SetBorderLeft(LeftBorder.Value.ToITextBorder());
                if (RightBorder.IsInitialized)
                    style.SetBorderRight(RightBorder.Value.ToITextBorder());
            }

            return style;
        }
    }
}