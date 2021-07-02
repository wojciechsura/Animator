using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace Animator.Editor.Dependencies
{
    public static class Container
    {
        private static Lazy<UnityContainer> container = new Lazy<UnityContainer>(() => new UnityContainer());

        public static IUnityContainer Instance
        {
            get
            {
                return container.Value;
            }
        }
    }
}
