using Animator.Designer.BusinessLogic.ViewModels.Base;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Animator.Designer.BusinessLogic.ViewModels.Main
{
    public class DocumentViewModel : BaseViewModel
    {
        private readonly BaseObjectViewModel[] displayItems;
        private BaseObjectViewModel selectedElement;

        public DocumentViewModel(BaseObjectViewModel rootNode, WrapperContext wrapperContext, string filename = "Animation.xml", bool filenameVirtual = true)
        {
            RootNode = rootNode;
            WrapperContext = wrapperContext;
            displayItems = new[] { rootNode };

            Filename = filename;
            FilenameVirtual = filenameVirtual;
        }

        public string Filename { get; set; }
        public bool FilenameVirtual { get; set; }
        public BaseObjectViewModel RootNode { get; }
        public WrapperContext WrapperContext { get; }

        public IEnumerable<BaseObjectViewModel> DisplayItems => displayItems;
        
        public BaseObjectViewModel SelectedElement 
        {
            get => selectedElement;
            set
            {
                Set(ref selectedElement, value);
            }
        }
    }
}
