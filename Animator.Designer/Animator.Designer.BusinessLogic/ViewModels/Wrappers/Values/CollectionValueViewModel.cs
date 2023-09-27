using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
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
        private readonly ObservableCollection<BaseObjectViewModel> items = new();

        private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.CollectionChanged?.Invoke(this, EventArgs.Empty);
        }

        public CollectionValueViewModel()
        {
            items.CollectionChanged += HandleCollectionChanged;
        }

        public IList<BaseObjectViewModel> Items => items;

        public event EventHandler CollectionChanged;
    }
}
