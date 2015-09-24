using System;
using System.Collections.Generic;

namespace XAP.Common
{
    public static class Utility
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }

        public static int AsInt(this string value)
        {
            return int.Parse(value);
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            items.ForEach(collection.Add);
        }

        private static string[] _knownLinebreaks = new string[] 
        {
            "__br__",
            "\r\n",
            "\r",
            "\n",
            "<br/>",
            "<br />"
        };


        public const string XapNewLine = "__br__";
        public const string HtmlNewLine = "<br />";
        public const string NewLine = "\r\n";
        public static string NewLineAs(this string str, string newLine)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            _knownLinebreaks.ForEach((i) =>
            {
                str = str.Replace(i, newLine);
            });

            return str;
        }

        public static string TrimToLen(this string value, int length)
        {
            return value.Substring(0, Math.Min(length, value.Length));
        }
    }
}
