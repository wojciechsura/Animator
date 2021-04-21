using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public abstract class Brush : ManagedObject
    {
        // Internal methods ---------------------------------------------------

        internal abstract System.Drawing.Brush BuildBrush();
    }
}
