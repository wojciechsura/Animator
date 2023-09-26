using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base.Persistence
{
    public sealed class PendingMarkupExtension
    {
        public PendingMarkupExtension(BaseMarkupExtension markupExtension, ManagedProperty property, ManagedObject @object)
        {
            MarkupExtension = markupExtension;
            Property = property;
            Object = @object;
        }

        public BaseMarkupExtension MarkupExtension { get; }
        public ManagedProperty Property { get; }
        public ManagedObject Object { get; }
    }
}
