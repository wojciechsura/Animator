using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public abstract class PathElement : ManagedObject
    {
        protected string F(float value) => string.Format(CultureInfo.InvariantCulture, "{0:.##}", value);

        public abstract string ToPathString();        
    }
}
