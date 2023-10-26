using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Base class for all brushes. A brush is an object, which
    /// defines fill of a shape. If you need a simple, mono-color
    /// fill, use SolidBrush.
    /// </summary>
    public abstract partial class Brush : SceneElement
    {
        // Internal methods ---------------------------------------------------

        internal abstract System.Drawing.Brush BuildBrush();
    }
}
