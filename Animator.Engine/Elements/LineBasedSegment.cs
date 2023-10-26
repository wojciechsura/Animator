using Animator.Engine.Base;
using Animator.Engine.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public abstract partial class LineBasedSegment : Segment
    {
        // Private fields -----------------------------------------------------

        private bool lengthValid = false;
        private float length;

        private bool lineValid;
        private PointF[] line;

        private PointF cachedStart;

        // Private methods ----------------------------------------------------

        private void NotifyLineChanged()
        {
            InvalidateLine();
        }

        private void ValidateLine(PointF start)
        {
            line = BuildLine(start);
            if (line.Length != 2)
                throw new InvalidOperationException("Invalid implementation of BuildLine! Should return array of exactly 2 points.");
            lineValid = true;

            InvalidateLength();
        }

        private void ValidateLength(PointF start)
        {
            ValidateLine(start);

            length = line[0].DistanceTo(line[1]);
            lengthValid = true;
        }

        // Protected methods --------------------------------------------------

        protected void InvalidateLength()
        {
            length = float.NaN;
            lengthValid = false;
        }

        protected void InvalidateLine()
        {
            lineValid = false;
            line = null;
            InvalidateLength();
        }

        protected abstract PointF[] BuildLine(PointF start);

        protected PointF[] GetLine(PointF start)
        {
            // Note: floating point equality is on purpose.
            // That's because we're not relying on floating point
            // operation precision, but on their consistency.
            if (!lineValid || start != cachedStart)
            {
                ValidateLine(start);

                cachedStart = start;
            }

            return line;
        }

        protected static void HandleLineChanged(ManagedObject sender, PropertyValueChangedEventArgs args)
        {
            if (sender is LineBasedSegment lineElement)
                lineElement.NotifyLineChanged();
        }

        // Internal methods ---------------------------------------------------

        internal override (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path)
        {
            PointF[] line = GetLine(start);

            if (path != null)
                path.AddLine(line[0], line[1]);

            return (line[1], line[1]);
        }

        internal override (PointF endPoint, PointF lastControlPoint) AddToGeometry(PointF start, PointF lastControlPoint, GraphicsPath path, float? cutFrom, float? cutTo)
        {
            PointF[] line = GetLine(start);

            PointF v = line[1].Subtract(line[0]);

            if (cutFrom == null)
                cutFrom = 0.0f;
            if (cutTo == null)
                cutTo = 1.0f;

            PointF pointFrom = line[0].Add(v.Multiply(cutFrom.Value));
            PointF pointTo = line[0].Add(v.Multiply(cutTo.Value));

            if (path != null)
                path.AddLine(pointFrom, pointTo);

            return (pointTo, pointTo);
        }

        internal override (float length, PointF endPoint, PointF lastControlPoint) EvalLength(PointF start, PointF lastControlPoint)
        {
            if (!lengthValid)
                ValidateLength(start);

            return (length, line[1], line[1]);
        }
    }
}
