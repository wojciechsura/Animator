using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values
{
    public class CollectionValueViewModel : ValueViewModel
    {
        private readonly ObservableCollection<ManagedObjectViewModel> items = new();

        public IList<ManagedObjectViewModel> Items => items;
    }
}
