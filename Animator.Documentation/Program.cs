using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;
using System.Text;
using System.Reflection;
using Animator.Engine.Elements;
using Animator.Engine.Base;
using Animator.Engine.Types;
using System.Text.RegularExpressions;
using System.Web;

namespace Animator.Documentation
{
    static class Program
    {
        private static readonly Regex genericTypeRegex = new Regex("(.*)`([0-9]+)");

        private static readonly HashSet<Type> staticallyInitializedTypes = new();

        private static string MakeLinkToType(string type) => $"<a href=\"#{type}\">{type}</a>";

        private static string ToReadableName(this Type type)
        {
            if (!type.IsGenericType)
                return type.Name;

            var baseName = type.Name[0..type.Name.IndexOf('`')];

            List<string> genericArguments = type.GenericTypeArguments
                .Select(t => ToReadableName(t))
                .ToList();

            return $"{baseName}<{string.Join(',', genericArguments)}>";
        }

        private static void BuildPropertiesDocumentation(StringBuilder sb, Type type, List<PropertyInfo> properties)
        {
            StaticInitializeRecursively(type);

            foreach (var property in properties)
            {
                sb.AppendLine("<tr>");
                sb.AppendLine("<td>");
                sb.AppendLine($"<pre><code>{HttpUtility.HtmlEncode(property.PropertyType.ToReadableName())}</code></pre>");
                sb.AppendLine("</td>");
                sb.AppendLine("<td>");
                sb.AppendLine($"<pre><code><strong>{property.Name}</strong></code></pre>");
                sb.AppendLine("</td>");
                sb.AppendLine("<td>");

                var propDocumentation = property.GetDocumentation();
                if (propDocumentation != null)
                {
                    var summary = propDocumentation["member"]?["summary"];

                    if (summary != null)
                    {
                        sb.AppendLine(summary.InnerXml);
                    }

                    var managedProperty = ManagedProperty.FindByTypeAndName(property.DeclaringType, property.Name);
                    if (managedProperty != null)
                    {
                        if (managedProperty is ManagedSimpleProperty simpleProperty)
                        {
                            sb.Append("<p>");

                            if (simpleProperty.Metadata.Inheritable)
                                sb.AppendLine("May be inherited by child elements. ");
                            else
                                sb.AppendLine("Not inheritable by child elements. ");

                            if (simpleProperty.Metadata.InheritedFromParent)
                                sb.AppendLine("Value is inherited from parent element if none provided explicitly. ");
                            if (simpleProperty.Metadata.NotAnimatable)
                                sb.AppendLine("Cannot be animated. ");

                            sb.AppendLine($"Default value is {simpleProperty.Metadata.DefaultValue?.ToString()}.");

                            sb.AppendLine("</p>");
                        }
                    }


                    var example = propDocumentation["member"]?["example"];

                    if (example != null)
                    {
                        sb.Append("<p><strong>Example:</strong></p>");
                        sb.AppendLine(example.InnerXml);
                    }
                }

                sb.AppendLine("</td>");
            }
        }

        private static void BuildTypeDocumentation(Type type, Assembly engineAssembly, string elementsNamespace, StringBuilder sb)
        {
            // Header

            sb.Append($"<h1><a name=\"{type.ToReadableName()}\"></a>{type.ToReadableName()}");
            if (type.IsAbstract)
            {
                sb.Append(" <em>(abstract)</em>");
            }
            sb.Append("</h1>");
            sb.AppendLine();

            // Inheritance

            Type rootType;
            if (type.IsAssignableTo(typeof(ManagedObject)))
                rootType = typeof(ManagedObject);
            else if (type.IsAssignableTo(typeof(BaseMarkupExtension)))
                rootType = typeof(BaseMarkupExtension);
            else
                throw new InvalidOperationException("Type does not derive from one of known types!");

            sb.AppendLine("<h2>Inheritance</h2>");

            sb.Append("<pre><code>");
            var inheritedTypes = new List<Type>();
            Type parentType = type;
            while (parentType != rootType)
            {
                inheritedTypes.Add(parentType);
                parentType = parentType.BaseType;
            }

            for (int i = inheritedTypes.Count - 1; i >= 0; i--)
            {
                if (inheritedTypes[i].IsAbstract)
                    sb.Append(inheritedTypes[i].ToReadableName());
                else
                    sb.Append(MakeLinkToType(inheritedTypes[i].ToReadableName()));

                if (i > 0)
                    sb.Append(" » ");
            }
            sb.AppendLine("</pre></code>");

            // Derived types

            var derivedTypes = engineAssembly.GetTypes()
                .Where(t => t.Namespace == elementsNamespace && t.BaseType == type)
                .ToList();

            if (derivedTypes.Any())
            {
                sb.AppendLine("<h2>Derived types</h2>");

                sb.AppendLine("<ul>");
                foreach (var derivedType in derivedTypes)
                {
                    sb.AppendLine($"<li>{MakeLinkToType(derivedType.ToReadableName())}</li>");
                }
                sb.AppendLine("</ul>");
            }

            // Description

            var documentation = type.GetDocumentation();

            if (documentation != null)
            {
                var summary = documentation["member"]?["summary"];
                if (summary != null)
                {
                    sb.AppendLine("<h2>Description</h2>");
                    sb.AppendLine(summary.InnerXml.Trim());
                }

                var remarks = documentation["member"]?["remarks"];
                if (remarks != null)
                {
                    sb.AppendLine("<h2>Remarks</h2>").AppendLine();
                    sb.AppendLine(remarks.InnerXml.Trim());
                }
            }

            // Properties

            bool propertiesHeaderShown = false;

            Type currentType = type;

            while (currentType != typeof(ManagedObject) && currentType != typeof(object))
            {
                var properties = currentType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    .Where(p => !p.CustomAttributes.Any(a => a.AttributeType == typeof(DoNotDocumentAttribute)))
                    .ToList();

                if (properties.Any())
                {
                    if (!propertiesHeaderShown)
                    {
                        sb.AppendLine("<h2>Properties</h2>");
                        sb.AppendLine("<table class=\"properties\">");
                        propertiesHeaderShown = true;
                    }

                    if (currentType != type)
                        sb.AppendLine($"<tr><td colspan=\"3\" class=\"table-separator\">Properties derived from <strong>{ToReadableName(currentType)}:</strong></td></tr>");

                    BuildPropertiesDocumentation(sb, currentType, properties);
                }

                currentType = currentType.BaseType;
            }

            if (propertiesHeaderShown)
            {
                sb.AppendLine("</table>");
            }
        }

