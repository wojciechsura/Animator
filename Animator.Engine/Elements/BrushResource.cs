using Animator.Engine.Base;
using Animator.Engine.Base.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    [ContentProperty(nameof(Value))]
    public partial class BrushResource : Resource
    {
        public override object GetValue() => Value.Clone();

        #region Value managed property

        /// <summary>Value of this resource</summary>
        public Brush Value
        {
            get => (Brush)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly ManagedProperty ValueProperty = ManagedProperty.RegisterReference(typeof(BrushResource),
            nameof(Value),
            typeof(Brush),
            new ManagedReferencePropertyMetadata());

        #endregion
    }
}
