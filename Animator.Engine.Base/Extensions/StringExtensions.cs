using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base.Extensions
{
    public static class StringExtensions
    {
        public static string[] SplitUnquoted(this string s, char separator)
        {
            bool quote = false;

            int last = 0;
            int i = 0;

            List<string> result = new();

            while (i < s.Length)
            {
                if (s[i] == '\'')
                {
                    quote = !quote;
                    i++;
                    continue;
                }

                if (s[i] == '\\' && quote)
                {
                    i++;
                    if (i >= s.Length)
                        throw new InvalidOperationException("Invalid escape sequence!");
                    i++;
                    continue;
                }

                if (s[i] == separator && !quote)
                {
                    string splitted = s[last..i];
                    result.Add(splitted);

                    i++;
                    last = i;
                    continue;
                }

                i++;
            }

            if (quote)
                throw new InvalidOperationException("Unterminated quote!");

            string lastSplitted = s[last..];
            result.Add(lastSplitted);

            return result.ToArray();
        }

        public static bool ContainsUnquoted(this string s, char ch)
        {
            bool quote = false;

            int i = 0;

            while (i < s.Length)
            {
                if (s[i] == '\'')
                {
                    quote = !quote;
                    i++;
                    continue;
                }

                if (s[i] == '\\' && quote)
                {
                    i++;
                    if (i >= s.Length)
                        throw new InvalidOperationException("Invalid escape sequence!");
                    i++;
                    continue;
                }

                if (s[i] == ch && !quote)
                {
                    return true;
                }

                i++;
            }

            if (quote)
                throw new InvalidOperationException("Unterminated quote!");

            return false;
        }

        public static string ExpandQuotes(this string s)
        {
            // If there are no single-quotes, string is already unquoted
            if (!s.Contains('\''))
                return s;

            // If there is at least one single-quote, the whole string is expected
            // to be enclosed in single-quotes, else it's an error

            if (s.Length < 2 || s[0] != '\'' || s[s.Length - 1] != '\'')
                throw new InvalidOperationException("Incorrectly quoted string!");

            StringBuilder result = new StringBuilder();

            int i = 1;
            while (i < s.Length - 1)
            {
                if (s[i] != '\\')
                    result.Append(s[i]);
                else
                {
                    i++;
                    if (i >= s.Length - 1)
                        throw new InvalidOperationException("Invalid escape sequence!");

                    result.Append(s[i]);
                }

                i++;
            }

            return result.ToString();            
        }
    }
}
