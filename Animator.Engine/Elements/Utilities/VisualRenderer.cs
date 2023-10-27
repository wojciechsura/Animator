using Animator.Engine.Elements.Rendering;
using Animator.Engine.Elements.Types;
using Animator.Engine.Tools;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Animator.Engine.Utils;

namespace Animator.Engine.Elements.Utilities
{
    internal static class VisualRenderer
    {
        private static Matrix BuildTransformMatrix(PointF? Origin, float? Rotation, PointF? Scale, PointF Position)
        {
            var result = new Matrix();

            if (Origin != null)
                result.Translate(-Origin.Value.X, -Origin.Value.Y, MatrixOrder.Append);

            if (Rotation != null)
                result.Rotate(Rotation.Value, MatrixOrder.Append);

            if (Scale != null)
                result.Scale(Scale.Value.X, Scale.Value.Y, MatrixOrder.Append);

            result.Translate(Position.X, Position.Y, MatrixOrder.Append);

            return result;
        }

        public static void ApplyAlpha(float alpha, BitmapBuffer image)
        {
            var data = image.Lock();
            ImageProcessing.ApplyAlpha(data.Scan0, data.Stride, data.Width, data.Height, (byte)(alpha * 255.0f));
            image.Unlock(data);
        }

        public static void ApplyEffects(BitmapBuffer buffer, BitmapBufferRepository buffers, IList<Effect> effects)
        {
            if (effects.Any())
            {
                var backBuffer = buffers.Lease(new Matrix());
                var frontBuffer = buffers.Lease(new Matrix());
                var frameBuffer = buffers.Lease(new Matrix());

                try
                {
                    CopyBitmap(buffer.Bitmap, frameBuffer.Bitmap);

                    foreach (var effect in effects)
                        effect.Apply(frameBuffer, backBuffer, frontBuffer, buffers);

                    // Join buffers

                    var backBufferData = backBuffer.Lock();
                    var frameBufferData = frameBuffer.Lock();
                    var frontBufferData = frontBuffer.Lock();
                    var bufferData = buffer.Lock();

                    ImageProcessing.CombineThree(backBufferData.Scan0, backBufferData.Stride, frameBufferData.Scan0, frameBufferData.Stride, frontBufferData.Scan0, frontBufferData.Stride, backBuffer.Bitmap.Width, backBuffer.Bitmap.Height);
                    ImageProcessing.CopyData(backBufferData.Scan0, backBufferData.Stride, bufferData.Scan0, bufferData.Stride, frameBuffer.Bitmap.Width, frameBuffer.Bitmap.Height);

                    backBuffer.Unlock(backBufferData);
                    frameBuffer.Unlock(frameBufferData);
                    frontBuffer.Unlock(frontBufferData);
                    buffer.Unlock(bufferData);
                }
                finally
                {
                    buffers.Return(frameBuffer);
                    buffers.Return(frontBuffer);
                    buffers.Return(backBuffer);
                }
            }
        }

        public static void ApplyMask(BitmapBuffer buffer, Matrix originalTransform, Matrix transform, IList<Visual> mask, MaskCoordinateSystem maskCoordinateSystem, bool invertMask, RenderingContext context, BitmapBufferRepository buffers)
        {
            if (mask.Any())
            {
                BitmapBuffer maskBuffer = BuildMaskBuffer(mask, maskCoordinateSystem == MaskCoordinateSystem.Local ? transform : originalTransform, buffers, context);
                try
                { 
                    ApplyMask(buffer, maskBuffer, invertMask);
                }
                finally
                {
                    buffers.Return(maskBuffer);
                }
            }
        }

        public static void ApplyMask(BitmapBuffer buffer, BitmapBuffer maskBuffer, bool invertMask)
        {
            var maskData = maskBuffer.Lock();
            var bufferData = buffer.Lock();

            ImageProcessing.ApplyMask(bufferData.Scan0, bufferData.Stride, maskData.Scan0, maskData.Stride, buffer.Bitmap.Width, buffer.Bitmap.Height, invertMask);

            buffer.Unlock(bufferData);
            maskBuffer.Unlock(maskData);
        }

        public static Matrix EvalCurrentTransform(Matrix originalTransform, bool visible, float alpha, PointF? origin, float? rotation, PointF? scale, PointF position)
        {
            if (!visible || alpha.IsZero())
                return null;

            var transform = originalTransform.Clone();
            transform.Multiply(VisualRenderer.BuildTransformMatrix(origin, rotation, scale, position), MatrixOrder.Prepend);

            // Scale eqaual to 0 equals to invisible object
            if (Math.Abs(transform.MatrixElements.M11).IsZero() &&
                Math.Abs(transform.MatrixElements.M12).IsZero() &&
                Math.Abs(transform.MatrixElements.M21).IsZero() &&
                Math.Abs(transform.MatrixElements.M22).IsZero())
                return null;

            return transform;
        }

        /// <remarks>
        /// The returned buffer is leased from given bitmap
        /// buffer repository. Caller is responsible for
        /// returning the buffer.
        /// </remarks>
        public static BitmapBuffer BuildMaskBuffer(IList<Visual> mask,
            Matrix transform,
            BitmapBufferRepository buffers, 
            RenderingContext context)
        {
            var itemBuffer = buffers.Lease(transform);
            try
            {
                var maskBuffer = buffers.Lease(new Matrix());
                var maskData = maskBuffer.Lock();
                foreach (var item in mask)
                {
                    itemBuffer.Graphics.Clear(Color.Transparent);
                    item.Render(itemBuffer, buffers, context);

                    var itemData = itemBuffer.Lock();
                    ImageProcessing.CombineTwo(maskData.Scan0, maskData.Stride, itemData.Scan0, itemData.Stride, maskBuffer.Bitmap.Width, maskBuffer.Bitmap.Height);
                    itemBuffer.Unlock(itemData);
                }

                maskBuffer.Unlock(maskData);

                return maskBuffer;
            }
            finally
            {
                buffers.Return(itemBuffer);
            }
        }

        public static void CopyBitmap(Bitmap source, Bitmap destination)
        {
            if (source.Width != destination.Width ||
                source.Height != destination.Height)
                throw new ArgumentException("Invalid destination bitmap size!", nameof(destination));

            if (source.PixelFormat != PixelFormat.Format32bppArgb)
                throw new ArgumentException("Invalid source pixel format!", nameof(source));

            if (destination.PixelFormat != PixelFormat.Format32bppArgb)
                throw new ArgumentException("Invalid destination pixel format!", nameof(destination));

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
    }
}
