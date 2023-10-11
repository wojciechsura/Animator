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

        public override void RequestMoveUp(ObjectViewModel obj)
        {
            var index = items.IndexOf(obj);
            if (index == -1)
                throw new InvalidOperationException("Cannot move up, item does not exist in the collection!");
            if (index == 0)
                throw new InvalidOperationException("Cannot move up, item is first!");

            items.Move(index, index - 1);
        }

        public override void RequestMoveDown(ObjectViewModel obj)
        {
            var index = items.IndexOf(obj);
            if (index == -1)
                throw new InvalidOperationException("Cannot move up, item does not exist in the collection!");
            if (index == items.Count - 1)
                throw new InvalidOperationException("Cannot move up, item is last!");

            items.Move(index, index + 1);
        }

        public IList<ObjectViewModel> Items => items;

        public event EventHandler CollectionChanged;
    }
}
