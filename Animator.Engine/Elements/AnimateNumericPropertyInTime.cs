using Animator.Engine.Animation.Maths;
using Animator.Engine.Base;
using Animator.Engine.Elements.Types;
using Animator.Engine.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Base class for all property animators, which are
    /// modifying a numeric property (ie. int, float, PointF, etc.)
    /// </summary>
    public abstract class AnimateNumericPropertyInTime : AnimatePropertyInTime
    {
        protected float Ease(float t)
        {
            if (IsPropertySet(EasingExpressionProperty) && !IsPropertySet(EasingFunctionProperty))
            {
                // If expression is set, use it
                return EasingExpression.CalculateFactor(t);
            }
            else if (IsPropertySet(EasingExpressionProperty) && IsPropertySet(EasingFunctionProperty))
            {
                // Both cannot be set at the same time
                throw new AnimationException($"{nameof(AnimateNumericPropertyInTime.EasingFunction)} and {nameof(AnimateNumericPropertyInTime.EasingExpression)} cannot be set at the same time!", GetPath());
            }
            else
            {
                // Use EasingFunction regardless of whether it is set or not
                return EasingFunctionRepository.Ease(EasingFunction, t);
            }
        }

        #region EasingFunction managed property

        /// <summary>
        /// Defines easing function used for animation. The role of
        /// easing function is to define, how value should change
        /// between previous and current keyframe. Possible
        /// easing functions include the following:
        /// 
        /// <ul>
        ///     <li>Linear</li>
        ///     <li>SwitchOnStart</li>
        ///     <li>SwitchInTheMiddle</li>
        ///     <li>SwitchOnEnd</li>
        ///     <li>SineSpeedUp</li>
        ///     <li>SineSlowDown</li>
        ///     <li>SineBoth</li>
        ///     <li>QuadSpeedUp</li>
        ///     <li>QuadSlowDown</li>
        ///     <li>QuadBoth</li>
        ///     <li>CubicSpeedUp</li>
        ///     <li>CubicSlowDown</li>
        ///     <li>CubicBoth</li>
        ///     <li>QuartSpeedUp</li>
        ///     <li>QuartSlowDown</li>
        ///     <li>QuartBoth</li>
        ///     <li>BackSpeedUp</li>
        ///     <li>BackSlowDown</li>
        ///     <li>BackBoth</li>
        /// </ul>
        /// </summary>
        public EasingFunction EasingFunction
        {
            get => (EasingFunction)GetValue(EasingFunctionProperty);
            set => SetValue(EasingFunctionProperty, value);
        }

        public static readonly ManagedProperty EasingFunctionProperty = ManagedProperty.Register(typeof(AnimateNumericPropertyInTime),
            nameof(EasingFunction),
            typeof(EasingFunction),
            new ManagedSimplePropertyMetadata { DefaultValue = EasingFunction.Linear, InheritedFromParent = true });

        #endregion

        #region Expression managed property

        public EasingExpression EasingExpression
        {
            get => (EasingExpression)GetValue(EasingExpressionProperty);
            set => SetValue(EasingExpressionProperty, value);
        }

        public static readonly ManagedProperty EasingExpressionProperty = ManagedProperty.RegisterReference(typeof(AnimateNumericPropertyInTime),
            nameof(EasingExpression),
            typeof(EasingExpression),
            new ManagedReferencePropertyMetadata());

        #endregion
    }
}
