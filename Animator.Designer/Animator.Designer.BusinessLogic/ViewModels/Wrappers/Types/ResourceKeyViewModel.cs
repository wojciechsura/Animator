﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Types
{
    public record ResourceKeyViewModel(string Key, ICommand Command, bool Enabled = true);
}
