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
    public class Rectangle : Shape
    {
        protected override void InternalRender(BitmapBuffer buffer)
        {
            if (IsPropertySet(BrushProperty))
            {
                using (var brush = Brush.BuildBrush())
                {
                    RectangleF rect = new RectangleF(0.0f, 0.0f, Width, Height);
                    buffer.Graphics.FillRectangle(brush, rect);
                }
            }

            if (IsPropertySet(PenProperty))
            {
                using (var pen = Pen.BuildPen())
                    buffer.Graphics.DrawRectangle(pen, 0.0f, 0.0f, Width, Height);
            }
        }

        #region Width managed property

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
