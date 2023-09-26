using Animator.Designer.BusinessLogic.Models.MovieSerialization;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects;
using Animator.Engine.Base.Exceptions;
using Animator.Engine.Base;
using Animator.Engine.Base.Persistence;
using Animator.Engine.Base.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using System.Collections;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values;

namespace Animator.Designer.BusinessLogic.Infrastructure
{
    public class MovieSerializer : BaseManagedObjectSerializer
    {
        // Private classes ----------------------------------------------------

        private class DeserializationContext
        {
            public DeserializationContext(Models.MovieSerialization.DeserializationOptions options)
            {
                Options = options;
            }

            public Dictionary<string, NamespaceDefinition> Namespaces { get; } = new();
            public Models.MovieSerialization.DeserializationOptions Options { get; }
        }

        // Private methods ----------------------------------------------------

        private void DeserializeChildren(XmlElement node, ManagedObjectViewModel deserializedObject, DeserializationContext context, HashSet<string> setProperties)
        {
            throw new NotImplementedException();
        }

        private void ProcessMarkupExtension(ManagedObjectViewModel deserializedObject, ManagedPropertyViewModel managedProperty, string value, XmlElement node, DeserializationContext context)
        {
            throw new NotImplementedException();
        }
    
        private void DeserializeAttributes(XmlElement node, 
            ManagedObjectViewModel deserializedObject, 
            DeserializationContext context, 
            HashSet<string> propertiesSet)
        {
            foreach (XmlAttribute attribute in node.Attributes)
            {
                // 1. Omit xmlns definitions, for some reason they are
                // being treated as regular attributes too

                if (attribute.Name == "xmlns" || attribute.Name.StartsWith("xmlns:"))
                    continue;

                // 2. Omit internal attributes

                if (attribute.NamespaceURI == ENGINE_NAMESPACE)
                {
                    if (attribute.LocalName == KEY_ATTRIBUTE)
                    {
                        // This attribute may be safely ignored.
                        continue;
                    }
                    else
                        throw new SerializerException($"Not recognized internal attribute: {attribute.LocalName}", node.FindXPath());
                }

                // 3. Check if property hasn't been already set

                if (propertiesSet.Contains(attribute.LocalName))
                    throw new SerializerException($"Property {attribute.LocalName} has been already set on type {deserializedObject.GetType().Name}",
                        node.FindXPath());

                // 4. Find property with appropriate name

                ManagedPropertyViewModel managedProperty = deserializedObject.Property<ManagedPropertyViewModel>(attribute.LocalName);

                if (managedProperty == null)
                    throw new SerializerException($"Property {attribute.LocalName} not found on object of type {deserializedObject.GetType().Name}",
                        node.FindXPath());
                if (managedProperty.ManagedProperty.Metadata.NotSerializable)
                    throw new SerializerException($"Property {attribute.LocalName} on object {deserializedObject.GetType().Name} is not serializable!\r\nRemove it from input file.",
                        node.FindXPath());

                // 5. Check for markup extension

                if (attribute.Value.StartsWith(MARKUP_EXTENSION_BEGIN) &&
                    !attribute.Value.StartsWith(MARKUP_EXTENSION_ESCAPED_BEGIN) &&
                    attribute.Value.EndsWith(MARKUP_EXTENSION_END))
                {
                    ProcessMarkupExtension(deserializedObject, managedProperty, attribute.Value, node, context);
                }
                else
                {
                    if (managedProperty is ManagedSimplePropertyViewModel simpleProperty)
                    {
                        simpleProperty.Value = new StringValueViewModel(attribute.Value);
                        propertiesSet.Add(attribute.LocalName);
                    }
                    else if (managedProperty is ManagedCollectionPropertyViewModel collectionProperty)
                    {
                        // This means that collection value is stored as attribute.
                        // In case of Designer this doesn't matter much - we simply
                        // set the StringValue to the property
                        
                        collectionProperty.Value = new StringValueViewModel(attribute.Value);
                        propertiesSet.Add(attribute.LocalName);
                    }
                    else if (managedProperty is ManagedReferencePropertyViewModel referenceProperty)
                    {
                        // This means that reference value is stored as attribute.
                        // In case of Designer this doesn't matter much - we simply
                        // set the StringValue to the property

                        referenceProperty.Value = new StringValueViewModel(attribute.Value);
                        propertiesSet.Add(attribute.LocalName);
                    }
                    else
                        throw new InvalidOperationException("Unsupported property type!");
                }
            }
        }

