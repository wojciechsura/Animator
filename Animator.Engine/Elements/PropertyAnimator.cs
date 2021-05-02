using Animator.Engine.Animation;
using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Base class for all animators, which operates on object's
    /// properties.
    /// </summary>
    public abstract class PropertyAnimator : BaseAnimator
    {
        // Public methods -----------------------------------------------------

        public override void ResetAnimation()
        {
            if (Scene == null)
                throw new InvalidOperationException("Animation can be reset only if scene is available!");

            (var obj, var prop) = Scene.FindProperty(TargetName, Path);
            if (obj != null && prop != null)
                obj.ClearAnimatedValue(prop);
        }

        // Public properties --------------------------------------------------

        #region TargetName managed property

        /// <summary>
        /// Defines name of an object, which property should be modified.
        /// </summary>
        public string TargetName
        {
            get => (string)GetValue(TargetNameProperty);
            set => SetValue(TargetNameProperty, value);
        }

        public static readonly ManagedProperty TargetNameProperty = ManagedProperty.Register(typeof(PropertyAnimator),
            nameof(TargetName),
            typeof(string),
            new ManagedSimplePropertyMetadata { NotAnimatable = true, InheritedFromParent = true });

        #endregion

        #region Path managed property

        /// <summary>
        /// Defines path to a property, starting at the object pointed
        /// to by TargetName. Path may be either a single property,
        /// for example <code>Position</code>, or a chain of properties,
        /// leading through subsequent object, like <code>Pen.Color</code>.
        /// </summary>
        public string Path
        {
            get => (string)GetValue(PathProperty);
            set => SetValue(PathProperty, value);
        }

        public static readonly ManagedProperty PathProperty = ManagedProperty.Register(typeof(PropertyAnimator),
            nameof(Path),
            typeof(string),
            new ManagedSimplePropertyMetadata { NotAnimatable = true, InheritedFromParent = true });

        #endregion
    }
}
