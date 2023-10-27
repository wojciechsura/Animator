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
using Animator.Engine.Base;
using Animator.Engine.Elements.Types;

namespace Animator.Engine.Elements
{
    public partial class Scene
    {
        private void Compose(Stack<BitmapBuffer> backgrounds, BaseCompositingItem item, Matrix originalTransform, BitmapBufferRepository buffers)
        {
            var currentItemBuffer = buffers.Lease(originalTransform);
            try
            {
                backgrounds.Push(currentItemBuffer);

                if (item is LayerCompositingItem nested)
                {
                    // Evaluating foreground of layer is deferred, because
                    // it is needed to apply all effects to evaluate it
                    // (both foreground and background ones)

                    nested.Visual.RenderComposited(originalTransform, (transform, buffers) =>
                    {
                        foreach (var compositingItem in nested.Items)
                            Compose(backgrounds, compositingItem, transform, buffers);

                        return currentItemBuffer;
                    }, buffers);
                }
                else if (item is VisualCompositingItem compositingItem)
                {
                    compositingItem.Visual.RenderComposited(originalTransform, (transform, buffers) => compositingItem.Foreground, buffers);

                    var bgData = currentItemBuffer.Lock();
                    var fgData = compositingItem.Foreground.Lock();

                    ImageProcessing.CombineTwo(bgData.Scan0, bgData.Stride, fgData.Scan0, fgData.Stride, bgData.Width, bgData.Height);

                    compositingItem.Foreground.Unlock(fgData);
                    currentItemBuffer.Unlock(bgData);
                }

                backgrounds.Pop();

                if (item.Visual.BackgroundEffects.Any())
                {
                    foreach (var background in backgrounds)
                        foreach (var backgroundEffect in item.Visual.BackgroundEffects)
                            backgroundEffect.Apply(background, currentItemBuffer, item.Mask, buffers);
                }

                var topBackground = backgrounds.Peek();

                var backgroundData = topBackground.Lock();
                var foregroundData = currentItemBuffer.Lock();

                ImageProcessing.CombineTwo(backgroundData.Scan0,
                    backgroundData.Stride,
                    foregroundData.Scan0,
                    foregroundData.Stride,
                    topBackground.Bitmap.Width,
                    topBackground.Bitmap.Height);

                topBackground.Unlock(backgroundData);
                currentItemBuffer.Unlock(foregroundData);
            }
            finally
            {
                buffers.Return(currentItemBuffer);
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

            BitmapBuffer backgroundBuffer = new BitmapBuffer(bitmap, graphics, null);

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
                Compose(backgrounds, compositingItem, originalTransform, buffers);

            backgrounds.Pop();

            if (backgrounds.Any())
                throw new InvalidOperationException("Backgrounds are not empty after processing!");

            // Return all leased buffers
            foreach (var compositingItem in compositingItems)
                compositingItem.Dispose();
        }

        // Public properties --------------------------------------------------


        #region UseCompositing managed property

        public bool UseCompositing
        {
            get => (bool)GetValue(UseCompositingProperty);
            set => SetValue(UseCompositingProperty, value);
        }

        public static readonly ManagedProperty UseCompositingProperty = ManagedProperty.Register(typeof(Scene),
            nameof(UseCompositing),
            typeof(bool),
            new ManagedSimplePropertyMetadata { DefaultValue = false });

        #endregion
    }
}
