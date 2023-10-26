using Animator.Engine.Elements.Rendering;
using Animator.Engine.Elements.Utilities;
using Animator.Engine.Tools;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Animator.Engine.Elements.Compositing;
using System.Drawing.Drawing2D;
using Animator.Engine.Utils;

namespace Animator.Engine.Elements
{
    public partial class Scene
    {
        private static void ComposeItem(Stack<BitmapBuffer> backgrounds, CompositingItem item, BitmapBufferRepository buffers)
        {
            // Apply background effects (if any)
            if (item.Visual.BackgroundEffects.Any())
            {
                foreach (var background in backgrounds)
                    foreach (var backgroundEffect in item.Visual.BackgroundEffects)
                        backgroundEffect.Apply(background, item.Foreground, item.Mask, buffers);
            }

            // Merge item with topmost background
            var topBackground = backgrounds.Peek();

            var backgroundData = topBackground.Lock();
            var foregroundData = item.Foreground.Lock();

            ImageProcessing.CombineTwo(backgroundData.Scan0,
                backgroundData.Stride,
                foregroundData.Scan0,
                foregroundData.Stride,
                topBackground.Bitmap.Width,
                topBackground.Bitmap.Height);

            topBackground.Unlock(backgroundData);
            item.Foreground.Unlock(foregroundData);
        }

        private void ComposeNested(Stack<BitmapBuffer> backgrounds, NestedCompositingItem nested, Matrix originalTransform, BitmapBufferRepository buffers)
        {
            var buffer = buffers.Lease(originalTransform);
            try
            {
                backgrounds.Push(buffer);

                try
                {
                    nested.Visual.RenderComposited(originalTransform, (transform, buffers) =>
                    {
                        foreach (var compositingItem in nested.Items)
                        {
                            if (compositingItem is CompositingItem item)
                            {
                                ComposeItem(backgrounds, item, buffers);
                            }
                            else if (compositingItem is NestedCompositingItem recursiveNested)
                            {
                                ComposeNested(backgrounds, recursiveNested, transform, buffers);
                            }
                        }

                        return buffer;
                    }, buffers);

                }
                finally
                {
                    backgrounds.Pop();
                }

                var topBackground = backgrounds.Peek();

                var backgroundData = topBackground.Lock();
                var foregroundData = buffer.Lock();

                ImageProcessing.CombineTwo(backgroundData.Scan0,
                    backgroundData.Stride,
                    foregroundData.Scan0,
                    foregroundData.Stride,
                    topBackground.Bitmap.Width,
                    topBackground.Bitmap.Height);

                topBackground.Unlock(backgroundData);
                buffer.Unlock(foregroundData);
            }
            finally
            {
                buffers.Return(buffer);
            }
        }

        public void RenderWithCompositing(Bitmap bitmap, RenderingContext context = null)
        {
            using var bufferRepository = new BitmapBufferRepository(bitmap.Width, bitmap.Height, bitmap.PixelFormat);
            RenderWithCompositing(bitmap, bufferRepository, context);
        }

        public void RenderWithCompositing(Bitmap bitmap, BitmapBufferRepository buffers, RenderingContext context = null)
        {
            if (buffers.Width != bitmap.Width || buffers.Height != bitmap.Height || buffers.PixelFormat != bitmap.PixelFormat)
                throw new ArgumentException("Buffer repository bitmap parameters doesn't match output bitmap ones!", nameof(buffers));

            // Prepare bitmap
            using Graphics graphics = Graphics.FromImage(bitmap);
            GraphicsInitializer.Initialize(graphics);
            graphics.Clear(Color.Transparent);

            if (IsPropertySet(BackgroundProperty))
            {
                using var brush = Background.BuildBrush();
                graphics.FillRectangle(brush, new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height));
            }

            BitmapBuffer backgroundBuffer = new BitmapBuffer(bitmap, graphics);

            // Build compositing items

            List<BaseCompositingItem> compositingItems = new();
            Matrix originalTransform = new();

            foreach (var item in Items)
            {
                compositingItems.Add(item.BuildCompositingItem(originalTransform, buffers));
            }

            // Compose items

            Stack<BitmapBuffer> backgrounds = new();
            backgrounds.Push(backgroundBuffer);

            foreach (var compositingItem in compositingItems)
            {
                if (compositingItem is CompositingItem item)
                {
                    ComposeItem(backgrounds, item, buffers);
                }
                else if (compositingItem is NestedCompositingItem nested)
                {
                    ComposeNested(backgrounds, nested, originalTransform, buffers);
                }
            }

            backgrounds.Pop();

            if (backgrounds.Any())
                throw new InvalidOperationException("Backgrounds are not empty after processing!");

            // Return all leased buffers
            foreach (var compositingItem in compositingItems)
                compositingItem.Dispose();
        }
    }
}
