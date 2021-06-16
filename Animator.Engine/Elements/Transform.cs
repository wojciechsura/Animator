using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Base class for linear transform definitions
    /// </summary>
    public abstract class Transform : SceneElement
    {
        // Internal methods ---------------------------------------------------

        internal abstract Matrix GetMatrix(Matrix currentMatrix, float multiplier = 1.0f);

        // Public properties --------------------------------------------------


        #region UseLocalCoords managed property

        /// <summary>
        /// Defines, whether local coords (ie. not affected by previous transforms)
        /// should be used for this transform. Note, that this takes effect only on
        /// those transforms, which use points (rotation, scale)
        /// </summary>
        public bool UseLocalCoords
        {
            get => (bool)GetValue(UseLocalCoordsProperty);
            set => SetValue(UseLocalCoordsProperty, value);
        }

        public static readonly ManagedProperty UseLocalCoordsProperty = ManagedProperty.Register(typeof(Transform),
            nameof(UseLocalCoords),
            typeof(bool),
            new ManagedSimplePropertyMetadata { DefaultValue = true });

        #endregion
    }
}
