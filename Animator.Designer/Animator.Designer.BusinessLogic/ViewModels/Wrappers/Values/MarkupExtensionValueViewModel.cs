using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values
{
    public class MarkupExtensionValueViewModel : ValueViewModel
    {
        private readonly MarkupExtensionViewModel value;

        public MarkupExtensionValueViewModel(MarkupExtensionViewModel value)
        {
            this.value = value;
            value.Parent = this;
        }

        public string Name => value.Name;

        public ObjectViewModel Value => value;       
    }
}
