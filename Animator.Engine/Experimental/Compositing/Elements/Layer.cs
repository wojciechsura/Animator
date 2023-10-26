using Animator.Engine.Elements.Compositing;
using Animator.Engine.Elements.Rendering;
using Animator.Engine.Elements.Types;
using Animator.Engine.Elements.Utilities;
using Animator.Engine.Tools;
using Animator.Engine.Utils;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public partial class Layer
    {
        private void DoBuildCompositingItems(List<BaseCompositingItem> results, Matrix transform, BitmapBufferRepository buffers)
        {
            foreach (var item in Items)
            {
                var compositingItem = item.BuildCompositingItem(transform, buffers);
                if (compositingItem != null)
                {
                    results.Add(compositingItem);
                }
            }
        }

        private void BuildCompositingItemCloned(List<BaseCompositingItem> results, Matrix originalTransform, BitmapBufferRepository buffers)
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

                DoBuildCompositingItems(results, transform, buffers);

                finish = Advance();
            }
        }

        private void BuildCompositingItemNotCloned(List<BaseCompositingItem> results, Matrix transform, BitmapBufferRepository buffers)
        {
            DoBuildCompositingItems(results, transform, buffers);
        }

        // Protected methods --------------------------------------------------

        internal override BaseCompositingItem BuildCompositingItem(Matrix originalTransform, BitmapBufferRepository buffers)
        {
            var results = new List<BaseCompositingItem>();

            if (!Visible || Alpha.IsZero())
                return null;

            var transform = originalTransform.Clone();
            transform.Multiply(BuildTransformMatrix(), MatrixOrder.Prepend);

            // Scale eqaual to 0 equals to invisible object
            if (Math.Abs(transform.MatrixElements.M11).IsZero() &&
                Math.Abs(transform.MatrixElements.M12).IsZero() &&
                Math.Abs(transform.MatrixElements.M21).IsZero() &&
                Math.Abs(transform.MatrixElements.M22).IsZero())
                return null;

            if (Clone.Count == 0)
                BuildCompositingItemNotCloned(results, transform, buffers);
            else
                BuildCompositingItemCloned(results, transform, buffers);

            // Build mask

            BitmapBuffer maskBuffer;

            var itemBuffer = buffers.Lease(MaskCoordinateSystem == MaskCoordinateSystem.Local ? transform : originalTransform);
            try
            {
                maskBuffer = VisualRenderer.BuildMaskBuffer(itemBuffer, Mask, buffers, null);
            }
            finally
            {
                buffers.Return(itemBuffer);
            }

            return new NestedCompositingItem(results, maskBuffer, this);
        }
    }
}
