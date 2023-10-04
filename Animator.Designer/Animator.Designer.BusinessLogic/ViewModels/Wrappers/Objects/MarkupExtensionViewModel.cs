using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Properties;
using Animator.Designer.BusinessLogic.ViewModels.Wrappers.Values;
using Animator.Engine.Base.Extensions;
using Animator.Engine.Base.Persistence;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;

namespace Animator.Designer.BusinessLogic.ViewModels.Wrappers.Objects
{
    public class MarkupExtensionViewModel : ObjectViewModel
    {
        private readonly List<ObjectViewModel> children = new();
        private readonly List<ClearableStringPropertyViewModel> properties = new();
        private readonly ClearableStringPropertyViewModel defaultProperty;
        private readonly Type type;

        private void DoDelete()
        {
            Parent.RequestDelete(this);
        }

        public MarkupExtensionViewModel(WrapperContext context, string ns, string name, Type type)
            : base(context)
        {
            this.type = type;
            this.Name = name;
            this.Namespace = ns;

            var defaultPropertyAttr = type.GetCustomAttribute<DefaultPropertyAttribute>();

            foreach (var propInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .OrderBy(prop => prop.Name))
            {
                var property = new ClearableStringPropertyViewModel(this, context, context.DefaultNamespace, propInfo.Name);
                properties.Add(property);

                if (property.Name == defaultPropertyAttr.PropertyName)
                    defaultProperty = property;
            }

            DeleteCommand = new AppCommand(obj => DoDelete());

            Icon = "MarkupExtension16.png";
        }

        public override XmlElement Serialize(XmlDocument document)
        {
            throw new NotSupportedException("Markup extensions can be only serialized into attributes via SerializeToString!");
        }

        public string SerializeToString()
        {
            // Utilities

            string BuildSeparator(ref bool firstProp)
            {
                if (firstProp)
                {
                    firstProp = false;
                    return " ";
                }

                return ", ";
            }

            string QuoteIfNeeded(string str)
            {
                if (str.Contains('\'') || str.Contains(' ') || str.Contains(','))
                    str = str.Quote();
                return str;
            }

            // Name and namespace (if needed)

            StringBuilder sb = new StringBuilder();

            var nsDef = context.Namespaces.First(ns => ns.NamespaceUri == Namespace);

            if (string.IsNullOrEmpty(nsDef.Prefix))
                sb.Append($"{{{Name}");
            else
                sb.Append($"{{{nsDef.Prefix}:{Name}");

            bool firstProp = true;

            // Default property

            if (defaultProperty.Value is StringValueViewModel stringValue)
            {
                var str = stringValue.Value;
                str = QuoteIfNeeded(str);

                sb.Append(BuildSeparator(ref firstProp)).Append(str);
            }

            // All other properties

            foreach (var property in properties)
            {
                if (property == defaultProperty)
                    continue;

                if (property.Value is StringValueViewModel strValue)
                {
                    var str = strValue.Value;
                    str = QuoteIfNeeded(str);

                    sb.Append(BuildSeparator(ref firstProp)).Append($"{property.Name}={str}");
                }
            }

            // Closing brace

            sb.Append("}");

            // Return

            return sb.ToString();
        }

        public override IEnumerable<PropertyViewModel> Properties => properties;

        public override IEnumerable<ObjectViewModel> DisplayChildren => children;

        public ICommand DeleteCommand { get; }
    }
}
