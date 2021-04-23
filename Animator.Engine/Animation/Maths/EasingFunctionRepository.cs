using Animator.Engine.Elements.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Animation.Maths
{
    public static class EasingFunctionRepository
    {
        // Private static fields ----------------------------------------------

        private static readonly Dictionary<EasingFunction, Func<float, float>> easingFunctions = new();

        private static readonly Func<float, float> sineSpeedUp = x => (float)-Math.Cos(x * Math.PI / 2.0f) + 1.0f;

        private static readonly Func<float, float> squareSpeedUp = x => (float)Math.Pow(x, 2.0);

        private static readonly Func<float, float> cubicSpeedUp = x => (float)Math.Pow(x, 3.0);

        private static readonly Func<float, float> quartSpeedUp = x => (float)Math.Pow(x, 4.0);

        private static readonly Func<float, float> backSpeedUp = x => cubicSpeedUp(x) + NegativeNudge(2, 8)(x);

        // Private static methods ---------------------------------------------

        /// <remarks>Assumes, that func is continuous, func(0) = 0 and func(1) = 1.</remarks>
        private static Func<float, float> SpeedUpToSlowDown(Func<float, float> f) => x => 1.0f - f(1.0f - x);

        /// <remarks>Assumes, that func is continuous, func(0) = 0 and func(1) = 1.</remarks>
        private static Func<float, float> SpeedUpToBoth(Func<float, float> f) => x => x < 0.5 ? f(2.0f * x) / 2.0f : 1.0f - f((1.0f - x) * 2.0f) / 2.0f;

        /// <remarks>Assumes, that func is continuous, func(0) = 0 and func(1) = 1.</remarks>
        private static Func<float, float> SpeedUpToReverseBoth(Func<float, float> f) => x => x < 0.5 ? 0.5f - f((0.5f - x) * 2.0f) / 2.0f : f((x - 0.5f) * 2.0f) / 2.0f + 0.5f;

        private static Func<float, float> NegativeNudge(float horizontalDivider, float verticalDivider) => x => x > 0 && x < 1.0f / horizontalDivider ? ((float)Math.Cos(horizontalDivider * x * Math.PI * 2) - 1) / verticalDivider : 0.0f;

        // Static ctor --------------------------------------------------------

        static EasingFunctionRepository()
        {
            easingFunctions[EasingFunction.Linear] = x => x;

            easingFunctions[EasingFunction.EaseSineSpeedUp] = sineSpeedUp;

            easingFunctions[EasingFunction.EaseSineSlowDown] = SpeedUpToSlowDown(sineSpeedUp);

            easingFunctions[EasingFunction.EaseSineBoth] = SpeedUpToBoth(sineSpeedUp);

            easingFunctions[EasingFunction.EaseQuadSpeedUp] = squareSpeedUp;

            easingFunctions[EasingFunction.EaseQuadSlowDown] = SpeedUpToSlowDown(squareSpeedUp);

            easingFunctions[EasingFunction.EaseQuadBoth] = SpeedUpToBoth(squareSpeedUp);

            easingFunctions[EasingFunction.EaseCubicSpeedUp] = cubicSpeedUp;

            easingFunctions[EasingFunction.EaseCubicSlowDown] = SpeedUpToSlowDown(cubicSpeedUp);

            easingFunctions[EasingFunction.EaseCubicBoth] = SpeedUpToBoth(cubicSpeedUp);

            easingFunctions[EasingFunction.EaseQuartSpeedUp] = quartSpeedUp;

            easingFunctions[EasingFunction.EaseQuartSlowDown] = SpeedUpToSlowDown(quartSpeedUp);

            easingFunctions[EasingFunction.EaseQuartBoth] = SpeedUpToBoth(quartSpeedUp);

            easingFunctions[EasingFunction.EaseBackSpeedUp] = backSpeedUp;

            easingFunctions[EasingFunction.EaseBackSlowDown] = SpeedUpToSlowDown(backSpeedUp);

            easingFunctions[EasingFunction.EaseBackBoth] = SpeedUpToBoth(backSpeedUp);
        }

        // Public static methods ----------------------------------------------

        public static float Ease(EasingFunction function, float x) => easingFunctions[function](x);
    }
}
