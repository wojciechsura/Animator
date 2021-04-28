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

namespace Animator.Documentation
{
    static class Program
    {
        private static readonly Regex genericTypeRegex = new Regex("(.*)`([0-9]+)");

        private static string MakeLinkToType(string type) => $"[{type}]({type})";

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

        private static void BuildTypeDocumentation(Type type, Assembly engineAssembly, string elementsNamespace, StringBuilder sb)
        {
            // Header

            sb.Append($"# <a name=\"{type.ToReadableName()}\"></a>{type.ToReadableName()}");
            if (type.IsAbstract)
            {
                sb.Append(" *(abstract)*");
            }
            sb.AppendLine();

            // Inheritance

            sb.AppendLine("### Inheritance");

            var inheritedTypes = new List<string>();
            Type parentType = type;
            while (parentType != typeof(ManagedObject))
            {
                inheritedTypes.Add(parentType.ToReadableName());
                parentType = parentType.BaseType;
            }

            for (int i = inheritedTypes.Count - 1; i >= 0; i--)
            {
                sb.Append(MakeLinkToType(inheritedTypes[i]));
                if (i > 0)
                    sb.Append(" » ");
            }
            sb.AppendLine().AppendLine();

            // Derived types

            var derivedTypes = engineAssembly.GetTypes()
                .Where(t => t.Namespace == elementsNamespace && t.BaseType == type)
                .ToList();

            if (derivedTypes.Any())
            {
                sb.AppendLine("### Derived types").AppendLine();

                foreach (var derivedType in derivedTypes)
                {
                    sb.AppendLine($"* {MakeLinkToType(derivedType.ToReadableName())}");
                }

                sb.AppendLine();
            }

            // Description

            var documentation = type.GetDocumentation();

            if (documentation != null)
            {
                var summary = documentation["member"]?["summary"];
                if (summary != null)
                {
                    sb.AppendLine("### Description").AppendLine();
                    sb.AppendLine(summary.InnerText.Trim());
                }

                var remarks = documentation["member"]?["remarks"];
                if (remarks != null)
                {
                    sb.AppendLine("**Remarks**").AppendLine();
                    sb.AppendLine(remarks.InnerText.Trim());
                }
            }

            // Properties

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(p => !p.CustomAttributes.Any(a => a.AttributeType == typeof(DoNotDocumentAttribute)))
                .ToList();

            if (properties.Any())
            {
                sb.AppendLine("### Properties");

                foreach (var property in properties)
                {
                    sb.AppendLine($"* `{property.PropertyType.ToReadableName()}` **`{type.ToReadableName()}.{property.Name}`**");

                    var propDocumentation = property.GetDocumentation();
                    if (propDocumentation != null)
                    {
                        var summary = propDocumentation["member"]?["summary"];

                        if (summary != null)
                        {
                            foreach (var line in summary.InnerText.Trim().Split('\n'))
                            {
                                sb.AppendLine($"    > {line}");
                            }
                            sb.AppendLine();
                        }
                    }

                    sb.AppendLine();
                }
            }

            sb.AppendLine();
        }

        private static StringBuilder BuildDocumentation(Assembly engineAssembly, string elementsNamespace)
        {
            StringBuilder sb = new StringBuilder();

            var elementTypes = engineAssembly.GetTypes()
                .Where(t => t.IsPublic && t.Namespace == elementsNamespace && !t.CustomAttributes.Any(a => a.AttributeType == typeof(DoNotDocumentAttribute)))
                .ToList();

            for (int i = 0; i < elementTypes.Count; i++)
            {
                var type = elementTypes[i];

                if (i > 0)
                    sb.AppendLine("<hr />");

                BuildTypeDocumentation(type, engineAssembly, elementsNamespace, sb);
            }

            return sb;
        }

        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: ");
                Console.WriteLine("");
                Console.WriteLine("Animator.Documentation <path to Animator.Engine.xml> <outfile.markdown>");

                Console.WriteLine("Run with full path to Animator.Engine.xml documentation file.");
                return;
            }

            Documentation.LoadXmlDocumentation(File.ReadAllText(args[0]));


            // Find assembly with elements and its current namespace

            var elementsNamespace = typeof(Animation).Namespace;
            var engineAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .Single(a => a.GetName().Name == "Animator.Engine");

            // Build documentation

            var sb = BuildDocumentation(engineAssembly, elementsNamespace);

            // Save documentation

            File.WriteAllText(args[1], sb.ToString());
        }
    }
}
