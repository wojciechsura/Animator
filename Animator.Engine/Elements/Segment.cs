﻿using Animator.Engine.Base;
using Animator.Engine.Utils;
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
    public abstract partial class Segment : SceneElement
    {
        // Protected types ----------------------------------------------------

        /// <summary>
        /// Utility class to simplify handling relative positions
        /// </summary>
        protected class RelativePoint
        {
            private PointF point;

            public RelativePoint(PointF start)
            {
                point = start;
            }

            public PointF Delta(PointF delta)
            {
                return new PointF(point.X + delta.X, point.Y + delta.Y);                
            }
        }

        // Protected methods --------------------------------------------------

        /// <summary>
        /// Formats a float into a SVG path-compatible string.
        /// </summary>
        protected string F(float value) => string.Format(CultureInfo.InvariantCulture, "{0:.##}", value);

        // Internal methods -----------------------------------------------------

        internal abstract (float length, PointF endPoint, PointF lastControlPoint) EvalLength(PointF start, PointF lastControlPoint);

        internal abstract (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path);

        internal abstract (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path, float? cutFrom, float? cutTo);

        internal abstract string ToPathString();        
    }
}
