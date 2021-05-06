using Animator.Engine.Base;
using Animator.Engine.Elements.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Draws a circle on a scene.
    /// </summary>
    public class Circle : Shape
    {
        protected override void InternalRender(BitmapBuffer buffer, BitmapBufferRepository buffers)
        {
            if (IsPropertySet(BrushProperty))
            {
                using System.Drawing.Brush brush = Brush.BuildBrush();
                buffer.Graphics.FillEllipse(brush, Center.X - Radius, Center.Y - Radius, 2 * Radius, 2 * Radius);
            }

            if (IsPropertySet(PenProperty))
            {
                using System.Drawing.Pen pen = Pen.BuildPen();
                buffer.Graphics.DrawEllipse(pen, Center.X - Radius, Center.Y - Radius, 2 * Radius, 2 * Radius);
            }
        }

        #region Center managed property

        /// <summary>
        /// Center point of the circle.
        /// </summary>
        public PointF Center
        {
            get => (PointF)GetValue(CenterProperty);
            set => SetValue(CenterProperty, value);
        }

        public static readonly ManagedProperty CenterProperty = ManagedProperty.Register(typeof(Circle),
            nameof(Center),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(0.0f, 0.0f) });

        #endregion

        #region Radius managed property

        /// <summary>
        /// Radius of the circle.
        /// </summary>
        public float Radius
        {
            get => (float)GetValue(RadiusProperty);
            set => SetValue(RadiusProperty, value);
        }

        public static readonly ManagedProperty RadiusProperty = ManagedProperty.Register(typeof(Circle),
            nameof(Radius),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 0.0f });

        #endregion
    }
}
