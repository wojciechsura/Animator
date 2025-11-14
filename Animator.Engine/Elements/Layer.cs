using Animator.Engine.Base;
using Animator.Engine.Base.Persistence;
using Animator.Engine.Elements.Rendering;
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
    public partial class Layer : Visual
    {
        // Private methods ----------------------------------------------------

        private void DoRender(BitmapBuffer buffer, BitmapBufferRepository buffers, BitmapBuffer itemBuffer, RenderingContext context)
        {
            var bufferData = buffer.Lock();

            foreach (var item in Items)
            {
                itemBuffer.Graphics.Clear(Color.Transparent);
                item.Render(itemBuffer, buffers, context);

                var itemData = itemBuffer.Lock();
                ImageProcessing.CombineTwo(bufferData.Scan0, bufferData.Stride, itemData.Scan0, itemData.Stride, buffer.Bitmap.Width, buffer.Bitmap.Height);
                itemBuffer.Unlock(itemData);
            }

            buffer.Unlock(bufferData);
        }

        private void RenderNotCloned(BitmapBuffer buffer, BitmapBufferRepository buffers, RenderingContext context)
        {
            BitmapBuffer itemBuffer = buffers.Lease(buffer.Graphics.Transform);

            try
            {
                DoRender(buffer, buffers, itemBuffer, context);
            }
            finally
            {
                buffers.Return(itemBuffer);
            }
        }

        private void RenderCloned(BitmapBuffer buffer, BitmapBufferRepository buffers, RenderingContext context)
        {
            var originalTransform = buffer.Graphics.Transform;
            BitmapBuffer itemBuffer = buffers.Lease(buffer.Graphics.Transform);

            try
            { 
                List<int> steps = new(Clones.Count);
                for (int i = 0; i < Clones.Count; i++)
                    if (!Clones[i].ReverseOrder)
                        steps.Add(0);
                    else
                        steps.Add(Clones[i].Count - 1);

                // Advances i-th step. Returns true if it overflowed.
                bool AdvanceStep(int i)
                {
                    if (!Clones[i].ReverseOrder)
                    {
                        steps[i]++;
                        if (steps[i] >= Clones[i].Count)
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
                            steps[i] = Clones[i].Count - 1;
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

                    while (i < Clones.Count && AdvanceStep(i))
                        i++;

                    return i >= Clones.Count;
                }

                bool finish = false;
                while (!finish)
                {
                    Matrix transform = new();
                    for (int i = 0; i < Clones.Count; i++)
                        Clones[i].ApplyTransforms(transform, steps[i]);

                    transform.Multiply(originalTransform, MatrixOrder.Append);

                    itemBuffer.Graphics.Transform = transform;
                    DoRender(buffer, buffers, itemBuffer, context);

                    finish = Advance();
                }
            }
            finally
            {
                buffers.Return(itemBuffer);
            }
        }

        // Protected methods --------------------------------------------------

        protected override void InternalRender(BitmapBuffer buffer, BitmapBufferRepository buffers, RenderingContext context)
        {
            if (Clones.Count == 0)
                RenderNotCloned(buffer, buffers, context);
            else
                RenderCloned(buffer, buffers, context);
        }

        // Public properites --------------------------------------------------

        public override bool AlwaysRender => Items.Any(i => i.AlwaysRender);

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
        public ManagedCollection<LayerCloningStep> Clones
        {
            get => (ManagedCollection<LayerCloningStep>)GetValue(CloneProperty);
        }

        public static readonly ManagedProperty CloneProperty = ManagedProperty.RegisterCollection(typeof(Layer),
            nameof(Clones),
            typeof(ManagedCollection<LayerCloningStep>));

        #endregion
    }
}
