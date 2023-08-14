using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Utils
{
    public static class FloatExtensions
    {
        private const float EPSILON = 0.0000000001f;

        public static bool IsZero(this float value) => Math.Abs(value) < EPSILON;
    }
}
