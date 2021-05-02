using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Base class for all shapes drawn on the scene.
    /// </summary>
    public abstract class Shape : Visual
    {

        #region Brush managed property

        /// <summary>
        /// Defines, how shape will be filled.
        /// </summary>
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

        /// <summary>
        /// Defines, how edges of the shape will be drawn.
        /// </summary>
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
