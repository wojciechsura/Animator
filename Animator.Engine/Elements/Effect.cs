using Animator.Engine.Elements.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// This class serves as an abstract base class for all
    /// effects, which may be applied to rendered elements.
    /// </summary>
    public abstract class Effect : SceneElement
    {
        internal abstract void Apply(BitmapBuffer framebuffer, BitmapBuffer backBuffer, BitmapBuffer frontBuffer, BitmapBufferRepository repository);        
    }
}
