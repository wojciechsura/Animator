using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Animator.Engine.Base
{
    public abstract class BaseGenerator
    {
        public abstract ManagedObject Generate(XmlNode node);
    }
}
