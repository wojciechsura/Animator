using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements.Utilities
{
    public class BitmapBuffer : IDisposable
    {
        public BitmapBuffer(Bitmap bitmap, Graphics graphics)
        {
            Bitmap = bitmap;
            Graphics = graphics;
        }

        public void Dispose()
        {
            Graphics.Dispose();
        }

        public Bitmap Bitmap { get; }
        public Graphics Graphics { get; }
    }
}
