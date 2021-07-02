using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Editor.Configuration
{
    public abstract class BaseItemCollection : ConfigItem
    {
        // Public methods -----------------------------------------------------

        public BaseItemCollection(string name, BaseItemContainer parent)
            : base(name, parent)
        {

        }
    }
}
