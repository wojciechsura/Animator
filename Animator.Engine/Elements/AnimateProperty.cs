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
    public abstract class AnimateProperty : Animation
    {
        // Public methods -----------------------------------------------------

        public override void ResetAnimation()
        {
            (var obj, var prop) = AnimatedObject.FindProperty(PropertyRef);
            obj.ClearAnimatedValue(prop);
        }

        // Public properties --------------------------------------------------


        #region PropertyRef managed property

        /// <summary>
        /// Reference to a property, relative to object owning this animator.
        /// Subsequent elements must be separated by dots. You may call elements
        /// from collections as well, as long as they have their Name property
        /// set and it is unique in this collection.
        /// </summary>
        /// <example>
        /// <code>PropertyRef="MyRectangle.Pen.Color"</code>
        /// </example>
        public string PropertyRef
        {
            get => (string)GetValue(PropertyRefProperty);
            set => SetValue(PropertyRefProperty, value);
        }

        public static readonly ManagedProperty PropertyRefProperty = ManagedProperty.Register(typeof(AnimateProperty),
            nameof(PropertyRef),
            typeof(string),
            new ManagedSimplePropertyMetadata { DefaultValue = null });

        #endregion        
    }
}
