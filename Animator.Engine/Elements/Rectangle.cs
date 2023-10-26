using Animator.Engine.Base;
using Animator.Engine.Elements.Rendering;
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
    /// Draws rectangle on a scene.
    /// </summary>
    public partial class Rectangle : Shape
    {
        protected override void InternalRender(BitmapBuffer buffer, BitmapBufferRepository buffers, RenderingContext context)
        {
            if (IsPropertySet(BrushProperty))
            {
                System.Drawing.Brush brush;
                using (brush = Brush.BuildBrush())
                {
                    RectangleF rect = new RectangleF(0.0f, 0.0f, Width, Height);
                    buffer.Graphics.FillRectangle(brush, rect);
                }
                brush = null;
            }

            if (IsPropertySet(PenProperty))
            {
                System.Drawing.Pen pen;
                using (pen = Pen.BuildPen())
                    buffer.Graphics.DrawRectangle(pen, 0.0f, 0.0f, Width, Height);
                pen = null;
            }
        }
        
        #region Width managed property

        /// <summary>
        /// Width of the rectangle.
        /// </summary>
        public float Width
        {
            get => (float)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }

        public static readonly ManagedProperty WidthProperty = ManagedProperty.Register(typeof(Rectangle),
            nameof(Width),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 0.0f });

        #endregion

        #region Height managed property

        /// <summary>
        /// Height of the rectangle.
        /// </summary>
        public float Height
        {
            get => (float)GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }

        public static readonly ManagedProperty HeightProperty = ManagedProperty.Register(typeof(Rectangle),
            nameof(Height),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 0.0f });

        #endregion
    }
}
