using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Types;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values;
using Animator.Engine.Base;
using Animator.Engine.Base.Extensions;
using Newtonsoft.Json.Linq;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties
{
    public abstract class ManagedPropertyViewModel : PropertyViewModel
    {
        private void SetValue(ValueViewModel value)
        {
            if (this.value != null) 
            {                
                value.Handler = null;
                value.Parent = null;
            }

            OnSetValue(value);

            if (this.value != null)
            {
                value.Parent = this;
                value.Handler = this;
            }
        }

        private void SetToMarkupExtension(Type type)
        {
            var namespaceDefinition = type.ToNamespaceDefinition();
            var markupExtensionViewModel = new MarkupExtensionViewModel(context, namespaceDefinition.ToString(), type.Name, type);
            var markupExtensionValue = new MarkupExtensionValueViewModel(markupExtensionViewModel);
            Value = markupExtensionValue;
        }

        protected ValueViewModel value;

        protected void OnStringValueChanged() => StringValueChanged?.Invoke(this, EventArgs.Empty);
        protected void OnCollectionChanged() => CollectionChanged?.Invoke(this, EventArgs.Empty);

        protected abstract void OnSetValue(ValueViewModel value);

        public ManagedPropertyViewModel(ObjectViewModel parent, WrapperContext context, ManagedProperty property)
            : base(parent, context)
        {
            Name = property.Name;
            Namespace = context.DefaultNamespace;

            SetToMarkupExtensionCommand = new AppCommand(obj => SetToMarkupExtension((Type)obj));
        }

        public override IEnumerable<TypeViewModel> AvailableMarkupExtensions
        {
            get
            {
                return context.Namespaces
                    .OfType<AssemblyNamespaceViewModel>()
                    .SelectMany(ns => ns.GetAvailableTypesFor(typeof(Animator.Engine.Base.BaseMarkupExtension)))
                    .OrderBy(mx => mx.Name)
                    .Select(type => new TypeViewModel(type, SetToMarkupExtensionCommand));
            }
        }

        public abstract ManagedProperty ManagedProperty { get; }

        public override string Name { get; }

        public override string Namespace { get; }

        public ValueViewModel Value
        {
            get => value;
            set => SetValue(value);
        }

        public event EventHandler StringValueChanged;
        public event EventHandler CollectionChanged;
    }
}
