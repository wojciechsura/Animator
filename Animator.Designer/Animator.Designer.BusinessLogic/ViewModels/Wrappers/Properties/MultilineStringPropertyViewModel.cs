﻿using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties
{
    public class MultilineStringPropertyViewModel : PropertyViewModel
    {
        private string value;

        private void HandleStringChanged()
        {
            context.NotifyPropertyChanged();
        }

        public MultilineStringPropertyViewModel(ObjectViewModel parent, WrapperContext context, string ns, string name)
            : base(parent, context)
        {
            Name = name;
            Namespace = ns;
        }

        public override void RequestDelete(ObjectViewModel obj)
        {
            throw new NotSupportedException();
        }

        public override void RequestSwitchToString()
        {
            throw new NotSupportedException();
        }

        public override string Name { get; }

        public override string Namespace { get; }

        public string Value
        {
            get => value;
            set => Set(ref this.value, value, changeHandler: HandleStringChanged);
        }
    }
}
