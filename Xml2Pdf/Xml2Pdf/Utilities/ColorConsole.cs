using System;

namespace Xml2Pdf.Utilities
{
    internal static class ColorConsole
    {
        internal static void WriteLine(ConsoleColor color, string format, params object[] parameters)
        {
            WriteLine(color, string.Format(format, parameters));
        }

        internal static void WriteLine(ConsoleColor color, string str)
        {
            var originalColor = Console.ForegroundColor;
            try
            {
                Console.ForegroundColor = color;
                Console.WriteLine(str);
            }
            finally
            {
                Console.ForegroundColor = originalColor;
            }
        }
    }
}