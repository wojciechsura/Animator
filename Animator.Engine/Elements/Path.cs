using Animator.Engine.Base;
using Animator.Engine.Elements.Persistence;
using Animator.Engine.Elements.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{    
    /// <summary>
    /// Draws a path on a scene.
    /// </summary>
    public class Path : Shape
    {
        // Private methods ----------------------------------------------------

        private void PaintPath(BitmapBuffer buffer, GraphicsPath path)
        {
            if (IsPropertySet(BrushProperty) && Brush != null)
            {
                using (var brush = Brush.BuildBrush())
                    buffer.Graphics.FillPath(brush, path);
            }

            if (IsPropertySet(PenProperty) && Pen != null)
            {
                using (var pen = Pen.BuildPen())
                    buffer.Graphics.DrawPath(pen, path);
            }
        }

        // Protected methods --------------------------------------------------

        protected override void InternalRender(BitmapBuffer buffer, BitmapBufferRepository buffers)
        {
            if (!IsPropertySet(CutFromProperty) && !IsPropertySet(CutToProperty))
            {
                RenderFullPath(buffer);
            }
            else
            {
                RenderCutPath(buffer, IsPropertySet(CutFromProperty) ? CutFrom : null, IsPropertySet(CutToProperty) ? CutTo : null);
            }
        }

        private void RenderFullPath(BitmapBuffer buffer)
        {
            GraphicsPath path = new();
            PointF start = new(0.0f, 0.0f);
            PointF lastControlPoint = new(0.0f, 0.0f);

            foreach (var pathElement in Definition)
                (start, lastControlPoint) = pathElement.AddToGeometry(start, lastControlPoint, path);
            
            PaintPath(buffer, path);
        }

        private void RenderCutPath(BitmapBuffer buffer, float? from, float? to)
        {
            // Checks
            if (from.HasValue && (from.Value < 0 || from.Value > 1))
                throw new ArgumentOutOfRangeException(nameof(from));
            if (to.HasValue && (to.Value < 0 || to.Value > 1))
                throw new ArgumentOutOfRangeException(nameof(to));

            // Make sure, that from and to are in proper order.
            if (from.HasValue && to.HasValue && from.Value > to.Value)
            {
                float tmp = to.Value;
                to = from.Value;
                from = tmp;
            }

            // Evaluate lengths of elements and total length of the whole path
            PointF start = new(0.0f, 0.0f);
            PointF lastControlPoint = new(0.0f, 0.0f);

            List<float> lengths = new();
            for (int i = 0; i < Definition.Count; i++)
            {
                var pathElement = Definition[i];

                float length;
                (length, start, lastControlPoint) = pathElement.EvalLength(start, lastControlPoint);

                lengths.Add(length);
            }

            var totalLength = lengths.Sum();

            // Find start element/factor and end element/factor
            int startElement = -1;
            float startFactor = float.NaN;
            int endElement = Definition.Count;
            float endFactor = float.NaN;

            void evalElementAndFactor(float? absolutePosition, ref int element, ref float factor)
            {
                if (absolutePosition.HasValue)
                {
                    var positionLength = totalLength * absolutePosition.Value;

                    float lengthAcc = 0.0f;
                    int i = 0;
                    while (i < Definition.Count && lengthAcc + lengths[i] < positionLength)
                    {
                        lengthAcc += lengths[i];
                        i++;
                    }

                    // In this case there is nothing to draw (ie. from = 1)
                    if (i == Definition.Count)
                        return;

                    element = i;

                    // FromLength now should be smaller than lengths[i]
                    positionLength -= lengthAcc;
                    factor = lengths[i] > 0 ? positionLength / lengths[i] : 0.0f;
                }
            }

            evalElementAndFactor(from, ref startElement, ref startFactor);
            evalElementAndFactor(to, ref endElement, ref endFactor);

            // Now draw
            GraphicsPath path = new();
            start = new(0.0f, 0.0f);
            lastControlPoint = new(0.0f, 0.0f);

            // Special case, single element cut in two places
            if (startElement == endElement)
            {
                // We have to pass through earlier elements, because we need proper start point
                for (int i = 0; i < startElement; i++)
                    (start, lastControlPoint) = Definition[i].AddToGeometry(start, lastControlPoint, null);

                // Adding segment with two cuts
                (start, lastControlPoint) = Definition[startElement].AddToGeometry(start, lastControlPoint, path, startFactor, endFactor);
                
                // If there is any following close-shape element, run it
                var closeElement = Definition.Skip(startElement + 1)
                    .OfType<CloseShapeSegment>()
                    .FirstOrDefault();

                if (closeElement != null)
                    closeElement.AddToGeometry(start, lastControlPoint, path);
            }
            else
            {
                // Start element
                if (startElement >= 0)
                {
                    for (int i = 0; i < startElement; i++)
                        (start, lastControlPoint) = Definition[i].AddToGeometry(start, lastControlPoint, null);

                    (start, lastControlPoint) = Definition[startElement].AddToGeometry(start, lastControlPoint, path, startFactor, null);
                }

                // Middle elements (if any)
                for (int i = startElement + 1; i < endElement; i++)
                    (start, lastControlPoint) = Definition[i].AddToGeometry(start, lastControlPoint, path);

                // Last element
                if (endElement < Definition.Count)
                {
                    (start, lastControlPoint) = Definition[endElement].AddToGeometry(start, lastControlPoint, path, null, endFactor);

                    // If there is any following close-shape element, run it
                    var closeElement = Definition.Skip(endElement + 1)
                        .OfType<CloseShapeSegment>()
                        .FirstOrDefault();

                    if (closeElement != null)
                        closeElement.AddToGeometry(start, lastControlPoint, path);
                }
            }

            PaintPath(buffer, path);
        }

        // Public properties --------------------------------------------------

        #region Definition managed collection

        /// <summary>
        /// List containing path elements, which define its shape.
        /// In XML, you may express value of this property in an
        /// attribute as SVG-compatible path description.
        /// </summary>
        public ManagedCollection<Segment> Definition
        {
            get => (ManagedCollection<Segment>)GetValue(DefinitionProperty);
        }

        public static readonly ManagedProperty DefinitionProperty = ManagedProperty.RegisterCollection(typeof(Path),
            nameof(Definition),
            typeof(ManagedCollection<Segment>),
            new ManagedCollectionMetadata { CustomSerializer = new PathElementsSerializer() });

        #endregion

        #region CutFrom managed property

        /// <summary>
        /// If set, defines first split point of the path. The path
        /// before this value will not be drawn. CutFrom must fit
        /// between 0 and 1, inclusive.
        /// </summary>
        public float CutFrom
        {
            get => (float)GetValue(CutFromProperty);
            set => SetValue(CutFromProperty, value);
        }

        public static readonly ManagedProperty CutFromProperty = ManagedProperty.Register(typeof(Path),
            nameof(CutFrom),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 0.0f, CoerceValueHandler = CoerceCutFrom });

        private static object CoerceCutFrom(ManagedObject obj, object baseValue)
        {
            float value = (float)baseValue;

            if (value < 0)
                value = 0;
            if (value > 1)
                value = 1;

            return value;
        }

        #endregion

        #region CutTo managed property

        /// <summary>
        /// If set, defines last split point of the path. The path
        /// after this value will not be drawn. CutTo must fit
        /// between 0 and 1, inclusive.
        /// </summary>
        public float CutTo
        {
            get => (float)GetValue(CutToProperty);
            set => SetValue(CutToProperty, value);
        }

        public static readonly ManagedProperty CutToProperty = ManagedProperty.Register(typeof(Path),
            nameof(CutTo),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 1.0f, CoerceValueHandler = CoerceCutTo });

        private static object CoerceCutTo(ManagedObject obj, object baseValue)
        {
            float value = (float)baseValue;

            if (value < 0)
                value = 0;
            if (value > 1)
                value = 1;

            return value;
        }

        #endregion
    }
}
