using Animator.Engine.Base;
using Animator.Engine.Elements.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public class FloatPropertyAnimator : TimeSegmentPropertyAnimator
    {
        // Public properties --------------------------------------------------

        #region From managed property

        public float From
        {
            get => (float)GetValue(FromProperty);
            set => SetValue(FromProperty, value);
        }

        public static readonly ManagedProperty FromProperty = ManagedProperty.Register(typeof(FloatPropertyAnimator),
            nameof(From),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 0.0f });

        #endregion

        #region To managed property

        public float To
        {
            get => (float)GetValue(ToProperty);
            set => SetValue(ToProperty, value);
        }

        public static readonly ManagedProperty ToProperty = ManagedProperty.Register(typeof(FloatPropertyAnimator),
            nameof(To),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 0.0f });

        #endregion

        #region EasingFunction managed property

        public EasingFunction EasingFunction
        {
            get => (EasingFunction)GetValue(EasingFunctionProperty);
            set => SetValue(EasingFunctionProperty, value);
        }

        public static readonly ManagedProperty EasingFunctionProperty = ManagedProperty.Register(typeof(FloatPropertyAnimator),
            nameof(EasingFunction),
            typeof(EasingFunction),
            new ManagedSimplePropertyMetadata { DefaultValue = EasingFunction.Linear });

        #endregion
    }
}
