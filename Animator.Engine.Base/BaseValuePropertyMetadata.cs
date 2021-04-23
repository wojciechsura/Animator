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

        /// <summary>Handler for value change events</summary>
        public PropertyValueChangedDelegate ValueChangedHandler { get; init; } = null;

        /// <summary>Custom serializer allows serializing value 
        /// into an XML attribute</summary>
        public TypeSerializer CustomSerializer { get; init; } = null;

    }
}
