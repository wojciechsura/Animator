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
    public class RotateTransform : Transform
    {
        // Internal methods ---------------------------------------------------

        internal override Matrix GetMatrix()
        {
            Matrix result = new Matrix();
            result.Translate(-Center.X, -Center.Y, MatrixOrder.Append);
            result.Rotate(Angle, MatrixOrder.Append);
            result.Translate(Center.X, Center.Y, MatrixOrder.Append);

            return result;
        }

        // Public properties --------------------------------------------------

        #region Center managed property

        /// <summary>
        /// Defines center of rotation
        /// </summary>
        public PointF Center
        {
            get => (PointF)GetValue(CenterProperty);
            set => SetValue(CenterProperty, value);
        }

        public static readonly ManagedProperty CenterProperty = ManagedProperty.Register(typeof(RotateTransform),
            nameof(Center),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(0.0f, 0.0f) });

        #endregion

        #region Angle managed property

        /// <summary>
        /// Defines angle of rotation.
        /// </summary>
        public float Angle
        {
            get => (float)GetValue(AngleProperty);
            set => SetValue(AngleProperty, value);
        }

        public static readonly ManagedProperty AngleProperty = ManagedProperty.Register(typeof(RotateTransform),
            nameof(Angle),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 0.0f });

        #endregion
    }
}
