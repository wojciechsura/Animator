using Animator.Engine.Base;
using Animator.Engine.Elements.Types;
using Animator.Engine.Elements.Utilities;
using Animator.Engine.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public class Image : Visual
    {
        // Private fields -----------------------------------------------------

        private static readonly object bitmapLockObject = new();

        private Bitmap cachedImage;
        private string cachedImagePath;

        // Private methods ----------------------------------------------------

        private void UpdateImageCache()
        {
            if (String.IsNullOrEmpty(Source))
                throw new AnimationException("Source property of Image is empty!", GetPath());
            if (!File.Exists(Source))
                throw new AnimationException($"Image {Source} does not exist!", GetPath());

            if (cachedImagePath != Source)
            {
                lock (bitmapLockObject)
                {
                    var bytes = File.ReadAllBytes(Source);
                    var ms = new MemoryStream(bytes);
                    cachedImage = (Bitmap)System.Drawing.Image.FromStream(ms);
                    cachedImagePath = Source;
                }
            }
        }

        // Protected methods --------------------------------------------------

        protected override void InternalRender(BitmapBuffer buffer, BitmapBufferRepository buffers)
        {
            UpdateImageCache();

            float y = VerticalAlignment switch
            {
                VerticalAlignment.Top => 0.0f,
                VerticalAlignment.Center => -cachedImage.Height / 2.0f,
                VerticalAlignment.Bottom => -cachedImage.Height,
                _ => throw new InvalidOperationException("Invalid vertical alignment")
            };

            float x = HorizontalAlignment switch
            {
                HorizontalAlignment.Left => 0.0f,
                HorizontalAlignment.Center => -cachedImage.Width / 2.0f,
                HorizontalAlignment.Right => -cachedImage.Width,
                _ => throw new InvalidOperationException("Invalid horizontal alignment!")
            };

            buffer.Graphics.DrawImage(cachedImage, new RectangleF(x, y, cachedImage.Width, cachedImage.Height));
        }

        // Public properties --------------------------------------------------

        #region Source managed property

        public string Source
        {
            get => (string)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public static readonly ManagedProperty SourceProperty = ManagedProperty.Register(typeof(Image),
            nameof(Source),
            typeof(string),
            new ManagedSimplePropertyMetadata { DefaultValue = null });

        #endregion

        #region HorizontalAlignment

        public HorizontalAlignment HorizontalAlignment
        {
            get => (HorizontalAlignment)GetValue(HorizontalAlignmentProperty);
            set => SetValue(HorizontalAlignmentProperty, value);
        }

        public static readonly ManagedProperty HorizontalAlignmentProperty = ManagedProperty.Register(typeof(Image),
            nameof(HorizontalAlignment),
            typeof(HorizontalAlignment),
            new ManagedSimplePropertyMetadata { DefaultValue = HorizontalAlignment.Left });

        #endregion

        #region VerticalAlignment

        public VerticalAlignment VerticalAlignment
        {
            get => (VerticalAlignment)GetValue(VerticalAlignmentProperty);
            set => SetValue(VerticalAlignmentProperty, value);
        }

        public static readonly ManagedProperty VerticalAlignmentProperty = ManagedProperty.Register(typeof(Image),
            nameof(VerticalAlignment),
            typeof(VerticalAlignment),
            new ManagedSimplePropertyMetadata { DefaultValue = VerticalAlignment.Top });

        #endregion
    }
}
