using Animator.Engine.Elements.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements.Compositing
{
    internal class CompositingItem : BaseCompositingItem, IDisposable        
    {
        private readonly BitmapBufferRepository repository;

        public CompositingItem(BitmapBuffer foreground, BitmapBuffer mask, Visual visual, BitmapBufferRepository repository)
            : base(visual)
        {
            this.repository = repository;

            Foreground = foreground;
            Mask = mask;            
        }

        public override void Dispose()
        {
            if (Mask != null)
            {
                repository.Return(Mask);
                Mask = null;
            }

            if (Foreground != null)
            {
                repository.Return(Foreground);
                Foreground = null;
            }
        }

        public BitmapBuffer Foreground { get; private set; }
        public BitmapBuffer Mask { get; private set; }        
    }
}
