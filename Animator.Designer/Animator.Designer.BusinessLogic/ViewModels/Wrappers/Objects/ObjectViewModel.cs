using Animator.Designer.BusinessLogic.Infrastructure;
using Animator.Designer.BusinessLogic.ViewModels.Base;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values;
using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects
{
    public abstract class ObjectViewModel : BaseObjectViewModel, IParentedItem<ValueViewModel>
    {
        private ValueViewModel parent;

        protected ObjectViewModel(WrapperContext context) 
            : base(context)
        {

        }

        protected XmlElement CreateNestedProperty(XmlDocument document, NamespaceViewModel objectNsDef, ManagedPropertyViewModel managedProperty)
        {
            var attrNs = context.Namespaces.First(ns => ns.NamespaceUri == managedProperty.Namespace);

            if (!string.IsNullOrEmpty(attrNs.Prefix))
                throw new InvalidOperationException("Property with non-default namespace cannot be a reference property!");

            XmlElement propElement;

            if (string.IsNullOrEmpty(objectNsDef.Prefix))
                propElement = document.CreateElement($"{Name}.{managedProperty.Name}");
            else
                propElement = document.CreateElement(objectNsDef.Prefix, $"{Name}.{managedProperty.Name}", objectNsDef.NamespaceUri);
            return propElement;
        }

        protected XmlAttribute CreateAttributeProp(XmlDocument document, ManagedPropertyViewModel managedProperty) =>
            CreateAttributeProp(document, managedProperty.Name, managedProperty.Namespace);

        protected XmlAttribute CreateAttributeProp(XmlDocument document, string name, string namespaceUri)
        {
            var attrNs = context.Namespaces.First(ns => ns.NamespaceUri == namespaceUri);

            XmlAttribute propAttr;

            if (string.IsNullOrEmpty(attrNs.Prefix))
                propAttr = document.CreateAttribute(name);
            else
                propAttr = document.CreateAttribute(attrNs.Prefix, name, attrNs.NamespaceUri);
            return propAttr;
        }

        protected XmlElement CreateRootElement(XmlDocument document)
        {
            XmlElement result;

            var objectNsDef1 = context.Namespaces.First(ns => ns.NamespaceUri == Namespace);

            if (string.IsNullOrEmpty(objectNsDef1.Prefix))
                result = document.CreateElement(Name);
            else
                result = document.CreateElement(objectNsDef1.Prefix, Name, objectNsDef1.NamespaceUri);
            return result;
        }

        public abstract XmlElement Serialize(XmlDocument document);

        public ValueViewModel Parent 
        { 
            get => parent;
            set => Set(ref parent, value);
        }

        public string Name { get; init; }

        public string Namespace { get; init; }
    }
}
