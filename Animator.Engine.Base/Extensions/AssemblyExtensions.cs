using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base.Extensions
{
    public static class AssemblyExtensions
    {
        private static HashSet<(Assembly assembly, string ns)> staticallyInitializedAssemblies = new();

        public static void InitializeStaticTypes(this Assembly assembly, string ns)
        {
            if (staticallyInitializedAssemblies.Contains((assembly, ns)))
                return;

            foreach (var type in assembly.GetTypes().Where(t => t.Namespace == ns))
            {
                type.StaticInitializeRecursively();
            }

            staticallyInitializedAssemblies.Add((assembly, ns));
        }
    }
}
