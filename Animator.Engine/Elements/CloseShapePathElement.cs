using System.Drawing;
using System.Drawing.Drawing2D;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Informs, that the shape defined by a series of path elements
    /// ends here. If needed, path is closed with a line leading to
    /// its start.
    /// </summary>
    public class CloseShapePathElement : PathElement
    {
        // Internal methods ---------------------------------------------------

        internal override (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path)
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

            return (new PointF(0.0f, 0.0f), new PointF(0.0f, 0.0f));
        }

        internal override string ToPathString() => "Z";
    }
}