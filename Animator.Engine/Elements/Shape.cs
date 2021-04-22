using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public abstract class Shape : Visual
    {

        #region Brush managed property

        public Brush Brush
        {
            get => (Brush)GetValue(BrushProperty);
            set => SetValue(BrushProperty, value);
        }

        public static readonly ManagedProperty BrushProperty = ManagedProperty.RegisterReference(typeof(Shape),
            nameof(Brush),
            typeof(Brush),
            new ManagedReferencePropertyMetadata());

        #endregion


        #region Pen managed property

        public Pen Pen
        {
            get => (Pen)GetValue(PenProperty);
            set => SetValue(PenProperty, value);
        }

        public static readonly ManagedProperty PenProperty = ManagedProperty.RegisterReference(typeof(Shape),
            nameof(Pen),
            typeof(Pen),
            new ManagedReferencePropertyMetadata());

        #endregion
    }
}
