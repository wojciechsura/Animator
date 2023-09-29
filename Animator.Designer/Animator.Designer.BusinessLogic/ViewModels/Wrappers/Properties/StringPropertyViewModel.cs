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

        public StringPropertyViewModel(WrapperContext context, string ns, string name)
            : base(context)
        {
            Name = name;
            Namespace = ns;
        }

        public override string Name { get; }

        public override string Namespace { get; }

        public string Value
        {
            get => value;
            set => Set(ref this.value, value);
        }
    }
}
