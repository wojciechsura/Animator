using Animator.Engine.Animation;
using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public abstract class PropertyAnimator : BaseAnimator
    {
        // Public methods -----------------------------------------------------

        public override void ResetAnimation()
        {
            if (Scene == null)
                throw new InvalidOperationException("Animation can be reset only if scene is available!");

            (var obj, var prop) = Scene.FindProperty(TargetName, Path);
            if (obj != null && prop != null)
                obj.ResetAnimatedValue(prop);
        }

        // Public properties --------------------------------------------------

        #region TargetName managed property

        public string TargetName
        {
            get => (string)GetValue(TargetNameProperty);
            set => SetValue(TargetNameProperty, value);
        }

        public static readonly ManagedProperty TargetNameProperty = ManagedProperty.Register(typeof(PropertyAnimator),
            nameof(TargetName),
            typeof(string),
            new ManagedSimplePropertyMetadata { NotAnimatable = true });

        #endregion

        #region Path managed property

        public string Path
        {
            get => (string)GetValue(PathProperty);
            set => SetValue(PathProperty, value);
        }

        public static readonly ManagedProperty PathProperty = ManagedProperty.Register(typeof(PropertyAnimator),
            nameof(Path),
            typeof(string),
            new ManagedSimplePropertyMetadata { NotAnimatable = true });

        #endregion
    }
}
