namespace Xml2Pdf.DocumentStructure
{
    /// <summary>
    ///Wrapper for document element property.
    /// It is useful for properties which can be inherited from parent to child elements of same base type.
    /// Only properties with initialized value (IsInitialized = true) are used in document rendering and inherited from
    /// parent to child.
    /// </summary>
    /// <typeparam name="T">Type of the property.</typeparam>
    public class ElementProperty<T>
    {
        /// <summary>
        /// Check if element property is initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Backing field for the value.
        /// </summary>
        private T _value;

        /// <summary>
        /// Value of the property. When the value is set, the property is marked as initialized.
        /// </summary>
        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                IsInitialized = true;
            }
        }

        /// <summary>
        /// Construct uninitialized document element property.
        /// </summary>
        public ElementProperty()
        {
            _value = default;
            IsInitialized = false;
        }

        /// <summary>
        /// Construct initialized document element property.
        /// </summary>
        /// <param name="value">Property value.</param>
        public ElementProperty(T value)
        {
            _value = value;
            IsInitialized = true;
        }
    }
}