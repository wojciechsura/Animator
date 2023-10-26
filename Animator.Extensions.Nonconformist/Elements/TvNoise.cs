using Animator.Engine.Base;
using Animator.Engine.Elements;
using Animator.Engine.Elements.Utilities;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Animator.Engine.Elements.Rendering;

namespace Animator.Extensions.Nonconformist.Elements
{
    public class TvNoise : Visual
    {
        protected override void InternalRender(BitmapBuffer buffer,
            BitmapBufferRepository buffers, RenderingContext context)
        {
            var temp = buffers.Lease(new System.Drawing.Drawing2D.Matrix());

            int width = Math.Min(temp.Bitmap.Width, Width);
            int height = Math.Min(temp.Bitmap.Height, Height);

            Random random = new Random();   

            for (int y = 0; y < height; y++)
            {
                var bits = temp.Lock();

                var bytes = new byte[width * 4];
                for (int i = 0; i < width; i++)
                {
                    var value = (byte)random.Next(256);
                    bytes[i * 4] = value;
                    bytes[i * 4 + 1] = value;
                    bytes[i * 4 + 2] = value;
                    bytes[i * 4 + 3] = 255; // Alpha
                }

                Marshal.Copy(bytes, 0, bits.Scan0, bytes.Length);

                temp.Unlock(bits);
            }

            buffer.Graphics.DrawImage(temp.Bitmap, 
                new System.Drawing.RectangleF(0, 0, width, height), 
                new System.Drawing.RectangleF(0, 0, width, height),
                GraphicsUnit.Pixel);
            buffers.Return(temp);
        }

        #region Width managed property

        public int Width
        {
            get => (int)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }

        public static readonly ManagedProperty WidthProperty = ManagedProperty.Register(typeof(TvNoise),
            nameof(Width),
            typeof(int),
            new ManagedSimplePropertyMetadata { DefaultValue = 0 });

        #endregion

        #region Height managed property

        public int Height
        {
            get => (int)GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }

        public static readonly ManagedProperty HeightProperty = ManagedProperty.Register(typeof(TvNoise),
            nameof(Height),
            typeof(int),
            new ManagedSimplePropertyMetadata { DefaultValue = 0 });

        #endregion
    }
}
