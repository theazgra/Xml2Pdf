using System;
using System.Collections.Generic;
using System.Linq;
using Xml2Pdf.Exceptions;

namespace Xml2Pdf.Format
{
    public class ValueFormatter
    {
        private class CallbackFormatter<T> : IPropertyFormatter
        {
            private Func<T, string> _formatFunction = null;
            public int Priority => 0;
            public Type RegisteredType => typeof(T);
            public string Format(object value) => _formatFunction((T) value);

            internal CallbackFormatter(Func<T, string> formatFunction) { _formatFunction = formatFunction; }
        }

        private readonly List<IPropertyFormatter> _formatters = new List<IPropertyFormatter>();

        public void AddFormatter(IPropertyFormatter formatter) { _formatters.Add(formatter); }

        public void AddFormatFunction<T>(Func<T, string> formatFunction)
        {
            AddFormatter(new CallbackFormatter<T>(formatFunction));
        }

        public string FormatValue(object value)
        {
            var valueType = value.GetType();
            IPropertyFormatter formatter = null;
            if (_formatters.SingleOrDefault(f => valueType == f.RegisteredType) is { } exactFormatter)
                formatter = exactFormatter;
            else
            {
                formatter = _formatters
                            .Where(f => valueType.IsSubclassOf(f.RegisteredType))
                            .OrderByDescending(f => f.Priority)
                            .FirstOrDefault();
            }

            if (formatter != null)
                return formatter.Format(value);

            throw new MissingFormatterException($"There is no property formatter for type '{value.GetType().Name}'");
        }
    }
}