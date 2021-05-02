using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Describes animation parameters.
    /// </summary>
    public class AnimationConfig : ManagedObject
    {
        #region Width managed property

        /// <summary>
        /// Width of the result animation, in pixels.
        /// </summary>
        public int Width
        {
            get => (int)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }

        public static readonly ManagedProperty WidthProperty = ManagedProperty.Register(typeof(AnimationConfig), 
            nameof(Width), 
            typeof(int), 
            new ManagedSimplePropertyMetadata { DefaultValue = 1024, CoerceValueHandler = CoerceWidth });

        private static object CoerceWidth(ManagedObject obj, object baseValue)
        {
            return Math.Max(1, (int)baseValue);
        }

        #endregion

        #region Height managed property

        /// <summary>
        /// Height of the result animation, in pixels.
        /// </summary>
        public int Height
        {
            get => (int)GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }

        public static readonly ManagedProperty HeightProperty = ManagedProperty.Register(typeof(AnimationConfig),
            nameof(Height),
            typeof(int),
            new ManagedSimplePropertyMetadata { DefaultValue = 768, CoerceValueHandler = CoerceHeight });

        private static object CoerceHeight(ManagedObject obj, object baseValue)
        {
            return Math.Max(1, (int)baseValue);
        }

        #endregion

        #region FramesPerSecond managed property

        /// <summary>
        /// Defines, how many frames should be in a second of animation.
        /// </summary>
        public float FramesPerSecond
        {
            get => (float)GetValue(FramesPerSecondProperty);
            set => SetValue(FramesPerSecondProperty, value);
        }

        public static readonly ManagedProperty FramesPerSecondProperty = ManagedProperty.Register(typeof(AnimationConfig),
            nameof(FramesPerSecond),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 30.0f });

        #endregion
    }
}
