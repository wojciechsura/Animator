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

            foreach (var pathElement in Items)
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

        #region Items managed collection

        public List<PathElement> Items
        {
            get => (List<PathElement>)GetValue(ItemsProperty);
        }

        public static readonly ManagedProperty ItemsProperty = ManagedProperty.RegisterCollection(typeof(Path),
            nameof(Items),
            typeof(List<PathElement>),
            new ManagedCollectionMetadata(() => new List<PathElement>(), new PathElementsSerializer()));

        #endregion
    }
}
