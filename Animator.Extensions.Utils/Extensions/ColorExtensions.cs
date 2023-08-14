using Animator.Extensions.Utils.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Extensions.Utils.Extensions
{
    public static class ColorExtensions
    {
        public static Color WithAlpha(this System.Drawing.Color color, byte alpha)
        {
            return Color.FromArgb(alpha, color.R, color.G, color.B);
        }

        public static Color BrightenBy(this System.Drawing.Color color, float factor) 
        {
            ColorHsl hsl = color.ToHsl();
            hsl.L = Math.Min(1.0f, hsl.L + factor);
            return hsl.ToColor();
        }

        public static Color DarkenBy(this System.Drawing.Color color, float factor)
        {
            ColorHsl hsl = color.ToHsl();
            hsl.L = Math.Max(0.0f, hsl.L - factor);
            return hsl.ToColor();
        }

        public static Color SaturateBy(this System.Drawing.Color color, float factor)
        {
            ColorHsl hsl = color.ToHsl();
            hsl.S = Math.Min(1.0f, hsl.S + factor);
            return hsl.ToColor();
        }

        public static Color DesaturateBy(this System.Drawing.Color color, float factor)
        {
            ColorHsl hsl = color.ToHsl();
            hsl.S = Math.Max(0.0f, hsl.S - factor);
            return hsl.ToColor();
        }

        public static Color ToColor(this ColorHsl hsl)
        {
            float v;
            float r, g, b;

            r = hsl.L;   // default to gray
            g = hsl.L;
            b = hsl.L;
            v = (hsl.L <= 0.5) ? (hsl.L * (1.0f + hsl.S)) : (hsl.L + hsl.S - hsl.L * hsl.S);
            if (v > 0)
            {
                float m;
                float sv;
                int sextant;
                float fract, vsf, mid1, mid2;

                m = hsl.L + hsl.L - v;
                sv = (v - m) / v;
                hsl.H *= 6.0f;
                sextant = (int)hsl.H;
                fract = hsl.H - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;

                switch (sextant)
                {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;

                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;

                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;

                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;

                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;

                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;
                }
            }

            return Color.FromArgb(Convert.ToByte(r * 255.0f), Convert.ToByte(g * 255.0f), Convert.ToByte(b * 255.0f));
        }

        // Given a Color (RGB Struct) in range of 0-255
        // Return H,S,L in range of 0-1
        public static ColorHsl ToHsl(this Color color)
        {
            ColorHsl hsl;

            float r = color.R / 255.0f;
            float g = color.G / 255.0f;
            float b = color.B / 255.0f;
            float v;
            float m;
            float vm;
            float r2, g2, b2;

            hsl.H = 0; // default to black
            hsl.S = 0;
            hsl.L = 0;
            v = Math.Max(r, g);
            v = Math.Max(v, b);
            m = Math.Min(r, g);
            m = Math.Min(m, b);
            hsl.L = (m + v) / 2.0f;

            if (hsl.L <= 0.0)
            {
                return hsl;
            }

            vm = v - m;
            hsl.S = vm;

            if (hsl.S > 0.0)
            {
                hsl.S /= (hsl.L <= 0.5f) ? (v + m) : (2.0f - v - m);
            }
            else
            {
                return hsl;
            }

            r2 = (v - r) / vm;
            g2 = (v - g) / vm;
            b2 = (v - b) / vm;

            if (r == v)
            {
                hsl.H = (g == m ? 5.0f + b2 : 1.0f - g2);
            }
            else if (g == v)
            {
                hsl.H = (b == m ? 1.0f + r2 : 3.0f - b2);
            }
            else
            {
                hsl.H = (r == m ? 3.0f + g2 : 5.0f - r2);
            }

            hsl.H /= 6.0f;

            return hsl;
        }
    }
}
