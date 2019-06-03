using System;

namespace SpanLibs
{
    public static class StringExtensions
    {
        public static string[] SpanSplit(this string value, char separator)
        {
            var span = value.AsSpan();
            var section = 1;
            foreach (var current in span)
            {
                if (current == separator)
                    section++;
            }
            var result = new string[section];

            var index = 0;
            var offset = 0;
            var length = 0;
            foreach (var current in span)
            {
                length++;
                if (current == separator)
                {
                    result[index++] = new string(span.Slice(offset, length - 1));
                    offset += length;
                    length = 0;
                }
            }
            result[index] = new string(span.Slice(offset, length));
            return result;
        }
    }
}
