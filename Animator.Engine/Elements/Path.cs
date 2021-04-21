using Animator.Engine.Base;
using Animator.Engine.Elements.Persistence;
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

        protected override void InternalRender(Bitmap bitmap, Graphics graphics)
        {
            GraphicsPath path = new GraphicsPath();
            PointF start = new PointF(0.0f, 0.0f);
            PointF lastControlPoint = new PointF(0.0f, 0.0f);

            foreach (var pathElement in Definition)
                (start, lastControlPoint) = pathElement.AddToGeometry(start, lastControlPoint, path);

            if (IsPropertySet(BrushProperty) && Brush != null)
            {
                using (var brush = Brush.BuildBrush())
                    graphics.FillPath(brush, path);
            }

            if (IsPropertySet(PenProperty) && Pen != null)
            {
                using (var pen = Pen.BuildPen())
                    graphics.DrawPath(pen, path);
            }            
        }

        // Public properties --------------------------------------------------

        #region Definition managed collection

        public List<PathElement> Definition
        {
            get => (List<PathElement>)GetValue(DefinitionProperty);
        }

        public static readonly ManagedProperty DefinitionProperty = ManagedProperty.RegisterCollection(typeof(Path),
            nameof(Definition),
            typeof(List<PathElement>),
            new ManagedCollectionMetadata(() => new List<PathElement>()));

        #endregion
    }
}
