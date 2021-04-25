﻿using Animator.Engine.Animation;
using Animator.Engine.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Elements
{
    public abstract class BaseElement : ManagedObject
    {
        // Private fields -----------------------------------------------------

        private Scene scene;

        // Private methods ----------------------------------------------------

        private void AddNamesRecursiveToParent()
        {
            ProcessElementsRecursive(baseElement =>
            {
                if (baseElement.IsPropertySet(NameProperty) && baseElement.Name != null)
                {
                    scene.RegisterName(baseElement.Name, baseElement);
                }
            });
        }

        private void RemoveNamesRecursiveFromParent()
        {
            ProcessElementsRecursive(baseElement =>
            {
                if (baseElement.IsPropertySet(NameProperty) && baseElement.Name != null)
                {
                    scene.UnregisterName(baseElement.Name, baseElement);
                }
            });
        }

        // Protected methods --------------------------------------------------

        protected override void OnParentDetaching()
        {
            if (Parent != null)
            {
                RemoveNamesRecursiveFromParent();
            }
        }

        protected override void OnParentAttached()
        {
            if (Parent != null)
            {
                if (Parent is Scene)
                    scene = (Scene)Parent;
                else if (Parent is BaseElement baseElement)
                    scene = baseElement.Scene;
                else
                    scene = null;

                if (scene != null)
                    AddNamesRecursiveToParent();
            }
            else
            {
                scene = null;
            }
        }

        // Public methods -----------------------------------------------------

        public void ProcessElementsRecursive(Action<BaseElement> action)
        {
            action(this);

            foreach (var prop in GetProperties(true).Where(p => IsPropertySet(p)))
            {
                if (prop is ManagedReferenceProperty refProp)
                {
                    var value = GetValue(prop);
                    if (value is BaseElement baseElement)
                        baseElement.ProcessElementsRecursive(action);
                }
                else if (prop is ManagedCollectionProperty collectionProp)
                {
                    var collection = GetValue(collectionProp) as ManagedCollection;

                    foreach (var obj in collection)
                        if (obj is BaseElement baseElement)
                            baseElement.ProcessElementsRecursive(action);

                }
            }
        }

        // Public properties --------------------------------------------------

        #region Name managed property

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

        public Scene Scene => scene;
    }
}
