using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
using System;
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

        public override void RequestMoveUp(ObjectViewModel obj)
        {
            throw new NotSupportedException();
        }

        public override void RequestMoveDown(ObjectViewModel obj)
        {
            throw new NotSupportedException();
        }

        public string Value
        {
            get => value;
            set => Set(ref this.value, value);
        }

        public IList<string> AvailableOptions => Handler.AvailableOptions;
    }
}
