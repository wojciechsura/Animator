﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties
{
    public class MultilineStringPropertyViewModel : PropertyViewModel
    {
        private string value;

        public MultilineStringPropertyViewModel(string name)
        {
            Name = name;
        }

        public override string Name { get; }

        public string Value
        {
            get => value;
            set => Set(ref this.value, value);
        }
    }
}
