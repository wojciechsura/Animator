using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Animator.Designer.BusinessLogic.ViewModels.Main
{
    public class DocumentViewModel
    {
        private readonly BaseObjectViewModel[] displayItems;

        public DocumentViewModel(BaseObjectViewModel rootNode, string filename = "Animation.xml", bool filenameVirtual = true)
        {
            RootNode = rootNode;
            displayItems = new[] { rootNode };

            Filename = filename;
            FilenameVirtual = filenameVirtual;
        }

        public string Filename { get; set; }
        public bool FilenameVirtual { get; set; }
        public BaseObjectViewModel RootNode { get; }

        public IEnumerable<BaseObjectViewModel> DisplayItems => displayItems;
    }
}
