using Animator.Engine.Base.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base.Tests.TestClasses
{
    [DefaultProperty(nameof(Text))]
    public class StringMarkupExtension : BaseMarkupExtension
    {
        public override void ProvideValue(ManagedObject @object, ManagedProperty property)
        {
            @object.SetValue(property, Text + Optional);
        }

        public string Text { get; set; } = "";

        public string Optional { get; set; } = "";
    }
}
