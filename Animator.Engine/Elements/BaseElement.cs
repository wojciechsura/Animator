using Animator.Engine.Animation;
using Animator.Engine.Base;
using Animator.Engine.Exceptions;
using Animator.Engine.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Base class for all elements used to describe an
    /// animation.
    /// </summary>
    public abstract class BaseElement : ManagedObject
    {
        // Protected fields ---------------------------------------------------

        protected readonly Dictionary<string, List<BaseElement>> names = new();

        // Private methods ----------------------------------------------------

        private void RegisterName(string name, BaseElement baseElement)
        {
            if (!names.TryGetValue(name, out List<BaseElement> list))
            {
                list = new List<BaseElement>();
                names[name] = list;
            }

            list.Add(baseElement);
        }

        private void UnregisterName(string name, BaseElement baseElement)
        {
            if (names.TryGetValue(name, out List<BaseElement> list))
            {
                list.Remove(baseElement);
                if (!list.Any())
                    names.Remove(name);
            }
        }

        private BaseElement FindElement(string[] path)
        {
            BaseElement current = this;

            for (int i = 0; i < path.Length; i++)
            {
                var property = current.GetProperty(path[i]);

                current.names.TryGetValue(path[i], out List<BaseElement> children);

                // Erroneus situation
                if (property != null && (children != null))
                    throw new AnimationException($"{path[i]} matches both property and child!");
                else if (property == null && children == null)
                    throw new AnimationException($"{path[i]} doesn't match any property or child!");
                else if (property != null)
                {
                    object value = current.GetValue(property);
                    if (value == null)
                        throw new AnimationException($"{path[i]} returns null element!");

                    if (value is not BaseElement baseElement)
                        throw new AnimationException($"Property {path[i]} yields object of type {value.GetType().Name}, which does not derive from BaseElement!");

                    current = baseElement;
                }
                else if (children != null)
                {
                    if (children.Count > 1)
                        throw new AnimationException($"{path[i]} yields more than one child element. Name is not unique.");

                    if (children.Single() is not BaseElement baseElement)
                        throw new AnimationException($"Child {path[i]} yields object of type {children.Single().GetType().Name}, which does not derive from BaseElement!");

                    current = baseElement;
                }
                else
                    throw new InvalidOperationException("Element seeking algorithm malfunction!");
            }

            return current;
        }

        // Protected methods --------------------------------------------------

        protected override void OnParentDetaching()
        {
            if (Parent is BaseElement baseElement)
            {
                baseElement.UnregisterName(Name, this);
            }

            base.OnParentDetaching();
        }

        protected override void OnParentAttached()
        {
            base.OnParentAttached();

            if (Parent is BaseElement baseElement && Name != null)
            {
                baseElement.RegisterName(Name, this);
            }
        }

        // Public methods -----------------------------------------------------

        public BaseElement FindElement(string elementRef)
        {
            var path = elementRef.Split('.');

            BaseElement finalElement;

            try
            {
                finalElement = FindElement(path);
            }
            catch (Exception e)
            {
                throw new AnimationException($"Failed to find element by reference {elementRef}!", e);
            }

            return finalElement;
        }

        public (BaseElement, ManagedProperty) FindProperty(string propertyRef)
        {
            var path = propertyRef.Split('.');

            BaseElement finalElement;

            try
            {                
                finalElement = FindElement(path[..^1]);
            }
            catch (Exception e)
            {
                throw new AnimationException($"Failed to process property reference { propertyRef }!", e);
            }

            var property = finalElement.GetProperty(path.Last());
            if (property == null)
                throw new AnimationException($"Failed to process property reference {propertyRef}: object {finalElement.GetType().Name} does not have property {path.Last()}!");

            return (finalElement, property);
        }

        public void ApplyAnimation(float timeMs)
        {
            ProcessElementsRecursive<BaseElement>(baseElement =>
            {
                foreach (var animator in baseElement.Animators)
                {
                    animator.ApplyAnimation(timeMs);
                }
            });
        }

        // Public properties --------------------------------------------------

        #region Name managed property

        /// <summary>
        /// Defines name of this object, which may be then used to
        /// reference it from various objects via their TargetName
        /// properties. This value should be unique among the whole
        /// scene, though this is not enforced. However, elements
        /// are reachable through TargetName properties only if
        /// their names are unique.
        /// </summary>
        public string Name
        {
            get => (string)GetValue(NameProperty);
            set => SetValue(NameProperty, value);
        }

        public static readonly ManagedProperty NameProperty = ManagedProperty.RegisterReference(typeof(BaseElement),
            nameof(Name),
            typeof(string),
            new ManagedReferencePropertyMetadata { ValueIsPermanent = true, ValueChangedHandler = HandleNameChanged });

        private static void HandleNameChanged(ManagedObject sender, PropertyValueChangedEventArgs args)
        {
            // Value is permanent, so the change may be done only once
            if (sender is BaseElement baseElement && baseElement.Scene != null)
                baseElement.Scene.RegisterName((string)args.NewValue, baseElement);
        }

        #endregion

        #region Animators managed collection

        /// <summary>
        /// Contains list of all animators, which animate properties
        /// of elements placed inside this element.
        /// </summary>
        public ManagedCollection<BaseAnimator> Animators
        {
            get => (ManagedCollection<BaseAnimator>)GetValue(AnimatorsProperty);
        }

        public static readonly ManagedProperty AnimatorsProperty = ManagedProperty.RegisterCollection(typeof(BaseElement),
            nameof(Animators),
            typeof(ManagedCollection<BaseAnimator>));

        #endregion

        [DoNotDocument]
        public Scene Scene
        {
            get
            {
                if (Parent is Scene scene)
                    return scene;
                else if (Parent is BaseElement baseElement)
                    return baseElement.Scene;
                else
                    return null;
            }
        }
    }
}
