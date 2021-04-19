﻿using Animator.Engine.Base;
using Animator.Engine.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Tests.TestClasses
{
    [ContentProperty(nameof(SimpleCompositeClass.NestedObject))]
    public class SimpleCompositeClass : ManagedObject
    {
        #region NestedObject managed property

        public SimplePropertyClass NestedObject
        {
            get => (SimplePropertyClass)GetValue(NestedObjectProperty);
            set => SetValue(NestedObjectProperty, value);
        }

        public static readonly ManagedProperty NestedObjectProperty = ManagedProperty.Register(typeof(SimpleCompositeClass),
            nameof(NestedObject),
            typeof(SimplePropertyClass));

        #endregion
    }
}
