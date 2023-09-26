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
        public DocumentViewModel(XmlDocument document, ManagedObjectViewModel rootNode, string filename = "Animation.xml", bool filenameVirtual = true)
        {
            Document = document;
            RootNode = rootNode;
            Filename = filename;
            FilenameVirtual = filenameVirtual;
        }

        public string Filename { get; set; }
        public bool FilenameVirtual { get; set; }
        public XmlDocument Document { get; }
        public ManagedObjectViewModel RootNode { get; }
    }
}
