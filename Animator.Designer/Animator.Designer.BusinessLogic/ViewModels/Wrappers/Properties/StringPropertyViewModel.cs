using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties
{
    public class StringPropertyViewModel : PropertyViewModel
    {
        private string value;

        public StringPropertyViewModel(string name)
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
