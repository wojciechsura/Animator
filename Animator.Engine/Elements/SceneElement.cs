using Animator.Engine.Animation;
using Animator.Engine.Base;
using Animator.Engine.Exceptions;
using Animator.Engine.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    /// <summary>
    /// Base class for all elements used to describe an
    /// animation.
    /// </summary>
    public abstract class SceneElement : Element
    {
        // Private fields -----------------------------------------------------

        private static readonly string nameRegexString = "[a-zA-Z_][a-zA-Z0-9_]*";
        private static readonly Regex nameRegex = new Regex($"^{nameRegexString}$");
        private static readonly Regex collectionAccessRegex = new Regex($@"^({nameRegexString})\[({nameRegexString})\]$");

        // Protected fields ---------------------------------------------------

        protected readonly Dictionary<string, List<SceneElement>> names = new();

        // Private methods ----------------------------------------------------

        private void RegisterName(string name, SceneElement baseElement)
        {
            if (name == null)
                return;

            if (!names.TryGetValue(name, out List<SceneElement> list))
            {
                list = new List<SceneElement>();
                names[name] = list;
            }

            list.Add(baseElement);
        }

        private void UnregisterName(string name, SceneElement baseElement)
        {
            if (name == null)
                return;

            if (names.TryGetValue(name, out List<SceneElement> list))
            {
                list.Remove(baseElement);
                if (!list.Any())
                    names.Remove(name);
            }
        }

        private SceneElement FindElement(string[] path)
        {
            SceneElement current = this;

            int i = 0;
            while (i < path.Length)
            {
                var collectionAccess = collectionAccessRegex.Match(path[i]);
                if (collectionAccess.Success)
                {
                    // Element looks like id1[id2] - this is a collection item access

                    var property = current.GetProperty(collectionAccess.Groups[1].Value);
                    if (property == null)
                        throw new AnimationException($"{path[i]} is invalid, because {current.GetType().Name} does not contain property {collectionAccess.Groups[1].Value}!", GetPath());
                    if (property is not ManagedCollectionProperty)
                        throw new AnimationException($"{path[i]} is invalid, because {collectionAccess.Groups[1].Value} is not a collection property!", GetPath());

                    var collection = (ManagedCollection)current.GetValue(property);

                    var items = collection.OfType<SceneElement>().Where(e => e.Name == collectionAccess.Groups[2].Value).ToList();

                    if (items.Count == 0)
                        throw new AnimationException($"{path[i]} yielded no elements. There is no element with name {collectionAccess.Groups[2].Value} in the collection.", GetPath());
                    if (items.Count > 1)
                        throw new AnimationException($"{path[i]} yielded no elements. There are multiple elements in the collection matching name {collectionAccess.Groups[2].Value} in the collection.", GetPath());

                    current = items.Single();
                }
                else
                {
                    // Element looks like id1 - this is either property or child item access

                    var property = current.GetProperty(path[i]);

                    current.names.TryGetValue(path[i], out List<SceneElement> children);

                    // Erroneus situation
                    if (property != null && (children != null))
                        throw new AnimationException($"{path[i]} matches both property and child!", GetPath());
                    else if (property == null && children == null)
                        throw new AnimationException($"{path[i]} doesn't match any property or child!", GetPath());
                    else if (property != null)
                    {
                        object value = current.GetValue(property);
                        if (value == null)
                            throw new AnimationException($"{path[i]} returns null element!", GetPath());

                        if (value is not SceneElement sceneElement)
                            throw new AnimationException($"Property {path[i]} yields object of type {value.GetType().Name}, which does not derive from BaseElement!", GetPath());

                        current = sceneElement;
                    }
                    else if (children != null)
                    {
                        if (children.Count > 1)
                            throw new AnimationException($"{path[i]} yields more than one child element. Name is not unique.", GetPath());

                        if (children.Single() is not SceneElement sceneElement)
                            throw new AnimationException($"Child {path[i]} yields object of type {children.Single().GetType().Name}, which does not derive from BaseElement!", GetPath());

                        current = sceneElement;
                    }
                    else
                        throw new InvalidOperationException("Element seeking algorithm malfunction!");
                }

                i++;
            }

            return current;
        }

        // Protected methods --------------------------------------------------

        protected override void OnParentDetaching()
        {
            if (Parent is SceneElement baseElement)
            {
                baseElement.UnregisterName(Name, this);
            }

            base.OnParentDetaching();
        }

        protected override void OnParentAttached()
        {
            base.OnParentAttached();

            if (Parent is SceneElement baseElement && Name != null)
            {
                baseElement.RegisterName(Name, this);
            }
        }

        // Public methods -----------------------------------------------------

        public SceneElement FindElement(string elementRef)
        {
            var path = elementRef.Split('.');

            SceneElement finalElement;

            try
            {
                finalElement = FindElement(path);
            }
            catch (Exception e)
            {
                throw new AnimationException($"Failed to find element by reference {elementRef}!", GetPath(), e);
            }

            return finalElement;
        }

        public (SceneElement, ManagedProperty) FindProperty(string propertyRef)
        {
            if (String.IsNullOrEmpty(propertyRef))
                throw new ArgumentException("Property reference is empty!");

            var path = propertyRef.Split('.');

            SceneElement finalElement;

            try
            {                
                finalElement = FindElement(path[..^1]);
            }
            catch (Exception e)
            {
                throw new AnimationException($"Failed to process property reference { propertyRef }!", GetPath(), e);
            }

            var property = finalElement.GetProperty(path.Last());
            if (property == null)
                throw new AnimationException($"Failed to process property reference {propertyRef}: object {finalElement.GetType().Name} does not have property {path.Last()}!", GetPath());

            return (finalElement, property);
        }

        public bool ApplyAnimation(float timeMs)
        {
            bool changed = false;

            ProcessElementsRecursive<SceneElement>(baseElement =>
            {
                foreach (var animator in baseElement.Animations)
                {
                    changed |= animator.ApplyAnimation(timeMs);
                }
            });

            return changed;
        }

        // Public properties --------------------------------------------------

        #region Name managed property

        /// <summary>
        /// Defines name of this object, which may be then used to
        /// reference it from various objects via their PropertyRef
        /// properties. This value should be unique among the whole
        /// scene, though this is not enforced. However, elements
        /// are reachable through PropertyRef properties only if
        /// their names are unique.
        /// </summary>
        public string Name
        {
            get => (string)GetValue(NameProperty);
            set => SetValue(NameProperty, value);
        }

        public static readonly ManagedProperty NameProperty = ManagedProperty.Register(typeof(SceneElement),
            nameof(Name),
            typeof(string),
            new ManagedSimplePropertyMetadata { ValueIsPermanent = true, ValueChangedHandler = HandleNameChanged, ValueValidationHandler = ValidateName });

        private static bool ValidateName(ManagedObject sender, ValueValidationEventArgs args)
        {
            string newName = (string)args.NewValue;

            return nameRegex.IsMatch(newName);
        }

        private static void HandleNameChanged(ManagedObject sender, PropertyValueChangedEventArgs args)
        {
            // Value is permanent, so the change may be done only once
            if (sender is SceneElement sceneElement && sender.Parent is SceneElement parentElement)
                parentElement.RegisterName((string)args.NewValue, sceneElement);
        }

        #endregion

        /// <summary>
        /// Defines, whether object should be always rendered. Animator application uses 
        /// a mechanism to avoid rendering frames which contains no changes from the 
        /// previous one. If object always requires rendering (because e.g. its contents 
        /// are generated on every frame), this will enforce rendering the whole frame 
        /// even if no effective value of object's properties has changed.
        /// </summary>
        [DoNotDocument]
        public virtual bool AlwaysRender { get; } = false;


        #region Animators managed collection

        /// <summary>
        /// Contains list of all animators, which animate properties
        /// of elements placed inside this element.
        /// </summary>
        public ManagedCollection<Animation> Animations
        {
            get => (ManagedCollection<Animation>)GetValue(AnimationsProperty);
        }

        public static readonly ManagedProperty AnimationsProperty = ManagedProperty.RegisterCollection(typeof(SceneElement),
            nameof(Animations),
            typeof(ManagedCollection<Animation>));

        #endregion

        #region Resources managed collection

        public ManagedCollection<Resource> Resources
        {
            get => (ManagedCollection<Resource>)GetValue(ResourcesProperty);
        }

        public static readonly ManagedProperty ResourcesProperty = ManagedProperty.RegisterCollection(typeof(SceneElement),
            nameof(Resources),
            typeof(ManagedCollection<Resource>));

        #endregion

        #region Variables managed collection

        public ManagedCollection<Variable> Variables
        {
            get => (ManagedCollection<Variable>)GetValue(VariablesProperty);
        }

        public static readonly ManagedProperty VariablesProperty = ManagedProperty.RegisterCollection(typeof(SceneElement),
            nameof(Variables),
            typeof(ManagedCollection<Variable>));

        #endregion

        [DoNotDocument]
        public Scene Scene
        {
            get
            {
                if (Parent is Scene scene)
                    return scene;
                else if (Parent is SceneElement baseElement)
                    return baseElement.Scene;
                else
                    return null;
            }
        }
    }
}
