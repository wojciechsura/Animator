using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.Infrastructure
{
    public interface IParentedItem<TParent>
        where TParent : class
    {
        public TParent Parent { get; set; }
    }
}
