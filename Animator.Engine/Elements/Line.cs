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
    public class Line : Visual
    {
        // Protected methods --------------------------------------------------

        protected override void InternalRender(BitmapBuffer buffer, BitmapBufferRepository buffers)
        {
            if (IsPropertySet(PenProperty))
            {
                using (var pen = Pen.BuildPen())
                    buffer.Graphics.DrawLine(pen, Start, End);
            }
        }

        // Public properties --------------------------------------------------

        #region Start managed property

        public PointF Start
        {
            get => (PointF)GetValue(StartProperty);
            set => SetValue(StartProperty, value);
        }

        public static readonly ManagedProperty StartProperty = ManagedProperty.Register(typeof(Line),
            nameof(Start),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = 0 });

        #endregion

        #region End managed property

        public PointF End
        {
            get => (PointF)GetValue(EndProperty);
            set => SetValue(EndProperty, value);
        }

        public static readonly ManagedProperty EndProperty = ManagedProperty.Register(typeof(Line),
            nameof(End),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = 0 });

        #endregion

        #region Pen managed property

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
