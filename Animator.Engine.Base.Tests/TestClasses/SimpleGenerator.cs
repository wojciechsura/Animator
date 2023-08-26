using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Animator.Engine.Base.Tests.TestClasses
{
    internal class SimpleGenerator : BaseGenerator
    {
        public override ManagedObject Generate(XmlElement node)
        {
            var result = new SimplePropertyClass();
            result.IntValue = 99;

            return result;
        }
    }
}
