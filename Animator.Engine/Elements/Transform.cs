using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Base class for linear transform definitions
    /// </summary>
    public abstract class Transform : SceneElement
    {
        // Internal methods ---------------------------------------------------

        internal abstract Matrix GetMatrix();        
    }
}
