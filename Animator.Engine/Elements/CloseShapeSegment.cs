using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Informs, that the shape defined by a series of path elements
    /// ends here. If needed, path is closed with a line leading to
    /// its start.
    /// </summary>
    public partial class CloseShapeSegment : Segment
    {
        // Internal methods ---------------------------------------------------

        internal override (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path)
        {
            if (path != null)
            {
                var pathData = path.PathData;

                if (pathData.Points.Length > 0)
                {
                    // Important for custom line caps. Force the path the close with an explicit line, not just an implicit close of the figure.
                    var last = pathData.Points.Length - 1;
                    if (!pathData.Points[0].Equals(pathData.Points[last]))
                    {
                        var i = last;
                        while (i > 0 && pathData.Types[i] > 0) --i;
                        path.AddLine(pathData.Points[last], pathData.Points[i]);
                    }

                    path.CloseFigure();
                }
            }

            return (PointF.Empty, PointF.Empty);
        }

        internal override (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path, float? cutFrom, float? cutTo)
        {
            return AddToGeometry(start, lastControlPoint, path);
        }

        internal override (float length, PointF endPoint, PointF lastControlPoint) EvalLength(PointF start, PointF lastControlPoint)
        {
            return (0.0f, PointF.Empty, PointF.Empty);
        }

        internal override string ToPathString() => "Z";
    }
}