using Animator.Engine.Base;
using Animator.Engine.Elements.Utilities;
using Animator.Engine.Exceptions;
using Svg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public class SvgImage : Visual
    {
        // Private fields -----------------------------------------------------

        private static readonly object svgLockObject = new();

        private SvgDocument cachedDocument;
        private string cachedDocumentPath;

        // Private methods ----------------------------------------------------

        private void UpdateDocumentCache()
        {
            if (string.IsNullOrEmpty(Source)) 
                throw new AnimationException("Source property of SvgImage is empty!", GetPath());

            var resolvedPath = ResolvePath(Source);

            if (!File.Exists(resolvedPath))
                throw new AnimationException($"SVG image {resolvedPath} does not exist!", GetPath());

            if (cachedDocumentPath != resolvedPath)
            {
                lock(svgLockObject)
                {
                    cachedDocument = SvgDocument.Open(resolvedPath);
                    cachedDocumentPath = resolvedPath;
                }
            }
        }

        // Protected methods --------------------------------------------------

        protected override void InternalRender(BitmapBuffer buffer, BitmapBufferRepository buffers)
        {
            UpdateDocumentCache();

            var renderer = SvgRenderer.FromGraphics(buffer.Graphics);
            cachedDocument.Draw(renderer);
        }

        // Managed properties -------------------------------------------------

        #region Source managed property

        /// <summary>
        /// Source of the SVG image
        /// </summary>
        public string Source
        {
            get => (string)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public static readonly ManagedProperty SourceProperty = ManagedProperty.Register(typeof(SvgImage),
            nameof(Source),
            typeof(string),
            new ManagedSimplePropertyMetadata { DefaultValue = string.Empty });

        #endregion
    }
}
