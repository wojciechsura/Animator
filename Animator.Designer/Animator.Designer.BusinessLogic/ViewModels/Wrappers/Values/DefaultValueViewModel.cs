using Animator.Engine.Base.Persistence.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values
{
    public class DefaultValueViewModel : ValueViewModel
    {       
        public DefaultValueViewModel(object defaultValue) 
        {
            if (defaultValue == null)
                Value = null;
            else if (TypeSerialization.CanSerialize(defaultValue, defaultValue.GetType()))
                Value = TypeSerialization.Serialize(defaultValue);
            else
                Value = defaultValue.ToString();            
        }

        public string Value { get; }
    }
}
