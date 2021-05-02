using Animator.Engine.Animation.Maths;
using Animator.Engine.Base;
using Animator.Engine.Elements.Types;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Base class for all keyframes, which define animation
    /// of a numeric properties (ie. of type int, float, PointF etc.)
    /// </summary>
    public abstract class BaseNumericKeyframe : BaseKeyframe
    {
        // Public methods -----------------------------------------------------

        #region EasingFunction managed property

        /// <summary>
        /// Defines easing function used for animation. The role of
        /// easing function is to define, how value should change
        /// between previous and current keyframe. Possible
        /// easing functions include the following:
        /// 
        /// <ul>
        ///     <li>Linear</li>
        ///     <li>EaseSineSpeedUp</li>
        ///     <li>EaseSineSlowDown</li>
        ///     <li>EaseSineBoth</li>
        ///     <li>EaseQuadSpeedUp</li>
        ///     <li>EaseQuadSlowDown</li>
        ///     <li>EaseQuadBoth</li>
        ///     <li>EaseCubicSpeedUp</li>
        ///     <li>EaseCubicSlowDown</li>
        ///     <li>EaseCubicBoth</li>
        ///     <li>EaseQuartSpeedUp</li>
        ///     <li>EaseQuartSlowDown</li>
        ///     <li>EaseQuartBoth</li>
        ///     <li>EaseBackSpeedUp</li>
        ///     <li>EaseBackSlowDown</li>
        ///     <li>EaseBackBoth</li>
        /// </ul>
        /// </summary>
        public EasingFunction EasingFunction
        {
            get => (EasingFunction)GetValue(EasingFunctionProperty);
            set => SetValue(EasingFunctionProperty, value);
        }

        public static readonly ManagedProperty EasingFunctionProperty = ManagedProperty.Register(typeof(TimeDurationNumericPropertyAnimator),
            nameof(EasingFunction),
            typeof(EasingFunction),
            new ManagedSimplePropertyMetadata { DefaultValue = EasingFunction.Linear, InheritedFromParent = true });

        #endregion
    }
}