using Animator.Designer.BusinessLogic.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.Helpers
{
    internal static class TypeIconHelper
    {
        private static readonly string fallbackIcon = "Generic16.png";

        private static readonly Dictionary<(NamespaceType Namespace, string Name), string> icons = new()
        {
            { (NamespaceType.Default, nameof(Animator.Engine.Elements.Image)), "Image16.png" },
            { (NamespaceType.Default, nameof(Animator.Engine.Elements.Layer)), "Layer16.png" },
            { (NamespaceType.Default, nameof(Animator.Engine.Elements.Rectangle)), "Rectangle16.png" },
            { (NamespaceType.Default, nameof(Animator.Engine.Elements.Circle)), "Circle16.png" },
            { (NamespaceType.Default, nameof(Animator.Engine.Elements.Ellipse)), "Ellipse16.png" },
            { (NamespaceType.Default, nameof(Animator.Engine.Elements.Line)), "Line16.png" },
            { (NamespaceType.Default, nameof(Animator.Engine.Elements.SvgImage)), "SvgImage16.png" },
            { (NamespaceType.Default, nameof(Animator.Engine.Elements.Label)), "Label16.png" },
            { (NamespaceType.Default, nameof(Animator.Engine.Elements.Path)), "Path16.png" },

            { (NamespaceType.Default, nameof(Animator.Engine.Elements.AnimateInt)), "AnimateInt16.png" },
            { (NamespaceType.Default, nameof(Animator.Engine.Elements.AnimateBool)), "AnimateBool16.png" },
            { (NamespaceType.Default, nameof(Animator.Engine.Elements.AnimateFloat)), "AnimateFloat16.png" },
            { (NamespaceType.Default, nameof(Animator.Engine.Elements.AnimatePoint)), "AnimatePoint16.png" },
            { (NamespaceType.Default, nameof(Animator.Engine.Elements.AnimateColor)), "AnimateColor16.png" },
            { (NamespaceType.Default, nameof(Animator.Engine.Elements.AnimateStopwatch)), "AnimateStopwatch16.png" },
            { (NamespaceType.Default, nameof(Animator.Engine.Elements.AnimateWithExpression)), "AnimateWithExpression16.png" },
            { (NamespaceType.Default, nameof(Animator.Engine.Elements.Storyboard)), "Storyboard16.png" },
        };

        internal static string GetIcon(NamespaceType namespaceType, string name)
        {
            if (icons.TryGetValue((namespaceType, name), out string icon))
                return icon;

            return fallbackIcon;
        }
    }
}
