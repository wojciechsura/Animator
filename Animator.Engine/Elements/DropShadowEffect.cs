using Animator.Engine.Base;
using Animator.Engine.Elements.Utilities;
using Animator.Engine.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// An effect, which adds a shadow to a visual.
    /// </summary>
    public class DropShadowEffect : Effect
    {
        internal override void Apply(BitmapBuffer framebuffer, BitmapBuffer backBuffer, BitmapBuffer frontBuffer, BitmapBufferRepository repository)
        {
            var frameData = framebuffer.Bitmap.LockBits(new System.Drawing.Rectangle(0, 0, framebuffer.Bitmap.Width, framebuffer.Bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var backData = backBuffer.Bitmap.LockBits(new System.Drawing.Rectangle(0, 0, frontBuffer.Bitmap.Width, frontBuffer.Bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            var colorArgb = (Color.A << 24) + (Color.R << 16) + (Color.G << 8) + Color.B;

            ImageProcessing.DropShadow(frameData.Scan0, 
                frameData.Stride, 
                backData.Scan0, 
                backData.Stride, 
                frameData.Width, 
                frameData.Height, 
                colorArgb,
                DX,
                DY,
                Radius);

            backBuffer.Bitmap.UnlockBits(backData);
            framebuffer.Bitmap.UnlockBits(frameData);
        }


        #region Color managed property

        /// <summary>
        /// Color of the shadow.
        /// </summary>
        public System.Drawing.Color Color
        {
            get => (System.Drawing.Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public static readonly ManagedProperty ColorProperty = ManagedProperty.Register(typeof(DropShadowEffect),
            nameof(Color),
            typeof(System.Drawing.Color),
            new ManagedSimplePropertyMetadata { DefaultValue = System.Drawing.Color.FromArgb(128, 0, 0, 0) });

        #endregion

        #region DX managed property

        /// <summary>
        /// Horizontal offset of the shadow.
        /// </summary>
        public int DX
        {
            get => (int)GetValue(DXProperty);
            set => SetValue(DXProperty, value);
        }

        public static readonly ManagedProperty DXProperty = ManagedProperty.Register(typeof(DropShadowEffect),
            nameof(DX),
            typeof(int),
            new ManagedSimplePropertyMetadata { DefaultValue = 5 });

        #endregion


        #region DY managed property

        /// <summary>
        /// Vertical offset of the shadow.
        /// </summary>
        public int DY
        {
            get => (int)GetValue(DYProperty);
            set => SetValue(DYProperty, value);
        }

        public static readonly ManagedProperty DYProperty = ManagedProperty.Register(typeof(DropShadowEffect),
            nameof(DY),
            typeof(int),
            new ManagedSimplePropertyMetadata { DefaultValue = 5 });

        #endregion

        #region Radius managed property

        /// <summary>
        /// Blur radius of the shadow.
        /// </summary>
        public int Radius
        {
            get => (int)GetValue(RadiusProperty);
            set => SetValue(RadiusProperty, value);
        }

        public static readonly ManagedProperty RadiusProperty = ManagedProperty.Register(typeof(DropShadowEffect),
            nameof(Radius),
            typeof(int),
            new ManagedSimplePropertyMetadata { DefaultValue = 5, CoerceValueHandler = CoerceRadius });

        private static object CoerceRadius(ManagedObject obj, object baseValue)
        {
            // Change to the next grater odd value
            return Math.Max(0, (int)baseValue);
        }

        #endregion
    }
}
