using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Brush, which fills an area with a single color.
    /// </summary>
    public partial class SolidBrush : Brush
    {
        // Internal methods ---------------------------------------------------

        internal override System.Drawing.Brush BuildBrush()
        {
            return new System.Drawing.SolidBrush(Color);
        }

        // Public properties --------------------------------------------------

        #region Color managed property

        /// <summary>
        /// Color of the fill.
        /// </summary>
        public System.Drawing.Color Color
        {
            get => (System.Drawing.Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public static readonly ManagedProperty ColorProperty = ManagedProperty.Register(typeof(SolidBrush),
            nameof(Color),
            typeof(System.Drawing.Color),
            new ManagedSimplePropertyMetadata { DefaultValue = System.Drawing.Color.Transparent });

        #endregion
    }
}
