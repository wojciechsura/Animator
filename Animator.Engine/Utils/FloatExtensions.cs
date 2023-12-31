﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Utils
{
    public static class FloatExtensions
    {
        private const float EPSILON = 0.0000000001f;

        public static bool IsZero(this float value) => Math.Abs(value) < EPSILON;

        public static string ToMoneyString(this float f, string format = null)
        {
            char[] incPrefixes = new[] { 'k', 'M', 'B', 'T' };

            // No prefix in this case
            if (Math.Abs(f) >= 0 && Math.Abs(f) < 1000)
                return f.ToString(format);

            int degree = (int)Math.Floor(Math.Log10(Math.Abs(f)) / 3);
            if (degree >= incPrefixes.Length)
                degree = incPrefixes.Length;
            double scaled = f * Math.Pow(1000, -degree);

            char? prefix = incPrefixes[degree - 1];
            return scaled.ToString(format) + prefix;
        }

        public static string ToSIString(this float f, string format = null)
        {
            char[] incPrefixes = new[] { 'k', 'M', 'G', 'T', 'P', 'E', 'Z', 'Y' };
            char[] decPrefixes = new[] { 'm', '\u03bc', 'n', 'p', 'f', 'a', 'z', 'y' };

            // No prefix in this case
            if (f >= 0 && f < 1000)
                return f.ToString(format);

            int degree = (int)Math.Floor(Math.Log10(Math.Abs(f)) / 3);
            switch (Math.Sign(degree))
            {
                case 1: 
                    if (degree >= incPrefixes.Length)
                        degree = incPrefixes.Length - 1;
                    break;
                case -1:
                    if (degree >= decPrefixes.Length) 
                        degree = decPrefixes.Length - 1;
                    break;
            }
            double scaled = f * Math.Pow(1000, -degree);

            char? prefix = null;
            switch (Math.Sign(degree))
            {
                case 1: prefix = incPrefixes[degree - 1]; break;
                case -1: prefix = decPrefixes[-degree - 1]; break;
            }

            return scaled.ToString(format) + prefix;
        }
    }
}