        private BaseObjectViewModel DeserializeElement(XmlElement node, DeserializationContext context)
        {
            // 1. Check control nodes

            if (string.Equals(node.NamespaceURI, ENGINE_NAMESPACE))
            {
                if (node.LocalName == INCLUDE_ELEMENT)
                {
                    var sourceAttribute = node.Attributes[SOURCE_ATTRIBUTE];
                    if (sourceAttribute == null)
                        throw new SerializerException("Include element must contain attribute Source!", node.FindXPath());

                    string filename = node.Attributes[SOURCE_ATTRIBUTE].Value;

                    var includeViewModel = new IncludeViewModel();
                    includeViewModel.Property<StringPropertyViewModel>(SOURCE_ATTRIBUTE).Value = filename;

                    return includeViewModel;
                }
                else if (node.LocalName == MACRO_ELEMENT)
                {
                    var keyAttribute = node.Attributes
                        .OfType<XmlAttribute>()
                        .FirstOrDefault(a => a.NamespaceURI == ENGINE_NAMESPACE && a.LocalName == KEY_ATTRIBUTE);

                    if (keyAttribute == null)
                        throw new SerializerException("Macro element misses Key attribute!", node.FindXPath());

                    var key = keyAttribute.Value;

                    if (node.ChildNodes.Count > 0)
                        throw new SerializerException("Macro may not contain any child elements!", node.FindXPath());

                    var macroViewModel = new MacroViewModel();
                    macroViewModel.Property<StringPropertyViewModel>(KEY_ATTRIBUTE).Value = key;

                    // Using new HashSet here on purpose.
                    // This allows overriding attributes, which are set in macro.
                    // eg.
                    // <x:Macro x:Key="SomeMacro" Name="Test" Position="20,30" />
                    DeserializeAttributes(node, macroViewModel, context, new HashSet<string>());

                    return macroViewModel;
                }
                else if (node.LocalName == GENERATE_ELEMENT)
                {
                    if (node.ChildNodes.Count != 1)
                        throw new SerializerException("Generate element must contain exactly one child!", node.FindXPath());

                    var child = (XmlElement)node.ChildNodes[0];

                    var generateViewModel = new GenerateViewModel();
                    generateViewModel.Property<MultilineStringPropertyViewModel>("Generator").Value = child.OuterXml;

                    return generateViewModel;
                }
                else
                {
                    throw new SerializerException($"Not recognized internal node: {node.LocalName}", node.FindXPath());
                }
            }
            else
            {
                // 1. Deserialize class

                ManagedObjectViewModel deserializedObject;

                var objectTypeData = ExtractObjectType(node.NamespaceURI, node.LocalName, typeof(ManagedObject), context.Namespaces);
                deserializedObject = new ManagedObjectViewModel(node.Name, objectTypeData.FullClassName, objectTypeData.Type);

                // 2. Load attributes

                var setProperties = new HashSet<string>();

                DeserializeAttributes(node, deserializedObject, context, setProperties);

                // 3. Load children

                DeserializeChildren(node, deserializedObject, context, setProperties);

                // 4. Return deserialized object

                return deserializedObject;
            }
        }

        private BaseObjectViewModel InternalDeserialize(XmlDocument document, string documentPath, Models.MovieSerialization.DeserializationOptions options)
        {
            var context = new DeserializationContext(options);

            if (options != null)
            {
                if (options.DefaultNamespace != null)
                {
                    context.Namespaces[string.Empty] = options.DefaultNamespace;
                }
            }

            return DeserializeElement(document.ChildNodes.OfType<XmlElement>().Single(), context);
        }

        // Public methods -----------------------------------------------------

        public BaseObjectViewModel Deserialize(string filename, Models.MovieSerialization.DeserializationOptions options = null)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(filename);

            string documentPath = Path.GetDirectoryName(filename);
            if (string.IsNullOrEmpty(documentPath))
                documentPath = Directory.GetCurrentDirectory();

            return InternalDeserialize(document, documentPath, options);
        }

        public BaseObjectViewModel Deserialize(XmlDocument document, string documentPath, Models.MovieSerialization.DeserializationOptions options = null)
        {
            return InternalDeserialize(document, documentPath, options);
        }
    }
}
