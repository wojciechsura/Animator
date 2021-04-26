using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements.Utilities
{
    public class BitmapBufferRepository : IDisposable
    {
        private readonly int width;
        private readonly int height;
        private readonly PixelFormat pixelFormat;

        private readonly List<BitmapBuffer> buffers = new();

        public BitmapBufferRepository(int width, int height, PixelFormat pixelFormat)
        {
            this.width = width;
            this.height = height;
            this.pixelFormat = pixelFormat;
        }

        public BitmapBuffer Lease(Matrix baseTransform)
        {
            BitmapBuffer buffer;

            if (buffers.Any())
            {
                buffer = buffers.Last();
                buffers.RemoveAt(buffers.Count - 1);
            }
            else
            {
                var bitmap = new Bitmap(width, height, pixelFormat);
                var graphics = Graphics.FromImage(bitmap);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;

                buffer = new BitmapBuffer(bitmap, graphics);
            }

            buffer.Graphics.Transform = baseTransform;

            return buffer;
        }

        public void Return(BitmapBuffer buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (buffer.Bitmap.Width != width || buffer.Bitmap.Height != height || buffer.Bitmap.PixelFormat != pixelFormat)
                throw new ArgumentException("Invalid buffer!");

            buffers.Add(buffer);
        }

        public void Dispose()
        {
            while (buffers.Any())
            {
                buffers.Last().Dispose();
                buffers.RemoveAt(buffers.Count - 1);
            }
        }

        public int Width => width;
        public int Height => height;
        public PixelFormat PixelFormat => pixelFormat;
    }
}
