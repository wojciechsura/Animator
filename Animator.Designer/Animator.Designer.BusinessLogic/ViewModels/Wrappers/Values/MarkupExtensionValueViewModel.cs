using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values
{
    public class MarkupExtensionValueViewModel : ValueViewModel
    {
        private readonly MarkupExtensionViewModel value;

        public MarkupExtensionValueViewModel(WrapperContext context, MarkupExtensionViewModel value) 
            : base(context)
        {
            this.value = value;
            value.Parent = this;

            // Go to is supported only for FromReference markup extension
            if (value.Type == typeof(Animator.Engine.Elements.FromResource))
                GoToCommand = new AppCommand(obj => DoGoTo());
        }

        private void DoGoTo()
        {
            var key = value.Property<ClearableStringPropertyViewModel>(context.DefaultNamespace, nameof(Animator.Engine.Elements.FromResource.Key));
            if (key.Value is StringValueViewModel strValue)
                Parent.RequestGoToResource(strValue.Value);
        }

        public override void RequestMoveUp(ObjectViewModel obj)
        {
            throw new NotSupportedException();
        }

        public override void RequestMoveDown(ObjectViewModel obj)
        {
            throw new NotSupportedException();
        }

        public string Name => value.Name;

        public ICommand GoToCommand { get; }

        public MarkupExtensionViewModel Value => value;       
    }
}
