using Animator.Engine.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base.Tests.TestClasses
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
            new ManagedSimplePropertyMetadata { DefaultValue = 0 });

        #endregion

        #region IntCollection managed collection

        public ManagedCollection<int> IntCollection
        {
            get => (ManagedCollection<int>)GetValue(IntCollectionProperty);
        }

        public static readonly ManagedProperty IntCollectionProperty = ManagedProperty.RegisterCollection(typeof(IntDataClass),
            nameof(IntCollection),
            typeof(ManagedCollection<int>),
            new ManagedCollectionMetadata { CollectionChangedHandler = HandleCollectionChanged });

        private static void HandleCollectionChanged(ManagedObject sender, ManagedCollectionChangedEventArgs args)
        {
            (sender as IntDataClass).CollectionChanged?.Invoke(sender, args);
        }


        #endregion

        public event ManagedCollectionChangedDelegate CollectionChanged;
    }
}
