using Animator.Engine.Elements.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements.Compositing
{
    internal class LayerCompositingItem : BaseCompositingItem
    {
        public LayerCompositingItem(IReadOnlyList<BaseCompositingItem> items, BitmapBuffer mask, Visual visual)
            : base(visual, mask)
        {
            Items = items;
        }

        public override void Dispose()
        {
            foreach (var item in Items)
            {
                item.Dispose();
            }

            Items = null;
        }

        public IReadOnlyList<BaseCompositingItem> Items { get; private set; }
    }
}
