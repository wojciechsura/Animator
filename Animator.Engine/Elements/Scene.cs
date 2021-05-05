using Animator.Engine.Base;
using Animator.Engine.Base.Persistence;
using Animator.Engine.Elements.Utilities;
using Animator.Engine.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Contains description of a single scene in animation.
    /// </summary>
    [ContentProperty(nameof(Items))]
    public class Scene : BaseElement
    {
        // Public methods -----------------------------------------------------

        public void Render(Bitmap bitmap)
        {
            using var bufferRepository = new BitmapBufferRepository(bitmap.Width, bitmap.Height, bitmap.PixelFormat);
            Render(bitmap, bufferRepository);
        }

        public void Render(Bitmap bitmap, BitmapBufferRepository buffers)
        {
            if (buffers.Width != bitmap.Width || buffers.Height != bitmap.Height || buffers.PixelFormat != bitmap.PixelFormat)
                throw new ArgumentException(nameof(buffers), "Buffer repository bitmap parameters doesn't match output bitmap ones!");

            // Prepare bitmap
            using Graphics graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphics.Clear(Color.Transparent);

            if (IsPropertySet(BackgroundProperty))
            {
                using var brush = Background.BuildBrush();
                graphics.FillRectangle(brush, new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height));
            }

            BitmapBuffer buffer = buffers.Lease(graphics.Transform);
            
            try
            {
                var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                foreach (var item in Items)
                {
                    buffer.Graphics.Clear(Color.Transparent);
                    item.Render(buffer, buffers);

                    var bufferData = buffer.Bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                    ImageProcessing.CombineTwo(bitmapData.Scan0, bitmapData.Stride, bufferData.Scan0, bufferData.Stride, bitmap.Width, bitmap.Height);
                    buffer.Bitmap.UnlockBits(bufferData);
                }

                bitmap.UnlockBits(bitmapData);
            }
            finally
            {
                buffers.Return(buffer);
            }
        }

        // Public properties --------------------------------------------------

        #region Duration managed property

        /// <summary>
        /// Defines duration of the scene.
        /// </summary>
        public TimeSpan Duration
        {
            get => (TimeSpan)GetValue(DurationProperty);
            set => SetValue(DurationProperty, value);
        }

        public static readonly ManagedProperty DurationProperty = ManagedProperty.Register(typeof(Scene),
            nameof(Duration),
            typeof(TimeSpan),
            new ManagedSimplePropertyMetadata { DefaultValue = TimeSpan.FromSeconds(10) });

        #endregion

        #region Items managed collection

        /// <summary>
        /// Contains all visual elements placed on the scene.
        /// </summary>
        public ManagedCollection<Visual> Items
        {
            get => (ManagedCollection<Visual>)GetValue(ItemsProperty);
        }

        public static readonly ManagedProperty ItemsProperty = ManagedProperty.RegisterCollection(typeof(Scene),
            nameof(Items),
            typeof(ManagedCollection<Visual>));

        #endregion

        #region Background managed property

        /// <summary>
        /// Defines fill of the background of the scene.
        /// </summary>
        public Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        public static readonly ManagedProperty BackgroundProperty = ManagedProperty.RegisterReference(typeof(Scene),
            nameof(Background),
            typeof(Brush));

        #endregion
    }
}
