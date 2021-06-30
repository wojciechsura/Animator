using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public class PointResource : Resource
    {
        public override object GetValue() => Value;

        #region Value managed property

        /// <summary>Value of this resource</summary>
        public PointF Value
        {
            get => (PointF)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly ManagedProperty ValueProperty = ManagedProperty.Register(typeof(PointResource),
            nameof(Value),
            typeof(PointF),
            new ManagedSimplePropertyMetadata { DefaultValue = PointF.Empty });

        #endregion
    }
}
