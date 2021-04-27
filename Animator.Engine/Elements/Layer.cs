using Animator.Engine.Base;
using Animator.Engine.Base.Persistence;
using Animator.Engine.Elements.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    [ContentProperty(nameof(Layer.Items))]
    public class Layer : Visual
    {
        protected override void InternalRender(BitmapBuffer buffer, BitmapBufferRepository buffers)
        {
            BitmapBuffer itemBuffer = buffers.Lease(buffer.Graphics.Transform);

            try
            {
                foreach (var item in Items)
                {
                    itemBuffer.Graphics.Clear(Color.Transparent);
                    item.Render(itemBuffer, buffers);
                    buffer.Graphics.DrawImage(itemBuffer.Bitmap, 0, 0);
                }
            }
            finally
            {
                buffers.Return(itemBuffer);
            }
        }


        #region Items managed collection

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
