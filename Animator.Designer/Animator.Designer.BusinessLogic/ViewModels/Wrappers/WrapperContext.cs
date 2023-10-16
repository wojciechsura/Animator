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

        public void Merge(WrapperContext newContext)
        {
            foreach (var newNamespace in newContext.Namespaces) 
            { 
                // If the new namespace's URI doesn't exist in the current namespace list
                if (!namespaces.Exists(ns => ns.NamespaceUri == newNamespace.NamespaceUri))
                {
                    // Check if the prefix can be used
                    if (!namespaces.Exists(ns => ns.Prefix == newNamespace.Prefix))
                    {
                        namespaces.Add(newNamespace);
                    }
                    else
                    {
                        // Generate an unique prefix
                        int i = 1;
                        while (namespaces.Exists(ns => ns.Prefix == $"n{i}"))
                            i++;

                        namespaces.Add(newNamespace.CloneWithPrefix($"n{i}"));
                    }
                }
            }
        }

        public IReadOnlyList<NamespaceViewModel> Namespaces => namespaces;

        public string DefaultNamespace { get; }
        public string EngineNamespace { get; }

        public event EventHandler MovieChanged;
    }
}
