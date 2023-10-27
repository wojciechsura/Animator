using Animator.Engine.Elements.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements.Compositing
{
    internal class VisualCompositingItem : BaseCompositingItem, IDisposable        
    {
        private readonly BitmapBufferRepository repository;

        public VisualCompositingItem(BitmapBuffer foreground, 
            BitmapBuffer mask, 
            Visual visual, 
            BitmapBufferRepository repository)
            : base(visual, mask)
        {
            this.repository = repository;

            Foreground = foreground;
        }

        public override void Dispose()
        {
            if (Foreground != null)
            {
                repository.Return(Foreground);
                Foreground = null;
            }
        }

        public BitmapBuffer Foreground { get; private set; }
    }
}
