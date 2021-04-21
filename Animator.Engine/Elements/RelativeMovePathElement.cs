using Animator.Engine.Base;
using Animator.Engine.Utils;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
    public class RelativeMovePathElement : PathElement
    {
        // Internal methods ---------------------------------------------------

        internal override string ToPathString() => $"m {F(DeltaEndPoint.X)} {F(DeltaEndPoint.Y)}";

        internal override (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path)
        {
            path.StartFigure();

            PointF endPoint = start.Add(DeltaEndPoint);

            return (endPoint, endPoint);
        }

        // Public properties --------------------------------------------------

        #region DeltaEndPoint managed property

        public PointF DeltaEndPoint
        {
            get => (PointF)GetValue(DeltaEndPointProperty);
            set => SetValue(DeltaEndPointProperty, value);
        }

        public static readonly ManagedProperty DeltaEndPointProperty = ManagedProperty.Register(typeof(RelativeMovePathElement),
            nameof(DeltaEndPoint),
            typeof(PointF),
            new ManagedAnimatedPropertyMetadata(new PointF(0.0f, 0.0f)));

        #endregion
    }
}