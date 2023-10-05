using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Types;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values;
using Animator.Engine.Base.Extensions;
using Spooksoft.VisualStateManager.Commands;
using Spooksoft.VisualStateManager.Conditions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties
{
    public class ClearableStringPropertyViewModel : PropertyViewModel
    {
        // Private fields -----------------------------------------------------

        private ValueViewModel value;

        // Private methods ----------------------------------------------------

        private void HandleStringChanged(object sender, PropertyChangedEventArgs e)
        {
            context.NotifyPropertyChanged();
        }

        private void SetDefault()
        {
            Value = new DefaultValueViewModel(null, false);
        }

        private void SetToString()
        {
            Value = new StringValueViewModel(null);
        }

        private void SetValue(ValueViewModel value)
        {
            // Clear parent
            if (this.value is StringValueViewModel)
            {
                this.value.PropertyChanged -= HandleStringChanged;
            }
            if (this.value != null)
            {
                this.value.Parent = null;
                this.value.Handler = null;
            }

            // Set new value
            if (value is StringValueViewModel)
            {
                Set(ref this.value, value, nameof(Value));
                value.Parent = this;
                value.Handler = this;
                value.PropertyChanged += HandleStringChanged;
            }
            else if (value is DefaultValueViewModel)
            {
                Set(ref this.value, value, nameof(Value));
                value.Parent = this;
                value.Handler = this;
            }
            else
            {
                throw new ArgumentException($"ClearableStringPropertyViewModel does not support value of type {value}!");
            }

            context.NotifyPropertyChanged();
        }

        // Public methods -----------------------------------------------------

        public ClearableStringPropertyViewModel(ObjectViewModel parent,
            WrapperContext context,
            string ns,
            string name)
            : base(parent, context)
        {
            Namespace = ns;
            Name = name;

            SetDefault();

            var valueIsDefaultCondition = Condition.Lambda(this, vm => vm.Value is DefaultValueViewModel, false);
            var valueIsStringCondition = Condition.Lambda(this, vm => vm.Value is StringValueViewModel, false);

            SetDefaultCommand = new AppCommand(obj => SetDefault(), !valueIsDefaultCondition);
            SetToStringCommand = new AppCommand(obj => SetToString(), !valueIsStringCondition);
        }

        public override void RequestDelete(ObjectViewModel obj)
        {
            throw new NotSupportedException();
        }

        public override void RequestSwitchToString()
        {
            throw new NotSupportedException();
        }

        // Public properties --------------------------------------------------

        public override string Name { get; }

        public override string Namespace { get; }

        public ValueViewModel Value
        {
            get => value;
            set => SetValue(value);
        }
    }
}
