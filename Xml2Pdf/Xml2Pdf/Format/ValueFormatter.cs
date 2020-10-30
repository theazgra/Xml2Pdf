using System.Collections.Generic;
using System.Linq;
using Xml2Pdf.Exceptions;

namespace Xml2Pdf.Format
{
    public class ValueFormatter
    {
        private readonly List<IPropertyFormatter> _formatters = new List<IPropertyFormatter>();

        public void AddFormatter(IPropertyFormatter formatter) { _formatters.Add(formatter); }

        public string FormatValue(object value)
        {
            IPropertyFormatter formatter = _formatters.Where(f => value.GetType().IsSubclassOf(f.RegisteredType))
                                                      .OrderByDescending(f => f.Priority)
                                                      .FirstOrDefault();
            if (formatter != null)
                return formatter.Format(value);

            throw new MissingFormatterException($"There is no property formatter for type '{value.GetType().Name}'");
        }
    }
}