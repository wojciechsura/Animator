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
    /// Draws an ellipse on the scene.
    /// </summary>
    public class Ellipse : Shape
    {
        // Protected methods --------------------------------------------------

        protected override void InternalRender(BitmapBuffer buffer, BitmapBufferRepository buffers)
        {
            if (IsPropertySet(BrushProperty))
            {
                using System.Drawing.Brush brush = Brush.BuildBrush();
                buffer.Graphics.FillEllipse(brush, new RectangleF(TopLeft.X, TopLeft.Y, BottomRight.X - TopLeft.X, BottomRight.Y - TopLeft.Y));
            }

            if (IsPropertySet(PenProperty))
            {
                using System.Drawing.Pen pen = Pen.BuildPen();
                buffer.Graphics.DrawEllipse(pen, new RectangleF(TopLeft.X, TopLeft.Y, BottomRight.X - TopLeft.X, BottomRight.Y - TopLeft.Y));
            }
        }

        // Public properties --------------------------------------------------

        #region TopLeft managed property

        /// <summary>
        /// Defines top and left coordinates of the rectangle
        /// circumscribing the desired ellipse.
        /// </summary>
        public PointF TopLeft
        {
            get => (PointF)GetValue(TopLeftProperty);
            set => SetValue(TopLeftProperty, value);
        }

        public static readonly ManagedProperty TopLeftProperty = ManagedProperty.Register(typeof(Ellipse),
            nameof(TopLeft),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = 0 });

        #endregion

        #region BottomRight managed property

        /// <summary>
        /// Defines bottom and right coordinates of the rectangle
        /// circumscribing the desired ellipse.
        /// </summary>
        public PointF BottomRight
        {
            get => (PointF)GetValue(BottomRightProperty);
            set => SetValue(BottomRightProperty, value);
        }

        public static readonly ManagedProperty BottomRightProperty = ManagedProperty.Register(typeof(Ellipse),
            nameof(BottomRight),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = 0 });

        #endregion
    }
}
