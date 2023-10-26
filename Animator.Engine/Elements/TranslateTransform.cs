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
    /// Represents a translation transform
    /// </summary>
    public partial class TranslateTransform : Transform
    {
        // Internal methods ---------------------------------------------------
        
        internal override Matrix GetMatrix(Matrix currentTransform, float multiplier)
        {
            Matrix result = new Matrix();

            result.Translate(DX * multiplier, DY * multiplier, MatrixOrder.Append);

            return result;
        }

        // Public properties --------------------------------------------------

        #region DX managed property

        /// <summary>
        /// Defines translation distance over X axis
        /// </summary>
        public float DX
        {
            get => (float)GetValue(DXProperty);
            set => SetValue(DXProperty, value);
        }

        public static readonly ManagedProperty DXProperty = ManagedProperty.Register(typeof(TranslateTransform),
            nameof(DX),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 0.0f });

        #endregion

        #region DY managed property

        /// <summary>
        /// Defines translation distance over X axis
        /// </summary>
        public float DY
        {
            get => (float)GetValue(DYProperty);
            set => SetValue(DYProperty, value);
        }

        public static readonly ManagedProperty DYProperty = ManagedProperty.Register(typeof(TranslateTransform),
            nameof(DY),
            typeof(float),
            new ManagedSimplePropertyMetadata { DefaultValue = 0.0f });

        #endregion
    }
}
