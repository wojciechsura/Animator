using Animator.Engine.Base;
using Animator.Engine.Elements.Utilities;
using Animator.Engine.Exceptions;
using System;
using System.Collections.Generic;
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
                cachedImage = new Bitmap(Source);
            }
        }

        // Protected methods --------------------------------------------------

        protected override void InternalRender(BitmapBuffer buffer, BitmapBufferRepository buffers)
        {
            UpdateImageCache();

            buffer.Graphics.DrawImage(cachedImage, new PointF(0.0f, 0.0f));
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
    }
}
