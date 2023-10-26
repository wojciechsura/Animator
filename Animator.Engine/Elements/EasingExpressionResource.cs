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
    public partial class EasingExpressionResource : Resource
    {
        public override object GetValue() => Value.Clone();

        #region Value managed property

        /// <summary>Value of this resource</summary>
        public EasingExpression Value
        {
            get => (EasingExpression)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly ManagedProperty ValueProperty = ManagedProperty.RegisterReference(typeof(EasingExpressionResource),
            nameof(Value),
            typeof(EasingExpression),
            new ManagedReferencePropertyMetadata());

        #endregion
    }
}
