using Animator.Engine.Base;
using Animator.Engine.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Animator.Engine.Persistence
{
    public class ManagedObjectSerializer
    {
        // Private types ------------------------------------------------------

        private class DeserializationContext
        {
            public Dictionary<string, NamespaceDefinition> Namespaces { get; } = new();
        }

        // Private constants --------------------------------------------------

        private const string contentDecoration = "content:{0}";

        // Private methods ----------------------------------------------------

        private NamespaceDefinition ParseNamespaceDefinition(string namespaceDefinition)
        {
            // Required namespace definition format:
            //
            // assembly=A.B.C;namespace=X.Y.Z

            string[] elements = namespaceDefinition.Split(';');

            if (elements.Length != 2)
                throw new ArgumentException(nameof(namespaceDefinition), $"Invalid namespace definition: {namespaceDefinition}\r\nThere should be exactly two sections separated with semicolon (;).");

            // Assembly

            if (!elements[0].StartsWith("assembly=") || elements[0].Length < 10)
                throw new ArgumentException(nameof(namespaceDefinition), $"Invalid namespace definition: {namespaceDefinition}\r\nMissing or invalid assembly section!");

            string assembly = elements[0].Substring(9);

            // Namespace

            if (!elements[1].StartsWith("namespace=") || elements[1].Length < 11)
                throw new ArgumentException(nameof(namespaceDefinition), $"Invalid namespace definition: {namespaceDefinition}\r\nMissing or invalid namespace section!");

            string @namespace = elements[1].Substring(10);

            return new NamespaceDefinition(assembly, @namespace);
        }

        private void DeserializeChildren(XmlNode node, ManagedObject deserializedObject, DeserializationContext context, HashSet<string> propertiesSet)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                // 1. Check if it is a property with extended notation

                if (child.NamespaceURI == node.NamespaceURI &&
                    child.LocalName.StartsWith($"{node.LocalName}."))
                {
                    // 1.1.1 Loading property

                    string propertyName = child.LocalName.Substring(node.LocalName.Length + 1);

                    var property = deserializedObject.GetProperty(propertyName);
                    if (property == null)
                        throw new InvalidOperationException($"Property {propertyName} not found on object of type {deserializedObject.GetType().Name}");

                    DeserializeObjectProperty(child, deserializedObject, property, context, propertiesSet);
                }
                else
                {
                    // 1.2.1 Find, which property is set as a so-called content property

                    var contentPropertyAttribute = deserializedObject.GetType().GetCustomAttribute<ContentPropertyAttribute>(true);
                    if (contentPropertyAttribute == null)
                        throw new InvalidOperationException($"Type {deserializedObject.GetType().Name} does not have ContentProperty specified. You have to specify property explicitly.");

                    var property = deserializedObject.GetProperty(contentPropertyAttribute.PropertyName);
                    if (property == null)
                        throw new InvalidOperationException($"Managed property {contentPropertyAttribute.PropertyName} specified as ContentProperty on type {deserializedObject.GetType().Name} does not exist!");

                    // 1.2.2 Deserialize object

                    var content = DeserializeElement(child, context);

                    if (property is ManagedSimpleProperty simpleProperty)
                    {
                        if (propertiesSet.Contains(property.Name))
                            throw new InvalidOperationException($"Property {property.Name} has been already set on type {deserializedObject.GetType().Name}");

                        deserializedObject.SetValue(simpleProperty, content);
                        propertiesSet.Add(String.Format(contentDecoration, property.Name));
                    }
                    else if (property is ManagedCollectionProperty collectionProperty)
                    {
                        if (propertiesSet.Contains(property.Name))
                            throw new InvalidOperationException($"Property {property.Name} has been already set on type {deserializedObject.GetType().Name}");

                        IList list = (IList)deserializedObject.GetValue(property);
                        list.Add(content);

                        propertiesSet.Add(String.Format(contentDecoration, property.Name));
                    }
                    else
                        throw new InvalidOperationException("Unsupported managed property!");
                }
            }
        }

        private void DeserializeObjectProperty(XmlNode propertyNode, 
            ManagedObject deserializedObject, 
            ManagedProperty property, 
            DeserializationContext context, 
            HashSet<string> propertiesSet)
        {
            if (propertiesSet.Contains(property.Name) || propertiesSet.Contains(String.Format(contentDecoration, property.Name)))
                throw new InvalidOperationException($"Property {property.Name} has been already set on type {deserializedObject.GetType().Name}");

            if (property is ManagedSimpleProperty simpleProperty)
            {
                if (propertyNode.ChildNodes.Count == 0)
                {
                    var content = TypeConverter.Deserialize(propertyNode.InnerText, property.Type);
                    deserializedObject.SetValue(property, content);

                    propertiesSet.Add(property.Name);
                }
                else if (propertyNode.ChildNodes.Count == 1)
                {
                    var content = DeserializeElement(propertyNode.FirstChild, context);
                    deserializedObject.SetValue(property, content);

                    propertiesSet.Add(property.Name);
                }
                else
                    throw new InvalidOperationException($"Property {property.Name} on type {deserializedObject.GetType().Name} is a simple property, but is provided with multiple values.");
            }
            else if (property is ManagedCollectionProperty collectionProperty)
            {
                var list = (IList)deserializedObject.GetValue(property);

                foreach (XmlNode child in propertyNode.ChildNodes)
                {
                    var content = DeserializeElement(child, context);
                    list.Add(content);
                }

                propertiesSet.Add(property.Name);
            }
            else
                throw new InvalidOperationException("Unsupported managed property!");
        }

        private void DeserializeAttributes(XmlNode node, ManagedObject deserializedObject, DeserializationContext context, HashSet<string> propertiesSet)
        {
            foreach (XmlAttribute attribute in node.Attributes)
            {
                // 1. Omit xmlns definitions

                if (attribute.Name == "xmlns" || attribute.Name.StartsWith("xmlns:"))
                    continue;

                // 2. Check if property hasn't been already set

                if (propertiesSet.Contains(attribute.LocalName))
                    throw new InvalidOperationException($"Property {attribute.LocalName} has been already set on type {deserializedObject.GetType().Name}");

                // 3. Find property with appropriate name

                ManagedProperty managedProperty = deserializedObject.GetProperty(attribute.LocalName);

                if (managedProperty == null)
                    throw new InvalidOperationException($"Property {attribute.LocalName} not found on object of type {deserializedObject.GetType().Name}");

                if (!(managedProperty is ManagedSimpleProperty property))
                    throw new InvalidOperationException($"Property {attribute.LocalName} of {deserializedObject.GetType().Name} is not a simple managed property!\r\nOnly simple managed properties may be set via XML attributes.");

                // 4. Deserialize value

                var value = attribute.Value;
                object deserializedValue = TypeConverter.Deserialize(value, property.Type);

                // 5. Set value to property and mark as set

                deserializedObject.SetValue(property, deserializedValue);
                propertiesSet.Add(attribute.LocalName);
            }
        }

        private ManagedObject DeserializeElement(XmlNode node, DeserializationContext context)
        {
            // 1. Figure out, which object to instantiate

            string className = node.LocalName;
            
            if (!context.Namespaces.TryGetValue(node.NamespaceURI, out NamespaceDefinition namespaceDefinition))
            {
                namespaceDefinition = ParseNamespaceDefinition(node.NamespaceURI);
                context.Namespaces[node.NamespaceURI] = namespaceDefinition;
            }

            // 2. Get type from the specified assembly + namespace + class name

            var assembly = AppDomain.CurrentDomain.GetAssemblies().
                SingleOrDefault(assembly => assembly.GetName().Name == namespaceDefinition.Assembly);

            if (assembly == null)
                try
                {
                    assembly = Assembly.Load(namespaceDefinition.Assembly);
                }
                catch (FileNotFoundException)
                {
                    // Intentionally left empty, will leave assembly as null.
                }

            if (assembly == null)
                throw new InvalidOperationException($"Cannot access assembly {namespaceDefinition.Assembly}\r\nMake sure, it is loaded or accessible to load.");

            string fullClassTypeName = String.Join('.', namespaceDefinition.Namespace, className);
            Type objType = assembly.GetType(fullClassTypeName, false);

            if (objType == null)
                throw new InvalidOperationException($"Cannot find type {fullClassTypeName} in assembly {namespaceDefinition.Assembly}");

            // 3. Make sure, that object derives from ManagedObject - only those are supported

            if (!objType.IsAssignableTo(typeof(ManagedObject)))
                throw new InvalidOperationException($"Type {fullClassTypeName} does not derive from ManagedObject!");

            // 4. Try to instantiate object

            ManagedObject deserializedObject;

            try
            {
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(objType.TypeHandle);
                deserializedObject = (ManagedObject)Activator.CreateInstance(objType);
            }
            catch
            {
                throw new InvalidOperationException($"Type {fullClassTypeName} does not contain public, parameterless constructor!");
            }

            // 5. Load attributes

            var setProperties = new HashSet<string>();

            DeserializeAttributes(node, deserializedObject, context, setProperties);

            // 6. Load children

            DeserializeChildren(node, deserializedObject, context, setProperties);

            // 7. Return deserialized object

            return deserializedObject;
        }

        // Public methods -----------------------------------------------------

        public ManagedObject Deserialize(string filename, DeserializationOptions options = null)
        {
            XmlDocument document = new XmlDocument();
            document.Load(filename);
            return Deserialize(document);
        }

        public ManagedObject Deserialize(XmlDocument document, DeserializationOptions options = null)
        {
            var context = new DeserializationContext();

            if (options != null)
            {
                if (options.DefaultNamespace != null)
                {
                    context.Namespaces[String.Empty] = options.DefaultNamespace;
                }
            }

            return DeserializeElement(document.FirstChild, context);
        }
    }
}
