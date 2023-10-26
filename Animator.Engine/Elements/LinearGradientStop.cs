using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Defines a stop in complex linear gradient.
    /// </summary>
    public partial class LinearGradientStop : SceneElement
    {
        // Public properties --------------------------------------------------

        #region Position managed property

        /// <summary>
        /// Position of the stop. 0.0f equals to start point, 1.0f - to end point.
        /// Values outside this range are allowed to define gradient on the outside
        /// as well.
        /// </summary>
        public float Position
        {
            get => (float)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        public static readonly ManagedProperty PositionProperty = ManagedProperty.Register(typeof(LinearGradientStop),
            nameof(Position),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 0.0f });

        #endregion

        #region Color managed property

        /// <summary>
        /// Color of this gradient stop.
        /// </summary>
        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public static readonly ManagedProperty ColorProperty = ManagedProperty.Register(typeof(LinearGradientStop),
            nameof(Color),
            typeof(Color),
            new ManagedSimplePropertyMetadata { DefaultValue = Color.White });

        #endregion
    }
}
