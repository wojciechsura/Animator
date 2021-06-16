using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public class ScaleTransform : Transform
    {
        // Internal methods ---------------------------------------------------

        internal override Matrix GetMatrix()
        {
            Matrix result = new Matrix();
            result.Translate(-Center.X, -Center.Y, MatrixOrder.Append);
            result.Scale(SX, SY, MatrixOrder.Append);
            result.Translate(Center.X, Center.Y, MatrixOrder.Append);

            return result;
        }

        // Public properties --------------------------------------------------

        #region Center managed property

        public PointF Center
        {
            get => (PointF)GetValue(CenterProperty);
            set => SetValue(CenterProperty, value);
        }

        public static readonly ManagedProperty CenterProperty = ManagedProperty.Register(typeof(ScaleTransform),
            nameof(Center),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(0.0f, 0.0f) });

        #endregion

        #region SX managed property

        /// <summary>
        /// Defines scale in X axis
        /// </summary>
        public float SX
        {
            get => (float)GetValue(SXProperty);
            set => SetValue(SXProperty, value);
        }

        public static readonly ManagedProperty SXProperty = ManagedProperty.Register(typeof(ScaleTransform),
            nameof(SX),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 1.0f });

        #endregion

        #region SY managed property

        /// <summary>
        /// Defines scale in Y axis
        /// </summary>
        public float SY
        {
            get => (float)GetValue(SYProperty);
            set => SetValue(SYProperty, value);
        }

        public static readonly ManagedProperty SYProperty = ManagedProperty.Register(typeof(ScaleTransform),
            nameof(SY),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 1.0f });

        #endregion
    }
}
