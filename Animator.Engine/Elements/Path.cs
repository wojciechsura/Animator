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
    public class Path : Shape
    {
        // Protected methods --------------------------------------------------

        protected override void InternalRender(BitmapBuffer buffer, BitmapBufferRepository buffers)
        {
            GraphicsPath path = new GraphicsPath();
            PointF start = new PointF(0.0f, 0.0f);
            PointF lastControlPoint = new PointF(0.0f, 0.0f);

            foreach (var pathElement in Definition)
                (start, lastControlPoint) = pathElement.AddToGeometry(start, lastControlPoint, path);

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

        // Public properties --------------------------------------------------

        #region Definition managed collection

        public ManagedCollection<PathElement> Definition
        {
            get => (ManagedCollection<PathElement>)GetValue(DefinitionProperty);
        }

        public static readonly ManagedProperty DefinitionProperty = ManagedProperty.RegisterCollection(typeof(Path),
            nameof(Definition),
            typeof(ManagedCollection<PathElement>));

        #endregion
    }
}
