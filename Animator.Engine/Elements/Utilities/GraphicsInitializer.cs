using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements.Utilities
{
    public static class GraphicsInitializer
    {
        public static void Initialize(Graphics graphics)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.PixelOffsetMode = PixelOffsetMode.Half;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        }
    }
}