        private static void StaticInitializeRecursively(Type type)
        {
            do
            {
                // If type is initialized, its base types must have been initialized too,
                // don't waste time on them
                if (staticallyInitializedTypes.Contains(type))
                    return;

                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);

                staticallyInitializedTypes.Add(type);

                type = type.BaseType;
            }
            while (type != typeof(ManagedObject) && type != typeof(object));
        }

        private static StringBuilder BuildDocumentation(Assembly engineAssembly, string elementsNamespace)
        {
            StringBuilder sb = new StringBuilder();

            var preStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Animator.Documentation.Resources.documentation_prefix.html");
            using (StreamReader sr = new StreamReader(preStream))
                sb.AppendLine(sr.ReadToEnd());

            var elementTypes = engineAssembly.GetTypes()
                .Where(t => t.IsPublic && !t.IsAbstract && t.Namespace == elementsNamespace && !t.CustomAttributes.Any(a => a.AttributeType == typeof(DoNotDocumentAttribute)))
                .ToList();

            for (int i = 0; i < elementTypes.Count; i++)
            {
                var type = elementTypes[i];

                if (i > 0)
                    sb.AppendLine("<hr />").AppendLine();

                BuildTypeDocumentation(type, engineAssembly, elementsNamespace, sb);
            }

            var postStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Animator.Documentation.Resources.documentation_postfix.html");
            using (StreamReader sr = new StreamReader(postStream))
                sb.AppendLine(sr.ReadToEnd());

            return sb;
        }

        private static StringBuilder BuildClassTree(Assembly engineAssembly, string elementsNamespace)
        {
            var sb = new StringBuilder();

            sb.AppendLine("digraph {");
            sb.AppendLine("  rankdir=LR");

            var elementTypes = engineAssembly.GetTypes()
                .Where(t => t.IsPublic && t.Namespace == elementsNamespace && !t.CustomAttributes.Any(a => a.AttributeType == typeof(DoNotDocumentAttribute)))
                .ToList();

            int index = 0;
            Dictionary<Type, int> types = new();

            var allTypes = elementTypes.Union(elementTypes.Select(t => t.BaseType)).ToList();
            allTypes.ForEach(t => types[t] = index++);

            foreach (var type in allTypes)
            {
                sb.AppendLine($"{types[type]} [ label = \"{type.ToReadableName()}\" ]");
            }

            foreach (var type in elementTypes)
            {
                sb.AppendLine($"{types[type.BaseType]} -> {types[type]}");
            }

            sb.AppendLine("}");

            return sb;
        }

        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: ");
                Console.WriteLine("");
                Console.WriteLine("Animator.Documentation <path to Animator.Engine.xml> <outfile.html> <outfile.dot>");

                Console.WriteLine("Run with full path to Animator.Engine.xml documentation file.");
                return;
            }

            Documentation.LoadXmlDocumentation(File.ReadAllText(args[0]));


            // Find assembly with elements and its current namespace

            var elementsNamespace = typeof(Movie).Namespace;
            var engineAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .Single(a => a.GetName().Name == "Animator.Engine");

            // Build documentation

            var sb = BuildDocumentation(engineAssembly, elementsNamespace);

            // Save documentation

            File.WriteAllText(args[1], sb.ToString());

            // Build class tree

            sb = BuildClassTree(engineAssembly, elementsNamespace);

            // Save class tree

            File.WriteAllText(args[2], sb.ToString());
        }
    }
}
