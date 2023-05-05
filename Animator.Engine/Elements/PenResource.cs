using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public class PenResource : Resource
    {
        public override object GetValue() => Value.Clone();

        #region Value managed property

        /// <summary>Value of this resource</summary>
        public Pen Value
        {
            get => (Pen)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly ManagedProperty ValueProperty = ManagedProperty.RegisterReference(typeof(PenResource),
            nameof(Value),
            typeof(Pen),
            new ManagedReferencePropertyMetadata());

        #endregion
    }
}
