﻿using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Tests.TestClasses
{
    public class IntDataClass : ManagedObject
    {
        #region IntValue managed property

        public int IntValue
        {
            get => (int)GetValue(IntValueProperty);
            set => SetValue(IntValueProperty, value);
        }

        public static readonly ManagedProperty IntValueProperty = ManagedProperty.Register(typeof(IntDataClass),
            nameof(IntValue),
            typeof(int),
            new ManagedAnimatedPropertyMetadata(0));

        #endregion


        #region IntCollection managed collection

        public List<int> IntCollection
        {
            get => (List<int>)GetValue(IntCollectionProperty);
        }

        public static readonly ManagedProperty IntCollectionProperty = ManagedProperty.RegisterCollection(typeof(IntDataClass),
            nameof(IntCollection),
            typeof(List<int>));

        #endregion
    }
}
