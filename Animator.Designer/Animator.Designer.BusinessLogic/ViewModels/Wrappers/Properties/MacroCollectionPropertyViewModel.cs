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
    public class MacroCollectionPropertyViewModel : PropertyViewModel
    {
        // Private fields -----------------------------------------------------

        private readonly CollectionValueViewModel value;

        // Private methods ----------------------------------------------------

        private void HandleCollectionChanged(object sender, EventArgs args)
        {
            OnCollectionChanged();
            context.NotifyPropertyChanged();
        }

        private void DoAddMacroDefinition()
        {
            value.Items.Add(new MacroDefinitionViewModel(context));
        }

        // Protected methods --------------------------------------------------        

        protected void OnCollectionChanged() =>
            CollectionChanged?.Invoke(this, EventArgs.Empty);

        // Public methods -----------------------------------------------------

        public MacroCollectionPropertyViewModel(ObjectViewModel parent,
            WrapperContext context,
            string ns,
            string name)
            : base(parent, context)
        {
            value = new CollectionValueViewModel();
            value.Parent = this;
            value.CollectionChanged += HandleCollectionChanged;

            Namespace = ns;
            Name = name;            

            AddMacroDefinitionCommand = new AppCommand(obj => DoAddMacroDefinition());
        }

        public override void RequestDelete(ObjectViewModel obj)
        {
            if (!value.Items.Contains(obj))
                throw new InvalidOperationException("Collection does not contain this object!");

            value.Items.Remove(obj);
        }

        public override void RequestSwitchToString()
        {
            throw new NotSupportedException();
        }

        // Public properties --------------------------------------------------

        public override string Name { get; }

        public override string Namespace { get; }

        public CollectionValueViewModel Value
        {
            get => value;            
        }

        public event EventHandler CollectionChanged;
    }
}
