using Animator.Engine.Base;
using Animator.Engine.Base.Persistence;
using Animator.Engine.Elements.Utilities;
using Animator.Engine.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        // Private methods ----------------------------------------------------

        private void DoRender(BitmapBuffer buffer, BitmapBufferRepository buffers, BitmapBuffer itemBuffer)
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

        private void RenderNotCloned(BitmapBuffer buffer, BitmapBufferRepository buffers)
        {
            BitmapBuffer itemBuffer = buffers.Lease(buffer.Graphics.Transform);

            try
            {
                DoRender(buffer, buffers, itemBuffer);
            }
            finally
            {
                buffers.Return(itemBuffer);
            }
        }

        private void RenderCloned(BitmapBuffer buffer, BitmapBufferRepository buffers)
        {
            var originalTransform = buffer.Graphics.Transform;
            BitmapBuffer itemBuffer = buffers.Lease(buffer.Graphics.Transform);

            try
            { 
                List<int> steps = new(Clone.Count);
                for (int i = 0; i < Clone.Count; i++)
                    if (!Clone[i].ReverseOrder)
                        steps.Add(0);
                    else
                        steps.Add(Clone[i].Count - 1);

                // Advances i-th step. Returns true if it overflowed.
                bool AdvanceStep(int i)
                {
                    if (!Clone[i].ReverseOrder)
                    {
                        steps[i]++;
                        if (steps[i] >= Clone[i].Count)
                        {
                            steps[i] = 0;
                            return true;
                        }
                        else
                            return false;
                    }
                    else
                    {
                        steps[i]--;
                        if (steps[i] < 0)
                        {
                            steps[i] = Clone[i].Count - 1;
                            return true;
                        }
                        else
                            return false;
                    }
                }

                // Advances whole series one step. Applies overflows
                // to next steps. Returns true if all steps overflowed
                // (= all steps were processed)
                bool Advance()
                {
                    int i = 0;

                    while (i < Clone.Count && AdvanceStep(i))
                        i++;

                    return i >= Clone.Count;
                }

                bool finish = false;
                while (!finish)
                {
                    Matrix transform = new();
                    for (int i = 0; i < Clone.Count; i++)
                        Clone[i].ApplyTransforms(transform, steps[i]);

                    transform.Multiply(originalTransform, MatrixOrder.Append);

                    itemBuffer.Graphics.Transform = transform;
                    DoRender(buffer, buffers, itemBuffer);

                    finish = Advance();
                }
            }
            finally
            {
                buffers.Return(itemBuffer);
            }
        }

        // Protected methods --------------------------------------------------

        protected override void InternalRender(BitmapBuffer buffer, BitmapBufferRepository buffers)
        {
            if (Clone.Count == 0)
                RenderNotCloned(buffer, buffers);
            else
                RenderCloned(buffer, buffers);
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

        #region Clone managed collection

        /// <summary>
        /// Clones layer contents with given transforms.
        /// </summary>
        public ManagedCollection<LayerCloningStep> Clone
        {
            get => (ManagedCollection<LayerCloningStep>)GetValue(CloneProperty);
        }

        public static readonly ManagedProperty CloneProperty = ManagedProperty.RegisterCollection(typeof(Layer),
            nameof(Clone),
            typeof(ManagedCollection<LayerCloningStep>));

        #endregion
    }
}
