using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers
{
    public class TypeViewModel
    {
        private static Dictionary<Type, TypeViewModel> typeCache = new();

        private readonly Type type;

        private TypeViewModel(Type type)
        {
            this.type = type;
        }

        public static TypeViewModel For(Type type)
        {
            if (typeCache.TryGetValue(type, out TypeViewModel viewModel))
                return viewModel;

            viewModel = new TypeViewModel(type);
            typeCache.Add(type, viewModel);
            return viewModel;
        }

        public string Name => type.Name;

        public Type Type => type;
    }
}
