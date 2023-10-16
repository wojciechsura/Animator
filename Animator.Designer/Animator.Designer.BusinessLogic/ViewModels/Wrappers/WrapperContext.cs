using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers
{
    public class GoToRequestEventArgs : EventArgs
    {
        public GoToRequestEventArgs(BaseObjectViewModel @object)
        {
            Object = @object;
        }

        public BaseObjectViewModel Object { get; }
    }

    public delegate void GoToRequestEventHandler(object sender, GoToRequestEventArgs e);

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
        {
            if (!namespaces.Any(n => n.NamespaceUri == ns.NamespaceUri))
                namespaces.Add(ns);
        }

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

        internal void RequestGoTo(ObjectViewModel objectViewModel)
        {
            GoToRequest?.Invoke(this, new GoToRequestEventArgs(objectViewModel));
        }

        public IReadOnlyList<NamespaceViewModel> Namespaces => namespaces;

        public string DefaultNamespace { get; }
        public string EngineNamespace { get; }

        public event EventHandler MovieChanged;

        public event GoToRequestEventHandler GoToRequest;
    }
}
