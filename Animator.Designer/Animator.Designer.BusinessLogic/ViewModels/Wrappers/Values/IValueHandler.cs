﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values
{
    public interface IValueHandler
    {
        IList<string> AvailableOptions { get; }

        void RequestSwitchToString();
    }
}
