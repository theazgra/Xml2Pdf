using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Xml2Pdf.Parser
{
    internal class PropertyPair<T>
    {
        private T _value;
        public string Name { get; set; }


        public T Value
        {
            get
            {
                IsProcessed = true;
                return _value;
            }
            private set => _value = value;
        }

        public bool IsProcessed { get; private set; } = false;

        internal PropertyPair(string name, T value)
        {
            Name = name;
            Value = value;
        }

        public void Deconstruct(out string name, out T value)
        {
            name = Name;
            value = Value;
        }
    }

    internal class PropertyBag<T>
    {
        private readonly PropertyPair<T>[] _bag;
        internal PropertyBag(int bagSize) { _bag = new PropertyPair<T>[bagSize]; }

        internal PropertyPair<T> this[int index]
        {
            get => _bag[index];
            set => _bag[index] = value;
        }

        internal IEnumerable<PropertyPair<T>> UnprocessedPairs() => _bag.Where(pair => !pair.IsProcessed);
    }
}