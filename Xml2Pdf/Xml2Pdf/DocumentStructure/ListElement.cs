using System;
using System.Text;

namespace Xml2Pdf.DocumentStructure
{
    public sealed class ListItemElement : LeafTextElement
    {
    }

    public class ListElement : TextElement
    {
        private static readonly Type[] PossibleChildren = {typeof(ListItemElement)};

        public ElementProperty<int> StartIndex { get; } = new ElementProperty<int>();
        public ElementProperty<float> Indentation { get; } = new ElementProperty<float>();
        public ElementProperty<string> ListSymbol { get; } = new ElementProperty<string>();
        public ElementProperty<string> PreSymbolText { get; } = new ElementProperty<string>();
        public ElementProperty<string> PostSymbolText { get; } = new ElementProperty<string>();
        public ElementProperty<bool> Enumeration { get; } = new ElementProperty<bool>();

        public override bool IsParentType => true;
        public override Type[] AllowedChildrenTypes => PossibleChildren;


        internal override void DumpToStringBuilder(StringBuilder dumpBuilder, int indent)
        {
            base.DumpToStringBuilder(dumpBuilder, indent);
            DumpElementProperty(dumpBuilder, indent, nameof(Enumeration), Enumeration);
            DumpElementProperty(dumpBuilder, indent, nameof(Indentation), Indentation);
            DumpElementProperty(dumpBuilder, indent, nameof(ListSymbol), ListSymbol);
            DumpElementProperty(dumpBuilder, indent, nameof(StartIndex), StartIndex);
            DumpElementProperty(dumpBuilder, indent, nameof(PreSymbolText), PreSymbolText);
            DumpElementProperty(dumpBuilder, indent, nameof(PostSymbolText), PostSymbolText);
            foreach (var child in Children)
            {
                child.DumpToStringBuilder(dumpBuilder, indent + DumpIndentationOffset);
            }
        }
    }
}