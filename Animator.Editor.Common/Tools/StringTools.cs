using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Editor.Common.Tools
{
    public static class StringTools
    {
        /// <remarks>https://referencesource.microsoft.com/system/R/43723ee9a1ef09ab.html</remarks>
        private static int HexDigit(char ch)
        {
            int d;

            if ((uint)(d = ch - '0') <= 9)
                return d;

            if ((uint)(d = ch - 'a') <= 5)
                return d + 0xa;

            if ((uint)(d = ch - 'A') <= 5)
                return d + 0xa;

            return -1;
        }

        ///<remarks>https://referencesource.microsoft.com/system/R/f9e89f1eede95acd.html</remarks>
        private static char ScanOctal(string input, ref int pos)
        {
            int chars = 3;
            int digit;
            int resultCode = 0;

            while (pos < input.Length && (uint)(digit = input[pos] - '0') < 7 && chars > 0)
            {
                resultCode = resultCode * 8 + digit;
                pos++;
            }

            resultCode &= 0xFF;

            return (char)resultCode;
        }


        /// <remarks>https://referencesource.microsoft.com/system/R/7cc9c5a36b728209.html</remarks>
        private static char ScanHex(string input, ref int pos, int chars)
        {
            if (input.Length - pos < chars)
                throw new ArgumentException("Invalid escape sequence!");

            int resultCode = 0;

            for (int i = 0; i < chars; i++)
            {
                resultCode = resultCode * 0x10 + HexDigit(input[pos]);
                pos++;
            }

            return (char)resultCode;
        }

        /// <remarks>https://referencesource.microsoft.com/system/R/224cee27e14210c4.html</remarks>
        private static char ScanControl(string input, ref int pos)
        {
            pos++;
            if (pos >= input.Length)
                throw new ArgumentException("Invalid escape sequence!");

            char ch = input[pos];
            pos++;            

            if (ch >= 'a' && ch <= 'z')
                ch = (char)(ch - ('a' - 'A'));

            if ((ch = (char)(ch - '@')) < ' ')
                return ch;

            throw new ArgumentException("Invalid escape sequence!");
        }

        /// <remarks>https://referencesource.microsoft.com/system/R/9247c4f54f140280.html</remarks>
        public static string Unescape(this string input)
        {
            StringBuilder sb = new StringBuilder();

            int pos = 0;
            while (pos < input.Length)
            {
                if (input[pos] == '\\')
                {
                    if (++pos >= input.Length)
                        throw new ArgumentException("Invalid escape sequence!");

                    switch (input[pos])
                    {
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                            sb.Append(ScanOctal(input, ref pos));
                            continue;
                        case 'x':
                            sb.Append(ScanHex(input, ref pos, 2));
                            continue;
                        case 'u':
                            sb.Append(ScanHex(input, ref pos, 4));
                            continue;
                        case 'a':
                            sb.Append('\u0007');
                            pos++;
                            continue;
                        case 'b':
                            sb.Append('\b');
                            pos++;
                            continue;
                        case 'e':
                            sb.Append('\u001B');
                            pos++;
                            continue;
                        case 'f':
                            sb.Append('\f');
                            pos++;
                            continue;
                        case 'n':
                            sb.Append('\n');
                            pos++;
                            continue;
                        case 'r':
                            sb.Append('\r');
                            pos++;
                            continue;
                        case 't':
                            sb.Append('\t');
                            pos++;
                            continue;
                        case 'v':
                            sb.Append('\u000B');
                            pos++;
                            continue;
                        case 'c':
                            sb.Append(ScanControl(input, ref pos));
                            continue;
                        case '\\':
                            sb.Append('\\');
                            pos++;
                            continue;
                        default:
                            throw new ArgumentException("Invalid escape sequence!");
                    }
                }
                else
                {
                    sb.Append(input[pos]);
                    pos++;
                }                    
            }

            return sb.ToString();
        }
    }
}
