using Animator.Engine.Elements.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements.Compositing
{
    internal abstract class BaseCompositingItem : IDisposable
    {
        public BaseCompositingItem(Visual visual, BitmapBuffer mask)
        {
            Visual = visual;
            Mask = mask;
        }

        public virtual void Dispose()
        {
            Visual = null;

            if (Mask != null)
            {
                Mask.Dispose();
                Mask = null;
            }
        }

        public Visual Visual { get; private set; }
        public BitmapBuffer Mask { get; private set; }
    }
}
