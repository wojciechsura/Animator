using Animator.Engine.Elements.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements.Compositing
{
    internal class NestedCompositingItem : BaseCompositingItem
    {
        public NestedCompositingItem(IReadOnlyList<BaseCompositingItem> items, BitmapBuffer mask, Visual visual)
            : base(visual)
        {
            Items = items;
            Mask = mask;
        }

        public override void Dispose()
        {
            foreach (var item in Items)
            {
                item.Dispose();
            }

            if (Mask != null)
            {
                Mask.Dispose();
                Mask = null;
            }

            Items = null;
        }

        public IReadOnlyList<BaseCompositingItem> Items { get; private set; }
        public BitmapBuffer Mask { get; private set; }
    }
}
