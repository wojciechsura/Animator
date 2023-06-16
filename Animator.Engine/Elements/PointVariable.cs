using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public class PointVariable : Variable
    {
        #region Value dependency property

        public PointF Value
        {
            get => (PointF)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly ManagedProperty ValueProperty = ManagedProperty.Register(typeof(PointVariable),
            nameof(Value),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = PointF.Empty, NotAnimatable = true });

        #endregion
    }
}
