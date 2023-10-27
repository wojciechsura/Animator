using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements.Utilities
{
    public class BitmapBuffer : IDisposable
    {
        public BitmapBuffer(Bitmap bitmap, Graphics graphics, BitmapBufferRepository repository)
        {
            Bitmap = bitmap;
            Graphics = graphics;
            Repository = repository;
        }

        public void Dispose()
        {
            Graphics.Dispose();
        }

        public BitmapData Lock() =>
            Bitmap.LockBits(new System.Drawing.Rectangle(0, 0, Bitmap.Width, Bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

        public void Unlock(BitmapData data) =>
            Bitmap.UnlockBits(data);

        public Bitmap Bitmap { get; }
        public Graphics Graphics { get; }
        public BitmapBufferRepository Repository { get; }
    }
}
