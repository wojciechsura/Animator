using Animator.Engine.Base;
using Animator.Engine.Base.Persistence;
using Animator.Engine.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    [DefaultProperty(nameof(Key))]
    public class FromResource : BaseMarkupExtension
    {
        public override void ProvideValue(ManagedObject @object, ManagedProperty property)
        {
            var current = @object as SceneElement;

            while (current != null)
            {
                var res = current.Resources.Where(r => r.Key == Key).ToList();
                if (res.Count == 0)
                {
                    current = current.Parent as SceneElement;
                }
                else if (res.Count == 1)
                {
                    @object.SetValue(property, res[0].GetValue());
                    return;
                }
                else
                    throw new AnimationException($"Object contains more than one resource named {Key}!", (current as Element)?.GetPath() ?? String.Empty);
            }
        }

        public string Key { get; set; }
    }
}
