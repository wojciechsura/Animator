using Animator.Engine.Base;
using Animator.Engine.Base.Exceptions;
using Animator.Engine.Base.Persistence.Types;
using Animator.Engine.Base.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Animator.Engine.Base.Persistence
{
    public class ManagedObjectSerializer
    {
        // Private types ------------------------------------------------------

        private class DeserializationContext
        {
            public DeserializationContext(CustomActivator customActivator, Dictionary<Type, TypeSerializer> customTypeSerializers)
            {
                CustomActivator = customActivator;
                CustomTypeSerializers = customTypeSerializers;
            }

            public Dictionary<string, NamespaceDefinition> Namespaces { get; } = new();
            public HashSet<Type> StaticallyInitializedTypes { get; } = new();
            public CustomActivator CustomActivator { get; }
            public Dictionary<Type, TypeSerializer> CustomTypeSerializers { get; }
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
                throw new ParseException($"Invalid namespace definition: {namespaceDefinition}\r\nThere should be exactly two sections separated with semicolon (;).");

            // Assembly

            if (!elements[0].StartsWith("assembly=") || elements[0].Length < 10)
                throw new ParseException($"Invalid namespace definition: {namespaceDefinition}\r\nMissing or invalid assembly section!");

            string assembly = elements[0].Substring(9);

            // Namespace

            if (!elements[1].StartsWith("namespace=") || elements[1].Length < 11)
                throw new ParseException($"Invalid namespace definition: {namespaceDefinition}\r\nMissing or invalid namespace section!");

            string @namespace = elements[1][10..];

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
                        throw new SerializerException($"Property {propertyName} not found on object of type {deserializedObject.GetType().Name}",
                            node.FindXPath());
                    if (property.Metadata.NotSerializable)
                        throw new SerializerException($"Property {propertyName} on object {deserializedObject.GetType().Name} is not serializable!\r\nRemove it from input file.",
                            node.FindXPath());

                    DeserializeObjectProperty(child, deserializedObject, property, context, propertiesSet);
                }
                else
                {
                    // 1.2.1 Find, which property is set as a so-called content property

                    var contentPropertyAttribute = deserializedObject.GetType().GetCustomAttribute<ContentPropertyAttribute>(true);
                    if (contentPropertyAttribute == null)
                        throw new SerializerException($"Type {deserializedObject.GetType().Name} does not have ContentProperty specified. You have to specify property explicitly.",
                            node.FindXPath());

                    var property = deserializedObject.GetProperty(contentPropertyAttribute.PropertyName);
                    if (property == null)
                        throw new SerializerException($"Managed property {contentPropertyAttribute.PropertyName} specified as ContentProperty on type {deserializedObject.GetType().Name} does not exist!",
                            node.FindXPath());
                    if (property.Metadata.NotSerializable)
                        throw new SerializerException($"Property {contentPropertyAttribute.PropertyName} on object {deserializedObject.GetType().Name} is not serializable!\r\nThe data structure is ill-formed: content property should be serializable.",
                            node.FindXPath());

                    // 1.2.2 Deserialize object

                    var content = DeserializeElement(child, context);

                    if (property is ManagedAnimatedProperty animatedProperty)
                    {
                        if (propertiesSet.Contains(property.Name))
                            throw new SerializerException($"Property {property.Name} has been already set on type {deserializedObject.GetType().Name}",
                                node.FindXPath());

                        deserializedObject.SetValue(animatedProperty, content);
                        propertiesSet.Add(string.Format(contentDecoration, property.Name));
                    }
                    else if (property is ManagedCollectionProperty collectionProperty)
                    {
                        if (propertiesSet.Contains(property.Name))
                            throw new SerializerException($"Property {property.Name} has been already set on type {deserializedObject.GetType().Name}",
                                node.FindXPath());

                        IList list = (IList)deserializedObject.GetValue(property);
                        list.Add(content);

                        propertiesSet.Add(string.Format(contentDecoration, property.Name));
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
                throw new SerializerException($"Cannot deserialize non-serializable property {property.Name}!",
                    propertyNode.FindXPath());

            if (propertiesSet.Contains(property.Name) || propertiesSet.Contains(string.Format(contentDecoration, property.Name)))
                throw new SerializerException($"Property {property.Name} has been already set on type {deserializedObject.GetType().Name}",
                    propertyNode.FindXPath());

            if (property is ManagedAnimatedProperty animatedProperty)
            {
                if (propertyNode.ChildNodes.OfType<XmlElement>().Count() == 0)
                {
                    var content = DeserializePropertyValue(context, animatedProperty, propertyNode.InnerText);
                    deserializedObject.SetValue(property, content);

                    propertiesSet.Add(property.Name);
                }
                else if (propertyNode.ChildNodes.OfType<XmlElement>().Count() == 1)
                {
                    var content = DeserializeElement(propertyNode.FirstChild, context);
                    deserializedObject.SetValue(property, content);

                    propertiesSet.Add(property.Name);
                }
                else
                    throw new SerializerException($"Property {property.Name} on type {deserializedObject.GetType().Name} is an animated property, but is provided with multiple values.",
                        propertyNode.FindXPath());
            }
            else if (property is ManagedCollectionProperty collectionProperty)
            {
                if (propertyNode.ChildNodes.OfType<XmlElement>().Count() == 0)
                {
                    var value = DeserializeCollectionPropertyValue(context, collectionProperty, propertyNode.InnerText);

                    if (value != null)
                    {
                        IList collection = (IList)deserializedObject.GetValue(collectionProperty);

                        foreach (object obj in value)
                            collection.Add(obj);

                        propertiesSet.Add(propertyNode.LocalName);
                    }
                    else
                        throw new SerializerException($"Property {propertyNode.LocalName} of {deserializedObject.GetType().Name} is a collection property, but its value is stored in attribute and no custom serializer is provided!",
                            propertyNode.FindXPath());
                }
                else
                {
                    var list = (IList)deserializedObject.GetValue(property);

                    foreach (XmlNode child in propertyNode.ChildNodes)
                    {
                        var content = DeserializeElement(child, context);
                        list.Add(content);
                    }

                    propertiesSet.Add(property.Name);
                }
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
                    throw new SerializerException($"Property {attribute.LocalName} has been already set on type {deserializedObject.GetType().Name}",
                        node.FindXPath());

                // 3. Find property with appropriate name

                ManagedProperty managedProperty = deserializedObject.GetProperty(attribute.LocalName);

                if (managedProperty == null)
                    throw new SerializerException($"Property {attribute.LocalName} not found on object of type {deserializedObject.GetType().Name}",
                        node.FindXPath());
                if (managedProperty.Metadata.NotSerializable)
                    throw new SerializerException($"Property {attribute.LocalName} on object {deserializedObject.GetType().Name} is not serializable!\r\nRemove it from input file.",
                        node.FindXPath());

                if (managedProperty is ManagedAnimatedProperty animatedProperty)
                {
                    string propertyValue = attribute.Value;

                    object value = DeserializePropertyValue(context, animatedProperty, attribute.Value);

                    deserializedObject.SetValue(animatedProperty, value);
                    propertiesSet.Add(attribute.LocalName);
                }
                else if (managedProperty is ManagedCollectionProperty collectionProperty)
                {
                    IList value = DeserializeCollectionPropertyValue(context, collectionProperty, attribute.Value);

                    if (value != null)
                    {
                        IList collection = (IList)deserializedObject.GetValue(collectionProperty);

                        foreach (object obj in value)
                            collection.Add(obj);

                        propertiesSet.Add(attribute.LocalName);
                    }
                    else
                        throw new SerializerException($"Property {attribute.LocalName} of {deserializedObject.GetType().Name} is a collection property, but its value is stored in attribute and no custom serializer is provided!",
                            node.FindXPath());
                }
                else
                    throw new InvalidOperationException("Unsupported property type!");
            }
        }

        private object DeserializePropertyValue(DeserializationContext context, ManagedAnimatedProperty animatedProperty, string propertyValue)
        {
            if (animatedProperty.Metadata.CustomSerializer != null && animatedProperty.Metadata.CustomSerializer.CanDeserialize(propertyValue))
                return animatedProperty.Metadata.CustomSerializer.Deserialize(propertyValue);
            else if (context.CustomTypeSerializers != null && context.CustomTypeSerializers.TryGetValue(animatedProperty.Type, out TypeSerializer customSerializer) && customSerializer.CanDeserialize(propertyValue))
                return customSerializer.Deserialize(propertyValue);
            else
                return TypeSerialization.Deserialize(propertyValue, animatedProperty.Type);
        }

        private static IList DeserializeCollectionPropertyValue(DeserializationContext context, ManagedCollectionProperty collectionProperty, string propertyValue)
        {
            if (collectionProperty.Metadata.CustomSerializer != null && collectionProperty.Metadata.CustomSerializer.CanDeserialize(propertyValue))
                return (IList)collectionProperty.Metadata.CustomSerializer.Deserialize(propertyValue);
            if (context.CustomTypeSerializers != null && context.CustomTypeSerializers.TryGetValue(collectionProperty.Type, out TypeSerializer customSerializer) && customSerializer.CanDeserialize(propertyValue))
                return (IList)customSerializer.Deserialize(propertyValue);

            return null;
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
                throw new SerializerException($"Cannot access assembly {namespaceDefinition.Assembly}\r\nMake sure, it is loaded or accessible to load.",
                    node.FindXPath());

            string fullClassTypeName = string.Join('.', namespaceDefinition.Namespace, className);
            Type objType = assembly.GetType(fullClassTypeName, false);

            if (objType == null)
                throw new SerializerException($"Cannot find type {fullClassTypeName} in assembly {namespaceDefinition.Assembly}",
                    node.FindXPath());

            // 3. Make sure, that object derives from ManagedObject - only those are supported

            if (!objType.IsAssignableTo(typeof(ManagedObject)))
                throw new SerializerException($"Type {fullClassTypeName} does not derive from ManagedObject!",
                    node.FindXPath());

            // 4. Try to instantiate object

            ManagedObject deserializedObject;

            try
            {
                // We have to run static initializers manually, because this is not done automatically.
                // If we don't do that, managed properties will not be registered.
                StaticInitializeRecursively(context, objType);

                if (context.CustomActivator != null)
                    deserializedObject = (ManagedObject)context.CustomActivator.CreateInstance(objType);
                else
                    deserializedObject = (ManagedObject)Activator.CreateInstance(objType);
            }
            catch (Exception e)
            {
                throw new SerializerException($"Cannot instantiate type {fullClassTypeName}. Check inner exception for details.",
                    node.FindXPath(),
                    e);
            }

            // 5. Load attributes

            var setProperties = new HashSet<string>();

            DeserializeAttributes(node, deserializedObject, context, setProperties);

            // 6. Load children

            DeserializeChildren(node, deserializedObject, context, setProperties);

            // 7. Return deserialized object

            return deserializedObject;
        }

        private static void StaticInitializeRecursively(DeserializationContext context, Type objType)
        {
            if (context.StaticallyInitializedTypes.Contains(objType))
                return;

            var type = objType;

            do
            {
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                context.StaticallyInitializedTypes.Add(type);

                type = type.BaseType;
            }
            while (type != typeof(ManagedObject) && type != typeof(object));
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
            var context = new DeserializationContext(options?.CustomActivator, options?.CustomSerializers);

            if (options != null)
            {
                if (options.DefaultNamespace != null)
                {
                    context.Namespaces[string.Empty] = options.DefaultNamespace;
                }
            }

            return DeserializeElement(document.FirstChild, context);
        }
    }
}
