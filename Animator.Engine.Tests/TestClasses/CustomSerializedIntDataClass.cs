﻿using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Tests.TestClasses
{
    public class CustomSerializedIntDataClass : ManagedObject
    {
        #region IntValue managed property

        public int IntValue
        {
            get => (int)GetValue(IntValueProperty);
            set => SetValue(IntValueProperty, value);
        }

        public static readonly ManagedProperty IntValueProperty = ManagedProperty.Register(typeof(CustomSerializedIntDataClass),
            nameof(IntValue),
            typeof(int),
            new ManagedSimplePropertyMetadata(0, null, null, new CustomIntSerializer()));

        #endregion

        #region IntCollection managed collection

        public List<int> IntCollection
        {
            get => (List<int>)GetValue(IntCollectionProperty);
        }

        public static readonly ManagedProperty IntCollectionProperty = ManagedProperty.RegisterCollection(typeof(CustomSerializedIntDataClass),
            nameof(IntCollection),
            typeof(List<int>), 
            new ManagedCollectionMetadata(() => new List<int>(), new CustomIntListSerializer()));

        #endregion
    }
}