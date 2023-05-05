using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Animation.Maths
{
    public static class TimeCalculator
    {
        private const float millisecondsInSecond = 1000.0f;

        public static float EvalAnimationFactor(TimeSpan start, TimeSpan end, TimeSpan current)
        {
            if (end < start)
                throw new ArgumentOutOfRangeException(nameof(end));

            if (current < start)
                return 0.0f;
            else if (current > end)
                return 1.0f;
            else
                return (float)((current - start).TotalMilliseconds / (end - start).TotalMilliseconds);
        }

        public static float EvalAnimationFactor(float startMs, float endMs, float currentMs)
        {
            if (endMs < startMs)
                throw new ArgumentOutOfRangeException(nameof(endMs));

            if (currentMs < startMs)
                return 0.0f;
            else if (currentMs > endMs)
                return 1.0f;
            else if (Math.Abs(endMs - startMs) < float.Epsilon)
                return 0.0f;
            else
                return (currentMs - startMs) / (endMs - startMs);
        }

        public static int EvalFrameCount(TimeSpan duration, float framesPerSecond)
        {
            if (framesPerSecond <= 0)
                throw new ArgumentOutOfRangeException(nameof(framesPerSecond));

            return (int)Math.Floor(duration.TotalSeconds * framesPerSecond);
        }

        public static int EvalFrameCount(float durationMs, float framesPerSecond)
        {
            if (framesPerSecond <= 0)
                throw new ArgumentOutOfRangeException(nameof(framesPerSecond));

            return (int)Math.Floor(durationMs / millisecondsInSecond * framesPerSecond);
        }

        public static float EvalMillisecondsForFrame(int frame, float framesPerSecond, float startMs = 0.0f)
        {
            if (framesPerSecond <= 0)
                throw new ArgumentOutOfRangeException(nameof(framesPerSecond));

            return startMs + frame / framesPerSecond * millisecondsInSecond;
        }
    }
}
