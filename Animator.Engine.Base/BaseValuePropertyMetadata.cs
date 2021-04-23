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
        public BaseValuePropertyMetadata()
        {

        }

        public PropertyValueChangedDelegate ValueChangedHandler { get; init; } = null;
        public TypeSerializer CustomSerializer { get; init; } = null;

    }
}
