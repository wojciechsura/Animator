using Animator.Engine.Animation;
using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public abstract class PropertyAnimator : ManagedObject
    {
        protected (ManagedObject, ManagedProperty) FindProperty(NameRegistry names, string targetName, string path)
        {
            // Find uniquely named component
            if (!names.TryGetValue(targetName, out List<BaseElement> elements))
                return (null, null);
            
            ManagedObject element = elements.SingleOrDefault();
            if (element == null)
                return (null, null);

            // Travel through properties in path (so that A.B.C property chaining is possible)
            var props = path.Split('.');
            if (props.Length == 0)
                return (null, null);

            // Get access to first property
            var property = element.GetProperty(props[0]);
            if (property == null)
                return (null, null);

            // Process next properties
            for (int i = 1; i < props.Length; i++)
            {
                // Value of the property must be a ManagedObject
                var value = element.GetValue(property);
                if (value is not ManagedObject)
                    return (null, null);

                element = value as ManagedObject;
                property = element.GetProperty(props[i]);
                if (property == null)
                    return (null, null);
            }

            return (element, property);
        }

        #region TargetName managed property

        public string TargetName
        {
            get => (string)GetValue(TargetNameProperty);
            set => SetValue(TargetNameProperty, value);
        }

        public static readonly ManagedProperty TargetNameProperty = ManagedProperty.Register(typeof(PropertyAnimator),
            nameof(TargetName),
            typeof(string),
            new ManagedSimplePropertyMetadata(null) { NotAnimatable = true });

        #endregion

        #region Path managed property

        public string Path
        {
            get => (string)GetValue(PathProperty);
            set => SetValue(PathProperty, value);
        }

        public static readonly ManagedProperty PathProperty = ManagedProperty.Register(typeof(PropertyAnimator),
            nameof(Path),
            typeof(string),
            new ManagedSimplePropertyMetadata(null) { NotAnimatable = true });

        #endregion
    }
}
