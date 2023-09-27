﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values
{
    public class StringValueViewModel : ValueViewModel
    {
        private string value;

        public StringValueViewModel(string initialValue)
        {
            value = initialValue;
        }

        public string Value
        {
            get => value;
            set => Set(ref this.value, value);
        }
    }
}