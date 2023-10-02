using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
using Animator.Designer.BusinessLogic.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values
{
    public class CollectionValueViewModel : ValueViewModel
    {
        private readonly ParentedObservableCollection<ObjectViewModel, ValueViewModel> items;

        private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => 
            this.CollectionChanged?.Invoke(this, EventArgs.Empty);

        public CollectionValueViewModel()
        {
            items = new(this);
            items.CollectionChanged += HandleCollectionChanged;
        }

        public IList<ObjectViewModel> Items => items;

        public event EventHandler CollectionChanged;
    }
}
