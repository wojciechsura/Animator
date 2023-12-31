﻿using Animator.Engine.Base;
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
    /// Adds a nice, Gaussian blur to a visual.
    /// </summary>
    public partial class GaussianBlurEffect : Effect
    {
        internal override void Apply(BitmapBuffer framebuffer, BitmapBuffer backBuffer, BitmapBuffer frontBuffer, BitmapBufferRepository repository)
        {
            var data = framebuffer.Lock();

            ImageProcessing.GaussianBlur(data.Scan0, 
                data.Stride, 
                data.Width, 
                data.Height, 
                Radius);

            framebuffer.Unlock(data);
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

        public static readonly ManagedProperty RadiusProperty = ManagedProperty.Register(typeof(GaussianBlurEffect),
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
