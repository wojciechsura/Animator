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
    /// Draws a line on the scene.
    /// </summary>
    public class Line : Visual
    {
        // Protected methods --------------------------------------------------

        protected override void InternalRender(BitmapBuffer buffer, BitmapBufferRepository buffers, RenderingContext context)
        {
            if (IsPropertySet(PenProperty))
            {
                using (var pen = Pen.BuildPen())
                    buffer.Graphics.DrawLine(pen, Start, End);
            }
        }

        // Public properties --------------------------------------------------

        #region Start managed property

        /// <summary>
        /// Point defining start of the line.
        /// </summary>
        public PointF Start
        {
            get => (PointF)GetValue(StartProperty);
            set => SetValue(StartProperty, value);
        }

        public static readonly ManagedProperty StartProperty = ManagedProperty.Register(typeof(Line),
            nameof(Start),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(0, 0) });

        #endregion

        #region End managed property

        /// <summary>
        /// Point defining end of the line.
        /// </summary>
        public PointF End
        {
            get => (PointF)GetValue(EndProperty);
            set => SetValue(EndProperty, value);
        }

        public static readonly ManagedProperty EndProperty = ManagedProperty.Register(typeof(Line),
            nameof(End),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(0, 0) });

        #endregion

        #region Pen managed property

        /// <summary>
        /// Describes visual properties of the drawn line.
        /// </summary>
        public Pen Pen
        {
            get => (Pen)GetValue(PenProperty);
            set => SetValue(PenProperty, value);
        }

        public static readonly ManagedProperty PenProperty = ManagedProperty.RegisterReference(typeof(Line),
            nameof(Pen),
            typeof(Pen),
            new ManagedReferencePropertyMetadata());

        #endregion
    }
}
