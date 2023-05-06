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
    /// <summary>Add a scanline effect to an image</summary>
    public class ScanlinesEffect : Effect
    {
        internal override void Apply(BitmapBuffer framebuffer, BitmapBuffer backBuffer, BitmapBuffer frontBuffer, BitmapBufferRepository repository)
        {
            var data = framebuffer.Bitmap.LockBits(new System.Drawing.Rectangle(0, 0, framebuffer.Bitmap.Width, framebuffer.Bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            ImageProcessing.Scanlines(data.Scan0, data.Stride, data.Width, data.Height, LineHeight, DarkenLevel);

            framebuffer.Bitmap.UnlockBits(data);
        }

        #region LineHeight managed property

        /// <summary>
        /// Height of a single scanline
        /// </summary>
        public int LineHeight
        {
            get => (int)GetValue(LineHeightProperty);
            set => SetValue(LineHeightProperty, value);
        }

        public static readonly ManagedProperty LineHeightProperty = ManagedProperty.Register(typeof(ScanlinesEffect),
            nameof(LineHeight),
            typeof(int),
            new ManagedSimplePropertyMetadata { DefaultValue = 2, CoerceValueHandler = CoerceLineHeight });

        private static object CoerceLineHeight(ManagedObject obj, object baseValue)
        {
            return Math.Max(1, (int)baseValue);
        }

        #endregion

        #region DarkenLevel managed property

        public int DarkenLevel
        {
            get => (int)GetValue(DarkenLevelProperty);
            set => SetValue(DarkenLevelProperty, value);
        }

        public static readonly ManagedProperty DarkenLevelProperty = ManagedProperty.Register(typeof(ScanlinesEffect),
            nameof(DarkenLevel),
            typeof(int),
            new ManagedSimplePropertyMetadata { DefaultValue = 10, CoerceValueHandler = CoerceDarkenLevel });

        private static object CoerceDarkenLevel(ManagedObject obj, object baseValue)
        {
            return Math.Min(255, Math.Max(1, (int)baseValue));
        }

        #endregion
    }
}
