using Animator.Engine.Base.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base.Tests.TestClasses
{
    [DefaultProperty(nameof(Value))]
    public class MarkupExtension : BaseMarkupExtension
    {
        public override void ProvideValue(ManagedObject @object, ManagedProperty property)
        {
            @object.SetValue(property, Value + Offset1 + Offset2);
        }

        public int Value { get; set; } = 0;
        public int Offset1 { get; set; } = 0;
        public int Offset2 { get; set; } = 0;
    }
}
