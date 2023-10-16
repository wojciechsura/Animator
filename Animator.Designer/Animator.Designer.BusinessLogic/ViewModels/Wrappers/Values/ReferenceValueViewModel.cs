using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values
{
    public class ReferenceValueViewModel : ValueViewModel
    {
        private ObjectViewModel value;

        private void DoGoTo()
        {
            value.GoTo();
        }

        public ReferenceValueViewModel(WrapperContext context, ObjectViewModel value)
            : base(context) 
        {
            this.value = value;
            value.Parent = this;

            GoToCommand = new AppCommand(obj => DoGoTo());
        }

        public override void RequestMoveUp(ObjectViewModel obj)
        {
            throw new NotSupportedException();
        }

        public override void RequestMoveDown(ObjectViewModel obj)
        {
            throw new NotSupportedException();
        }

        public ICommand GoToCommand { get; }

        public ObjectViewModel Value => value;
    }
}
