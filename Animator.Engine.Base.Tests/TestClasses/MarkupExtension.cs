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

        public int Value { get; set; }
        public int Offset1 { get; set; }
        public int Offset2 { get; set; }
    }
}
