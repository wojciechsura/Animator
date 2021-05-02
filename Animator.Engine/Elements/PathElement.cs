using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Base class for all path elements.
    /// </summary>
    public abstract class PathElement : BaseElement
    {
        // Protected types ----------------------------------------------------

        /// <summary>
        /// Utility class to simplify handling relative positions
        /// </summary>
        protected class RunningPoint
        {
            private PointF point;

            public RunningPoint(PointF start)
            {
                point = start;
            }

            public PointF Delta(PointF delta)
            {
                point = new PointF(point.X + delta.X, point.Y + delta.Y);
                return point;
            }

            public PointF Current => point;
        }

        // Protected methods --------------------------------------------------

        protected string F(float value) => string.Format(CultureInfo.InvariantCulture, "{0:.##}", value);

        // Internal methods -----------------------------------------------------

        internal abstract (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path);

        internal abstract string ToPathString();        
    }
}
