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
    public class GaussianBlurEffect : BaseEffect
    {
        internal override void Apply(BitmapBuffer framebuffer, BitmapBuffer overlayBuffer, BitmapBuffer underlayBuffer, BitmapBufferRepository repository)
        {
            var data = framebuffer.Bitmap.LockBits(new System.Drawing.Rectangle(0, 0, framebuffer.Bitmap.Width, framebuffer.Bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            ImageProcessing.GaussianBlur(data.Scan0, data.Stride, data.Width, data.Height, Radius);

            framebuffer.Bitmap.UnlockBits(data);
        }

        #region Radius managed property

        public int Radius
        {
            get => (int)GetValue(RadiusProperty);
            set => SetValue(RadiusProperty, value);
        }

        public static readonly ManagedProperty RadiusProperty = ManagedProperty.Register(typeof(GaussianBlurEffect),
            nameof(Radius),
            typeof(int),
            new ManagedSimplePropertyMetadata { DefaultValue = 5, CoerceValueHandler = CoerceRadius });

        private static object CoerceRadius(ManagedObject obj, object baseValue)
        {
            // Change to the next grater odd value
            return (int)baseValue + (1 - (int)baseValue % 2);
        }

        #endregion

    }
}
