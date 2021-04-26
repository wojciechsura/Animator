using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements.Utilities
{
    public class BufferRepository
    {
        private readonly int width;
        private readonly int height;
        private readonly PixelFormat pixelFormat;

        private readonly List<Bitmap> buffers = new();

        public BufferRepository(int width, int height, PixelFormat pixelFormat)
        {
            this.width = width;
            this.height = height;
            this.pixelFormat = pixelFormat;
        }

        public Bitmap Lease()
        {
            Bitmap buffer;

            if (buffers.Any())
            {
                buffer = buffers.Last();
                buffers.RemoveAt(buffers.Count - 1);
            }
            else
                buffer = new Bitmap(width, height, pixelFormat);

            using (Graphics g = Graphics.FromImage(buffer))
            {
                g.Clear(Color.Transparent);
            }

            return buffer;
        }

        public void Return(Bitmap buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (buffer.Width != width || buffer.Height != height || buffer.PixelFormat != pixelFormat)
                throw new ArgumentException("Invalid buffer!");

            buffers.Add(buffer);
        }

        public int Width => width;
        public int Height => height;
        public PixelFormat PixelFormat => pixelFormat;
    }
}
