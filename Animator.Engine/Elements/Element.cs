using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public abstract class Element : ManagedObject
    {
        public string GetPath()
        {
            ManagedObject obj = this;
            List<string> parents = new();

            while (obj != null)
            {
                if (obj is SceneElement sceneElement)
                    parents.Add($"{sceneElement.GetType().Name} ({sceneElement.Name})");
                else
                    parents.Add($"{obj.GetType().Name}");
            }

            parents.Reverse();

            return string.Join(" -> ", parents);
        }
    }
}
