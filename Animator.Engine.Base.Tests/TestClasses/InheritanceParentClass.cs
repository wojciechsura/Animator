using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base.Tests.TestClasses
{
    public class InheritanceParentClass : ManagedObject
    {
        #region Value managed property

        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly ManagedProperty ValueProperty = ManagedProperty.Register(typeof(InheritanceParentClass),
            nameof(Value),
            typeof(int),
            new ManagedSimplePropertyMetadata { DefaultValue = 10 });

        #endregion


        #region Value2 managed property

        public int Value2
        {
            get => (int)GetValue(Value2Property);
            set => SetValue(Value2Property, value);
        }

        public static readonly ManagedProperty Value2Property = ManagedProperty.Register(typeof(InheritanceParentClass),
            nameof(Value2),
            typeof(int),
            new ManagedSimplePropertyMetadata { DefaultValue = 10 });

        #endregion


        #region Value3 managed property

        public int Value3
        {
            get => (int)GetValue(Value3Property);
            set => SetValue(Value3Property, value);
        }

        public static readonly ManagedProperty Value3Property = ManagedProperty.Register(typeof(InheritanceParentClass),
            nameof(Value3),
            typeof(int),
            new ManagedSimplePropertyMetadata { DefaultValue = 10, Inheritable = false });

        #endregion

        #region Child managed property

        public InheritanceChildClass Child
        {
            get => (InheritanceChildClass)GetValue(ChildProperty);
            set => SetValue(ChildProperty, value);
        }

        public static readonly ManagedProperty ChildProperty = ManagedProperty.RegisterReference(typeof(InheritanceParentClass),
            nameof(Child),
            typeof(InheritanceChildClass),
            new ManagedReferencePropertyMetadata());

        #endregion

        #region Children managed collection

        public ManagedCollection<InheritanceChildClass> Children
        {
            get => (ManagedCollection<InheritanceChildClass>)GetValue(ChildrenProperty);
        }

        public static readonly ManagedProperty ChildrenProperty = ManagedProperty.RegisterCollection(typeof(InheritanceParentClass),
            nameof(Children),
            typeof(ManagedCollection<InheritanceChildClass>));

        #endregion
    }
}
