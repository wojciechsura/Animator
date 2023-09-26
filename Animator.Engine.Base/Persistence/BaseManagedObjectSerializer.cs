using Animator.Engine.Base;
using Animator.Engine.Base.Exceptions;
using Animator.Engine.Base.Extensions;
using Animator.Engine.Base.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Animator.Engine.Base.Persistence
{
    public class BaseManagedObjectSerializer
    {
        // Protected types ----------------------------------------------------

        protected record class ObjectTypeData(Type Type,
            string FullClassName)
        {

        }

        protected record class MarkupExtensionData(string Name,
            ObjectTypeData TypeData,
            List<(string property, string value)> Params)
        {

        }

        // Private constants --------------------------------------------------

        protected const string ENGINE_NAMESPACE = "https://spooksoft.pl/animator";

        protected const string CONTENT_DECORATION = "content:{0}";

        protected const string MACROS_ELEMENT = "Macros";
        protected const string MACRO_ELEMENT = "Macro";
        protected const string INCLUDE_ELEMENT = "Include";
        protected const string GENERATE_ELEMENT = "Generate";

        protected const string KEY_ATTRIBUTE = "Key";

        protected const string SOURCE_ATTRIBUTE = "Source";

        protected const string NS_ASSEMBLY_PREFIX = "assembly=";
        protected const string NS_NAMESPACE_PREFIX = "namespace=";

        protected const string MARKUP_EXTENSION_BEGIN = "{";
        protected const string MARKUP_EXTENSION_ESCAPED_BEGIN = "{{";
        protected const string MARKUP_EXTENSION_END = "}";

        protected static readonly Regex markupExtensionRegex = new Regex(@"\{\s*(?<Name>[^\s,=\}]+)\s*(?<Params>[^\s]|[^\s].*[^\s])?\s*\}");

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

            if (!elements[0].StartsWith(NS_ASSEMBLY_PREFIX) || elements[0].Length < 10)
                throw new ParseException($"Invalid namespace definition: {namespaceDefinition}\r\nMissing or invalid assembly section!");

            string assembly = elements[0].Substring(9);

            // Namespace

            if (!elements[1].StartsWith(NS_NAMESPACE_PREFIX) || elements[1].Length < 11)
                throw new ParseException($"Invalid namespace definition: {namespaceDefinition}\r\nMissing or invalid namespace section!");

            string @namespace = elements[1][10..];

            return new NamespaceDefinition(assembly, @namespace);
        }

        // Protected methods --------------------------------------------------

        protected MarkupExtensionData DeserializeMarkupExtension(string value,
            XmlNode node,
            Dictionary<string, NamespaceDefinition> namespaces)
        {
            // 1. Parse entry

            var match = markupExtensionRegex.Match(value);
            if (!match.Success)
                throw new SerializerException("Invalid markup extension structure!", node.FindXPath());

            string name = null;
            string defaultParam = null;
            List<(string property, string value)> @params = new();

            var nameGroup = match.Groups
                .OfType<Group>()
                .SingleOrDefault(g => g.Name == "Name" && g.Success);

            name = nameGroup.Value;

            var paramStringGroup = match.Groups
                .OfType<Group>()
                .SingleOrDefault(g => g.Name == "Params" && g.Success);

            if (paramStringGroup != null)
            {
                string[] paramStrings;
                try
                {
                    paramStrings = paramStringGroup.Value
                        .SplitUnquoted(',')
                        .Select(x => x.Trim())
                        .ToArray();
                }
                catch (Exception e)
                {
                    throw new SerializerException("Invalid markup extension structure!", node.FindXPath(), e);
                }

                for (int i = 0; i < paramStrings.Length; i++)
                {
                    if (i == 0 && !paramStrings[i].ContainsUnquoted('='))
                        defaultParam = paramStrings[i].ExpandQuotes();
                    else
                    {
                        var values = paramStrings[i].SplitUnquoted('=');
                        if (values.Length != 2)
                            throw new SerializerException($"Invalid markup extension structure: invalid parameter definition: {paramStrings[i]}!", node.FindXPath());

                        @params.Add((values[0], values[1].ExpandQuotes()));
                    }
                }
            }

            // 2. Figure out object type

            string namespaceKey = string.Empty;
            string className = string.Empty;

            if (name.Contains(':'))
            {
                var data = name.Split(':');
                if (data.Length != 2)
                    throw new SerializerException($"Invalid markup extension class name: {name}!", node.FindXPath());

                namespaceKey = data[0];
                className = data[1];
            }
            else
            {
                className = name;
            }

            string namespaceUri = node.GetNamespaceOfPrefix($"{namespaceKey}");

            var typeData = ExtractObjectType(namespaceUri, className, typeof(BaseMarkupExtension), namespaces);

            // 3. Process default parameter

            if (defaultParam != null)
            {
                var defaultPropertyAttribute = typeData.Type.GetCustomAttribute<DefaultPropertyAttribute>(true);
                if (defaultPropertyAttribute == null)
                    throw new SerializerException($"Markup extension {name} doesn't have default parameter defined!", node.FindXPath());

                @params.Add((defaultPropertyAttribute.PropertyName, defaultParam));
            }

            // Every param must be used only once.

            if (@params.Select(p => p.property).Distinct().Count() != @params.Count)
                throw new SerializerException($"One of parameters for {name} is defined more than once (it may be as well the default one!)", node.FindXPath());

            // Return results

            return new MarkupExtensionData(name, typeData, @params);
        }

        protected ObjectTypeData ExtractObjectType(string namespaceUri,
            string className,
            Type requiredBaseType,
            Dictionary<string, NamespaceDefinition> namespaces)
        {
            // 1. Figure out, which object to instantiate

            if (!namespaces.TryGetValue(namespaceUri, out NamespaceDefinition namespaceDefinition))
            {
                namespaceDefinition = ParseNamespaceDefinition(namespaceUri);
                namespaces[namespaceUri] = namespaceDefinition;
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
                throw new ActivatorException($"Cannot access assembly {namespaceDefinition.Assembly}\r\nMake sure, it is loaded or accessible to load.");

            string fullClassTypeName = string.Join('.', namespaceDefinition.Namespace, className);
            Type objType = assembly.GetType(fullClassTypeName, false);
            if (objType == null)
                throw new ActivatorException($"Cannot find type {fullClassTypeName} in assembly {namespaceDefinition.Assembly}");

            // 3. Make sure, that object derives from ManagedObject - only those are supported

            if (!objType.IsAssignableTo(requiredBaseType))
                throw new ActivatorException($"Type {fullClassTypeName} does not derive from {requiredBaseType.Name}!");

            return new ObjectTypeData(objType, fullClassTypeName);
        }
    }
}
