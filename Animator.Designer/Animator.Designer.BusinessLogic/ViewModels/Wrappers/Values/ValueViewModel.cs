﻿using Animator.Designer.BusinessLogic.Infrastructure;
using Animator.Designer.BusinessLogic.ViewModels.Base;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values
{
    public abstract class ValueViewModel : BaseViewModel, IParentedItem<PropertyViewModel>
    {       
        public IValueHandler Handler { get; set; }

        public PropertyViewModel Parent { get; set; }

        public void RequestDelete(ObjectViewModel obj)
        {
            Parent.RequestDelete(obj);
        }

        public abstract void RequestMoveDown(ObjectViewModel obj);

        public abstract void RequestMoveUp(ObjectViewModel obj);
    }
}
