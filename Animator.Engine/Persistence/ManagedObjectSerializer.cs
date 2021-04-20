using Animator.Engine.Base;
using Animator.Engine.Elements;
using Animator.Engine.Persistence.Types;
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
            public DeserializationContext(CustomActivator customActivator)
            {
                CustomActivator = customActivator;
            }

            public Dictionary<string, NamespaceDefinition> Namespaces { get; } = new();
            public CustomActivator CustomActivator { get; }
        }

        // Private constants --------------------------------------------------

        private const string contentDecoration = "content:{0}";

        // Private methods ----------------------------------------------------

        /// <summary>
        /// Parses namespace from string into instance of NamespaceDefinition
        /// </summary>        
        /// <remarks>
        /// Namespace must be in format: <code>assembly=A.B.C;namespace=X.Y.Z</code>,
        /// whitespace- and case-sensitive.
        /// </remarks>
        private NamespaceDefinition ParseNamespaceDefinition(string namespaceDefinition)
        {
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

        /// <summary>
        /// Deserializes child nodes of given node. Those may be
        /// either properties stored in extended form, values
        /// of content property, or contents of collection (if
        /// deserialized node is one)
        /// </summary>
        private void DeserializeChildren(XmlNode node, 
            ManagedObject deserializedObject, 
            DeserializationContext context, 
            HashSet<string> propertiesSet)
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
                    if (property.Metadata.NotSerializable)
                        throw new InvalidOperationException($"Property {propertyName} on object {deserializedObject.GetType().Name} is not serializable!\r\nRemove it from input file.");

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
                    if (property.Metadata.NotSerializable)
                        throw new InvalidOperationException($"Property {contentPropertyAttribute.PropertyName} on object {deserializedObject.GetType().Name} is not serializable!\r\nThe data structure is ill-formed: content property should be serializable.");

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

        /// <summary>
        /// Deserializes property node (eg. for &lt;Element&gt;, a property
        /// node would be an &lt;Element.Property&gt;) into specific
        /// deserialized object's property.
        /// </summary>
        private void DeserializeObjectProperty(XmlNode propertyNode, 
            ManagedObject deserializedObject, 
            ManagedProperty property, 
            DeserializationContext context, 
            HashSet<string> propertiesSet)
        {
            if (property.Metadata.NotSerializable)
                throw new ArgumentException("Cannot deserialize non-serializable property!");

            if (propertiesSet.Contains(property.Name) || propertiesSet.Contains(String.Format(contentDecoration, property.Name)))
                throw new InvalidOperationException($"Property {property.Name} has been already set on type {deserializedObject.GetType().Name}");

            if (property is ManagedSimpleProperty simpleProperty)
            {
                if (propertyNode.ChildNodes.Count == 0)
                {
                    var content = TypeSerialization.Deserialize(propertyNode.InnerText, property.Type);
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

        /// <summary>
        /// Deserialize attributes of given node into object's properties
        /// </summary>
        private void DeserializeAttributes(XmlNode node, 
            ManagedObject deserializedObject, 
            DeserializationContext context, 
            HashSet<string> propertiesSet)
        {
            foreach (XmlAttribute attribute in node.Attributes)
            {
                // 1. Omit xmlns definitions, for some reason they are
                // being treated as regular attributes too

                if (attribute.Name == "xmlns" || attribute.Name.StartsWith("xmlns:"))
                    continue;

                // 2. Check if property hasn't been already set

                if (propertiesSet.Contains(attribute.LocalName))
                    throw new InvalidOperationException($"Property {attribute.LocalName} has been already set on type {deserializedObject.GetType().Name}");

                // 3. Find property with appropriate name

                ManagedProperty managedProperty = deserializedObject.GetProperty(attribute.LocalName);

                if (managedProperty == null)
                    throw new InvalidOperationException($"Property {attribute.LocalName} not found on object of type {deserializedObject.GetType().Name}");
                if (managedProperty.Metadata.NotSerializable)
                    throw new InvalidOperationException($"Property {attribute.LocalName} on object {deserializedObject.GetType().Name} is not serializable!\r\nRemove it from input file.");

                if (managedProperty is ManagedSimpleProperty simpleProperty)
                {
                    // 3.1.1 Check for custom serializer

                    object value;
                    if (simpleProperty.Metadata.Serializer != null)
                    {
                        value = simpleProperty.Metadata.Serializer.Deserialize(attribute.Value);
                    }
                    else
                    {
                        value = TypeSerialization.Deserialize(attribute.Value, simpleProperty.Type);
                    }

                    // 4. Set value to property and mark as set

                    deserializedObject.SetValue(simpleProperty, value);
                    propertiesSet.Add(attribute.LocalName);
                }
                else if (managedProperty is ManagedCollectionProperty collectionProperty)
                {
                    // 3.2.1 Check for custom serializer

                    if (collectionProperty.Metadata.CustomSerializer != null)
                    {
                        IList value = collectionProperty.Metadata.CustomSerializer.Deserialize(attribute.Value);

                        IList collection = (IList)deserializedObject.GetValue(collectionProperty);

                        if (value != null)
                            foreach (object obj in value)
                                collection.Add(obj);

                        propertiesSet.Add(attribute.LocalName);
                    }
                    else
                        throw new InvalidOperationException($"Property {attribute.LocalName} of {deserializedObject.GetType().Name} is a collection property, but its value is stored in attribute and no custom serializer is provided!");
                }
                else
                    throw new InvalidOperationException("Unsupported property type!");
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

                if (context.CustomActivator != null)
                    deserializedObject = (ManagedObject)context.CustomActivator.CreateInstance(objType);
                else
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
            return Deserialize(document, options);
        }

        public ManagedObject Deserialize(XmlDocument document, DeserializationOptions options = null)
        {
            var context = new DeserializationContext(options?.CustomActivator);

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
