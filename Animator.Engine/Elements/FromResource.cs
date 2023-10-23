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
            var current = @object as Element;

            while (current != null)
            {
                if (current is SceneElement sceneElement)
                {
                    var res = sceneElement.Resources.Where(r => r.Key == Key).ToList();

                    if (res.Count == 1)
                    {
                        @object.SetValue(property, res[0].GetValue());
                        return;
                    }
                    else if (res.Count > 1)
                        throw new AnimationException($"Object contains more than one resource named {Key}!", (current as Element)?.GetHumanReadablePath() ?? String.Empty);
                }

                current = current.ParentInfo?.Parent as Element;
            }
        }

        public string Key { get; set; }
    }
}
