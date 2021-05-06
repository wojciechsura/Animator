using Animator.Engine.Animation.Maths;
using Animator.Engine.Base;
using Animator.Engine.Elements.Types;
using Animator.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Base class for all keyframe classes. A keyframe
    /// class describes fixed value for a property in 
    /// specific time.
    /// </summary>
    public abstract class Keyframe : Element
    {
        // Public methods -----------------------------------------------------

        public abstract object GetValue();

        public abstract object EvalValue(float fromTimeMs, object fromValue, float currentTimeMs);

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

        public static readonly ManagedProperty TimeProperty = ManagedProperty.Register(typeof(Keyframe),
            nameof(Time),
            typeof(TimeSpan),
            new ManagedSimplePropertyMetadata { DefaultValue = TimeSpan.FromMilliseconds(0) });

        #endregion

        #region PropertyRef managed property

        /// <summary>
        /// Reference to a property, relative to object owning this animator.
        /// Subsequent elements must be separated by dots. You may call elements
        /// from collections as well, as long as they have their Name property
        /// set and it is unique in this collection.
        /// </summary>
        /// <example>
        /// <code>PropertyRef=\"MyRectangle.Pen.Color\"</code>
        /// </example>
        public string PropertyRef
        {
            get => (string)GetValue(PropertyRefProperty);
            set => SetValue(PropertyRefProperty, value);
        }

        public static readonly ManagedProperty PropertyRefProperty = ManagedProperty.Register(typeof(Keyframe),
            nameof(PropertyRef),
            typeof(string),
            new ManagedSimplePropertyMetadata { DefaultValue = null });

        #endregion               
    }
}
