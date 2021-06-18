using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public abstract class StoryboardEntry : Element
    {
        // Internal methods ---------------------------------------------------

        internal abstract void AddKeyframesRecursive(List<Keyframe> keyframes);

        // Public properties --------------------------------------------------

        #region Time managed property

        /// <summary>
        /// Define exact time since start of a scene, when a
        /// property should be set to a fixed value.
        /// </summary>
        public TimeSpan Time
        {
            get => (TimeSpan)GetValue(TimeProperty);
            set => SetValue(TimeProperty, value);
        }

        public static readonly ManagedProperty TimeProperty = ManagedProperty.Register(typeof(StoryboardEntry),
            nameof(Time),
            typeof(TimeSpan),
            new ManagedSimplePropertyMetadata { DefaultValue = TimeSpan.FromMilliseconds(0), Inheritable = true, InheritedFromParent = true });

        #endregion

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

        public static readonly ManagedProperty PropertyRefProperty = ManagedProperty.Register(typeof(StoryboardEntry),
            nameof(PropertyRef),
            typeof(string),
            new ManagedSimplePropertyMetadata { DefaultValue = null, Inheritable = true, InheritedFromParent = true });

        #endregion               
    }
}
