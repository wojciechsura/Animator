using Animator.Engine.Base;
using Animator.Engine.Elements.Compositing;
using Animator.Engine.Elements.Rendering;
using Animator.Engine.Elements.Types;
using Animator.Engine.Elements.Utilities;
using Animator.Engine.Tools;
using Animator.Engine.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Represents basic element, which can be drawn on the scene.
    /// </summary>
    public abstract partial class Visual
    {
        internal virtual BaseCompositingItem BuildCompositingItem(Matrix originalTransform, BitmapBufferRepository buffers)
        {
            var transform = VisualRenderer.EvalCurrentTransform(originalTransform,
                Visible,
                Alpha,
                IsPropertySet(OriginProperty) ? Origin : null,
                IsPropertySet(RotationProperty) ? Rotation : null,
                IsPropertySet(ScaleProperty) ? Scale : null,
                Position);

            if (transform == null)
                return null;

            var buffer = buffers.Lease(originalTransform);
            buffer.Graphics.Transform = transform;

            InternalRender(buffer, buffers, null);

            // Build mask

            BitmapBuffer maskBuffer = null;

            if (Mask.Any())
                maskBuffer = VisualRenderer.BuildMaskBuffer(Mask, 
                    MaskCoordinateSystem == MaskCoordinateSystem.Local ? transform : originalTransform, 
                    buffers, 
                    null);

            // Note: buffer and maskBuffer were not returned.
            // They will get returned when compositing item
            // gets disposed.

            return new VisualCompositingItem(buffer, maskBuffer, this, buffers);
        }

        internal void RenderComposited(Matrix originalTransform, Func<Matrix, BitmapBufferRepository, BitmapBuffer> renderFunc, BitmapBufferRepository buffers)
        {
            var transform = VisualRenderer.EvalCurrentTransform(originalTransform,
                Visible,
                Alpha,
                IsPropertySet(OriginProperty) ? Origin : null,
                IsPropertySet(RotationProperty) ? Rotation : null,
                IsPropertySet(ScaleProperty) ? Scale : null,
                Position);

            if (transform == null) 
                return;

            var buffer = renderFunc(transform, buffers);

            if (IsPropertySet(AlphaProperty))
                VisualRenderer.ApplyAlpha(Alpha, buffer);

            VisualRenderer.ApplyEffects(buffer, buffers, Effects);

            VisualRenderer.ApplyMask(buffer, originalTransform, transform, Mask, MaskCoordinateSystem, InvertMask, null, buffers);
        }

        #region BackgroundEffects managed collection

        /// <summary>
        /// List of effects, which will be applied to
        /// background underneath a visual after its rendering
        /// and before it is composited with the background.
        /// Requires <b>composite rendering</b>.
        /// </summary>
        public ManagedCollection<BackgroundEffect> BackgroundEffects
        {
            get => (ManagedCollection<BackgroundEffect>)GetValue(BackgroundEffectsProperty);
        }

        public static readonly ManagedProperty BackgroundEffectsProperty = ManagedProperty.RegisterCollection(typeof(Visual),
            nameof(BackgroundEffects),
            typeof(ManagedCollection<BackgroundEffect>));

        #endregion
    }
}
