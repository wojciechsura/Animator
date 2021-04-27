using Animator.Engine.Base;
using Animator.Engine.Elements.Utilities;
using Animator.Engine.Tools;
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
    public abstract class Visual : BaseElement
    {
        // Private methods ----------------------------------------------------

        private void ApplyAlpha(float alpha, Bitmap image)
        {
            var data = image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            ImageProcessing.ApplyAlpha(data.Scan0, data.Stride, data.Width, data.Height, alpha);

            image.UnlockBits(data);
        }

        private void CopyBitmap(Bitmap source, Bitmap destination)
        {
            if (source.Width != destination.Width ||
                source.Height != destination.Height)
                throw new ArgumentException(nameof(destination), "Invalid destination bitmap size!");

            if (source.PixelFormat != PixelFormat.Format32bppArgb)
                throw new ArgumentException(nameof(source), "Invalid source pixel format!");

            if (destination.PixelFormat != PixelFormat.Format32bppArgb)
                throw new ArgumentException(nameof(destination), "Invalid destination pixel format!");

            BitmapData sourceData = source.LockBits(new System.Drawing.Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData destinationData = destination.LockBits(new System.Drawing.Rectangle(0, 0, destination.Width, destination.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            try
            {
                ImageProcessing.CopyData(sourceData.Scan0, sourceData.Stride, destinationData.Scan0, destinationData.Stride, source.Width, source.Height);
            }
            finally
            {
                source.UnlockBits(sourceData);
                destination.UnlockBits(destinationData);
            }
        }

        // Protected methods --------------------------------------------------

        protected abstract void InternalRender(BitmapBuffer buffer, BitmapBufferRepository buffers);

        protected Matrix BuildTransformMatrix()
        {
            var result = new Matrix();

            if (IsPropertySet(OriginProperty))
                result.Translate(-Origin.X, -Origin.Y, MatrixOrder.Append);

            if (IsPropertySet(RotationProperty))
                result.Rotate(Rotation, MatrixOrder.Append);

            if (IsPropertySet(ScaleProperty))
                result.Scale(Scale.X, Scale.Y, MatrixOrder.Append);

            result.Translate(Position.X, Position.Y, MatrixOrder.Append);

            return result;
        }

        // Internal methods ---------------------------------------------------

        internal void Render(BitmapBuffer buffer, BitmapBufferRepository buffers)
        {
            var originalTransform = buffer.Graphics.Transform;

            var transform = originalTransform.Clone();
            transform.Multiply(BuildTransformMatrix(), MatrixOrder.Prepend);
            buffer.Graphics.Transform = transform;

            InternalRender(buffer, buffers);

            if (Effects.Any())
            {
                var backBuffer = buffers.Lease(new Matrix());
                backBuffer.Graphics.Clear(Color.Transparent);

                var frontBuffer = buffers.Lease(new Matrix());
                frontBuffer.Graphics.Clear(Color.Transparent);

                var frameBuffer = buffers.Lease(new Matrix());
                frameBuffer.Graphics.Clear(Color.Transparent);
                try
                {
                    CopyBitmap(buffer.Bitmap, frameBuffer.Bitmap);

                    foreach (var effect in Effects)
                        effect.Apply(frameBuffer, backBuffer, frontBuffer, buffers);

                    // Join buffers

                    backBuffer.Graphics.DrawImage(frameBuffer.Bitmap, new Point(0, 0));
                    backBuffer.Graphics.DrawImage(frontBuffer.Bitmap, new Point(0, 0));

                    CopyBitmap(backBuffer.Bitmap, buffer.Bitmap);
                }
                finally
                {
                    buffers.Return(frameBuffer);
                    buffers.Return(frontBuffer);
                    buffers.Return(backBuffer);
                }
            }

            if (IsPropertySet(AlphaProperty))
                ApplyAlpha(Alpha, buffer.Bitmap);

            buffer.Graphics.Transform = originalTransform;
        }

        // Public properties --------------------------------------------------

        #region Position managed property

        public PointF Position
        {
            get => (PointF)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        public static readonly ManagedProperty PositionProperty = ManagedProperty.Register(typeof(Visual),
            nameof(Position),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(0.0f, 0.0f) });

        #endregion

        #region Origin managed property

        public PointF Origin
        {
            get => (PointF)GetValue(OriginProperty);
            set => SetValue(OriginProperty, value);
        }

        public static readonly ManagedProperty OriginProperty = ManagedProperty.Register(typeof(Visual),
            nameof(Origin),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(0.0f, 0.0f) });

        #endregion

        #region Rotation managed property

        public float Rotation
        {
            get => (float)GetValue(RotationProperty);
            set => SetValue(RotationProperty, value);
        }

        public static readonly ManagedProperty RotationProperty = ManagedProperty.Register(typeof(Visual),
            nameof(Rotation),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 0.0f });

        #endregion

        #region Scale managed property

        public PointF Scale
        {
            get => (PointF)GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }

        public static readonly ManagedProperty ScaleProperty = ManagedProperty.Register(typeof(Visual),
            nameof(Scale),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(1.0f, 1.0f) });

        #endregion

        #region Alpha managed property

        public float Alpha
        {
            get => (float)GetValue(AlphaProperty);
            set => SetValue(AlphaProperty, value);
        }

        public static readonly ManagedProperty AlphaProperty = ManagedProperty.Register(typeof(Visual),
            nameof(Alpha),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 1.0f });

        #endregion

        #region Effects managed collection

        public ManagedCollection<BaseEffect> Effects
        {
            get => (ManagedCollection<BaseEffect>)GetValue(EffectsProperty);
        }

        public static readonly ManagedProperty EffectsProperty = ManagedProperty.RegisterCollection(typeof(Visual),
            nameof(Effects),
            typeof(ManagedCollection<BaseEffect>));

        #endregion
    }
}
