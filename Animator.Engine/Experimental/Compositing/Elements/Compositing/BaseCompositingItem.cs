using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements.Compositing
{
    internal abstract class BaseCompositingItem : IDisposable
    {
        public BaseCompositingItem(Visual visual)
        {
            Visual = visual;
        }

        public virtual void Dispose()
        {
            Visual = null;
        }

        public Visual Visual { get; private set; }
    }
}
