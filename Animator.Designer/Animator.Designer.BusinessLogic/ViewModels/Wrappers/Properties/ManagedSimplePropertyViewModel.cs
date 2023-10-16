using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Types;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values;
using Animator.Engine.Base;
using Animator.Engine.Base.Extensions;
using Animator.Engine.Base.Persistence.Types;
using Spooksoft.VisualStateManager.Commands;
using Spooksoft.VisualStateManager.Conditions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties
{
    public class ManagedSimplePropertyViewModel : ManagedPropertyViewModel
    {
        private readonly ManagedSimpleProperty simpleProperty;

        private void HandleStringValueChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StringValueViewModel.Value))
            {
                OnStringValueChanged();
                context.NotifyPropertyChanged();
            }
        }

        private IEnumerable<ResourceKeyViewModel> GetAvailableResources()
        {
            Func<ManagedObjectViewModel, bool> matchesCurrentType;

            if (simpleProperty.Type == typeof(int))
                matchesCurrentType = managedObj => managedObj.Type == typeof(Animator.Engine.Elements.IntResource);
            else if (simpleProperty.Type == typeof(float))
                matchesCurrentType = managedObj => managedObj.Type == typeof(Animator.Engine.Elements.FloatResource);
            else if (simpleProperty.Type == typeof(PointF))
                matchesCurrentType = managedObj => managedObj.Type == typeof(Animator.Engine.Elements.PointResource);
            else if (simpleProperty.Type == typeof(TimeSpan))
                matchesCurrentType = managedObj => managedObj.Type == typeof(Animator.Engine.Elements.TimeResource);
            else if (simpleProperty.Type == typeof(string))
                matchesCurrentType = managedObj => managedObj.Type == typeof(Animator.Engine.Elements.StringResource);
            else
                matchesCurrentType = managedObj => false;
            
            return InternalGetAvailableResources(matchesCurrentType);
        }

        private void SetDefault()
        {
            string defaultValue = TypeSerialization.Serialize(simpleProperty.Metadata.DefaultValue);
            Value = new DefaultValueViewModel(context, defaultValue, true);
        }

        private void SetToString()
        {
            string newValue = string.Empty;

            if (value is DefaultValueViewModel defaultValue)
            {
                newValue = defaultValue.Value;
            }

            Value = new StringValueViewModel(context, newValue);
        }

        protected override void OnSetValue(ValueViewModel value)
        {
            // Unhook existing event handlers

            if (Value is StringValueViewModel currentStringValue)
            {
                currentStringValue.PropertyChanged -= HandleStringValueChanged;
            }

            // Hook new event handlers and set value

            if (value is StringValueViewModel)
            {
                value.PropertyChanged += HandleStringValueChanged;
                Set(ref this.value, value, nameof(Value));
            }
            else if (value is DefaultValueViewModel or MarkupExtensionValueViewModel)
                Set(ref this.value, value, nameof(Value));
            else
                throw new ArgumentException($"ManagedSimplePropertyViewModel does not support value of type {value}!");

            context.NotifyPropertyChanged();
        }

        public ManagedSimplePropertyViewModel(ObjectViewModel parent, WrapperContext context, ManagedSimpleProperty property)
            : base(parent, context, property)
        {
            this.simpleProperty = property;
            SetDefault();

            var valueIsStringCondition = Condition.Lambda(this, vm => vm.Value is StringValueViewModel, false);
            var valueIsDefaultCondition = Condition.Lambda(this, vm => vm.Value is DefaultValueViewModel, false);
            
            SetDefaultCommand = new AppCommand(obj => SetDefault(), !valueIsDefaultCondition);
            SetToStringCommand = new AppCommand(obj => SetToString(), !valueIsStringCondition);
            SetToFromResourceCommand = new AppCommand(obj => SetToFromResource((string)obj));

            if (property.Type == typeof(bool))
            {
                AvailableOptions = new List<string> { "True", "False" };
            }
            else if (property.Type.IsEnum)
            {
                var availableOptions = new List<string>();

                foreach (var enumVal in Enum.GetValues(property.Type))
                {
                    availableOptions.Add(enumVal.ToString());
                }

                AvailableOptions = availableOptions;
            }
            else
            {
                AvailableOptions = null;
            }
        }

        public override void RequestDelete(ObjectViewModel obj)
        {
            throw new NotSupportedException();
        }

        public override void RequestSwitchToString()
        {
            if (value is not StringValueViewModel)
            {
                SetToString();
            }
        }

        public override IEnumerable<ResourceKeyViewModel> AvailableResources => GetAvailableResources();

        public override ManagedProperty ManagedProperty => simpleProperty;        
    }
}
