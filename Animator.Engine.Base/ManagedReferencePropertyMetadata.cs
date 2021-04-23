using Animator.Engine.Base.Persistence.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    public class ValueValidationEventArgs : EventArgs
    {
        public ValueValidationEventArgs(object newValue)
        {
            NewValue = newValue;
        }

        public object NewValue { get; }
    }

    public delegate bool ValueValidationDelegate(ManagedObject sender, ValueValidationEventArgs args);

    public class ManagedReferencePropertyMetadata : BaseValuePropertyMetadata
    {
        private static readonly ManagedReferencePropertyMetadata defaultMetadata = new ManagedReferencePropertyMetadata();

        public static ManagedReferencePropertyMetadata DefaultFor(Type propertyType) => defaultMetadata;

        public ManagedReferencePropertyMetadata()
        {

        }

        /// <summary>If set, the non-null value can be set only once.</summary>
        public bool ValueIsPermanent { get; init; } = false;

        /// <summary>Allows verifying, if new value is valid</summary>
        public ValueValidationDelegate ValueValidationHandler { get; init; }
    }
}
