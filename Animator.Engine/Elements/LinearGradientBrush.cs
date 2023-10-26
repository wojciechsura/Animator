using Animator.Engine.Base;
using Animator.Engine.Base.Persistence;
using Animator.Engine.Exceptions;
using Animator.Engine.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Brush, which fills shape with a linear gradient.
    /// </summary>
    [ContentProperty(nameof(Stops))]
    public partial class LinearGradientBrush : Brush
    {
        internal override System.Drawing.Brush BuildBrush()
        {
            System.Drawing.Brush brush;

            if (Point1.DistanceTo(Point2).IsZero())
            {
                brush = new System.Drawing.SolidBrush(Color1);
            }
            else
            {
                if (Stops.Any())
                {
                    if (Stops.Count < 2)
                        throw new AnimationException("You should specify at least two steps.", GetHumanReadablePath());

                    var gradientBrush = new System.Drawing.Drawing2D.LinearGradientBrush(Point1, Point2, Color.Transparent, Color.Transparent);

                    var blend = new System.Drawing.Drawing2D.ColorBlend();
                    blend.Colors = Stops.Select(s => s.Color).ToArray();
                    blend.Positions = Stops.Select(s => s.Position).ToArray();

                    gradientBrush.InterpolationColors = blend;

                    brush = gradientBrush;
                }
                else
                {
                    brush = new System.Drawing.Drawing2D.LinearGradientBrush(Point1, Point2, Color1, Color2);
                }
            }

            return brush;
        }

        // Public properties --------------------------------------------------

        #region Point1 managed property

        /// <summary>
        /// Start point of the gradient
        /// </summary>
        public PointF Point1
        {
            get => (PointF)GetValue(Point1Property);
            set => SetValue(Point1Property, value);
        }

        public static readonly ManagedProperty Point1Property = ManagedProperty.Register(typeof(LinearGradientBrush),
            nameof(Point1),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(0.0f, 0.0f) });

        #endregion

        #region Point2 managed property

        /// <summary>
        /// End point of the gradient
        /// </summary>
        public PointF Point2
        {
            get => (PointF)GetValue(Point2Property);
            set => SetValue(Point2Property, value);
        }

        public static readonly ManagedProperty Point2Property = ManagedProperty.Register(typeof(LinearGradientBrush),
            nameof(Point2),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = new PointF(1.0f, 1.0f) });

        #endregion

        #region Color1 managed property

        /// <summary>
        /// Start color of the gradient
        /// </summary>
        public Color Color1
        {
            get => (Color)GetValue(Color1Property);
            set => SetValue(Color1Property, value);
        }

        public static readonly ManagedProperty Color1Property = ManagedProperty.Register(typeof(LinearGradientBrush),
            nameof(Color1),
            typeof(Color),
            new ManagedSimplePropertyMetadata { DefaultValue = Color.Black });

        #endregion

        #region Color2 managed property

        /// <summary>
        /// End color of the gradient
        /// </summary>
        public Color Color2
        {
            get => (Color)GetValue(Color2Property);
            set => SetValue(Color2Property, value);
        }

        public static readonly ManagedProperty Color2Property = ManagedProperty.Register(typeof(LinearGradientBrush),
            nameof(Color2),
            typeof(Color),
            new ManagedSimplePropertyMetadata { DefaultValue = Color.White });

        #endregion

        #region Stops managed collection

        /// <summary>
        /// Defines additional stops for the gradient. Note, that if you
        /// use Steps, values of Color1 and Color2 are ignored.
        /// </summary>
        public ManagedCollection<LinearGradientStop> Stops
        {
            get => (ManagedCollection<LinearGradientStop>)GetValue(StopsProperty);
        }

        public static readonly ManagedProperty StopsProperty = ManagedProperty.RegisterCollection(typeof(LinearGradientBrush),
            nameof(Stops),
            typeof(ManagedCollection<LinearGradientStop>));

        #endregion
    }
}
