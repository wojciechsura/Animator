using Animator.Engine.Base;
using Animator.Engine.Base.Persistence;
using Animator.Engine.Elements.Utilities;
using Animator.Engine.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Allows grouping visuals. You can use it to apply
    /// transforms or effects to whole group of elements, or
    /// add a common effect to all of them at once.
    /// </summary>
    [ContentProperty(nameof(Layer.Items))]
    public class Layer : Visual
    {
        protected override void InternalRender(BitmapBuffer buffer, BitmapBufferRepository buffers)
        {
            BitmapBuffer itemBuffer = buffers.Lease(buffer.Graphics.Transform);

            try
            {
                var bufferData = buffer.Bitmap.LockBits(new System.Drawing.Rectangle(0, 0, buffer.Bitmap.Width, buffer.Bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                foreach (var item in Items)
                {
                    itemBuffer.Graphics.Clear(Color.Transparent);
                    item.Render(itemBuffer, buffers);

                    var itemData = itemBuffer.Bitmap.LockBits(new System.Drawing.Rectangle(0, 0, itemBuffer.Bitmap.Width, itemBuffer.Bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    ImageProcessing.CombineTwo(bufferData.Scan0, bufferData.Stride, itemData.Scan0, itemData.Stride, buffer.Bitmap.Width, buffer.Bitmap.Height);
                    itemBuffer.Bitmap.UnlockBits(itemData);
                }

                buffer.Bitmap.UnlockBits(bufferData);
            }
            finally
            {
                buffers.Return(itemBuffer);
            }
        }

        #region Items managed collection

        /// <summary>
        /// Elements on a layer.
        /// </summary>
        public ManagedCollection<Visual> Items
        {
            get => (ManagedCollection<Visual>)GetValue(ItemsProperty);
        }

        public static readonly ManagedProperty ItemsProperty = ManagedProperty.RegisterCollection(typeof(Layer),
            nameof(Items),
            typeof(ManagedCollection<Visual>));

        #endregion
    }
}
