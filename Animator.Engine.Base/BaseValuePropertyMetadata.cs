using Animator.Engine.Base.Persistence.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base
{
    public class BaseValuePropertyMetadata : BasePropertyMetadata
    {
        public BaseValuePropertyMetadata(PropertyValueChangedDelegate valueChangedHandler, TypeSerializer customSerializer)
        {
            ValueChangedHandler = valueChangedHandler;
            CustomSerializer = customSerializer;
        }

        public PropertyValueChangedDelegate ValueChangedHandler { get; }
        public TypeSerializer CustomSerializer { get; }

    }
}
