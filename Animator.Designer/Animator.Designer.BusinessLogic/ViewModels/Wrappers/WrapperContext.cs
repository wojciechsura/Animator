using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers
{
    public class WrapperContext
    {
        private readonly List<NamespaceViewModel> namespaces = new();

        public WrapperContext(string engineNamespace, string defaultNamespace)
        {
            namespaces = new();
            EngineNamespace = engineNamespace;
            DefaultNamespace = defaultNamespace;
        }

        public void AddNamespace(NamespaceViewModel ns)
            => namespaces.Add(ns);

        public void ApplyNamespaces(XmlDocument document, XmlElement rootNode)
        {
            foreach (var @namespace in Namespaces.Where(ns => !string.IsNullOrEmpty(ns.Prefix)))
            {
                var attr = document.CreateAttribute($"xmlns:{@namespace.Prefix}");
                attr.Value = @namespace.NamespaceUri;
                rootNode.Attributes.Append(attr);
            }
        }

        public void NotifyPropertyChanged()
        {
            MovieChanged?.Invoke(this, EventArgs.Empty);
        }

        public IReadOnlyList<NamespaceViewModel> Namespaces => namespaces;

        public string DefaultNamespace { get; }
        public string EngineNamespace { get; }

        public event EventHandler MovieChanged;
    }
}
