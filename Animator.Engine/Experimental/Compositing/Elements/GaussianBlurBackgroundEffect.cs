using Animator.Engine.Base;
using Animator.Engine.Elements.Types;
using Animator.Engine.Elements.Utilities;
using Animator.Engine.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Adds a nice, Gaussian blur to background below
    /// a visual. Requires <b>composite rendering</b>.
    /// </summary>
    public class GaussianBlurBackgroundEffect : BackgroundEffect
    {
        protected override void InternalApply(BitmapBuffer background, BitmapBuffer mask, bool useAlpha, BitmapBufferRepository buffers)
        {
            var backgroundData = background.Lock();
            var maskData = mask.Lock();

            ImageProcessing.MaskedGaussianBlur(backgroundData.Scan0,
                backgroundData.Stride,
                maskData.Scan0,
                maskData.Stride,
                background.Bitmap.Width,
                background.Bitmap.Height,
                Radius,
                useAlpha);

            background.Unlock(backgroundData);
            mask.Unlock(maskData);
        }        

        #region Radius managed property

        /// <summary>
        /// Radius of the blur.
        /// </summary>
        public int Radius
        {
            get => (int)GetValue(RadiusProperty);
            set => SetValue(RadiusProperty, value);
        }

        public static readonly ManagedProperty RadiusProperty = ManagedProperty.Register(typeof(GaussianBlurBackgroundEffect),
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
