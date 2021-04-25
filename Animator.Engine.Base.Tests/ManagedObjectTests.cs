using Animator.Engine.Base;
using Animator.Engine.Base.Tests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Animator.Engine.Base.Tests
{
    [TestClass]
    public class ManagedObjectTests
    {
        [TestMethod]
        public void ManagedPropertyValueSetTest()
        {
            // Arrange

            var cls = new SimplePropertyClass();

            // Act

            cls.IntValue = 44;

            // Assert

            Assert.AreEqual(44, cls.IntValue);
        }

        [TestMethod]
        public void ManagedPropertyInvalidTypeTest()
        {
            // Arrange

            var cls = new SimplePropertyClass();

            // Act

            Assert.ThrowsException<ArgumentException>(() => cls.SetValue(SimplePropertyClass.IntValueProperty, "Ala ma kota"));
        }

        [TestMethod]
        public void ManagedPropertyValueDoubleSetTest()
        {
            // Arrange

            var cls = new SimplePropertyClass();

            // Act

            cls.IntValue = 44;
            cls.IntValue = 99;

            // Assert

            Assert.AreEqual(99, cls.IntValue);
        }

        [TestMethod]
        public void ManagedPropertyEffectiveValueNotificationTest()
        {
            // Arrange

            List<(object oldValue, object newValue)> registeredChanges = new();

            var cls = new SimplePropertyClass();
            cls.IntValueChanged += (s, e) =>
            {
                registeredChanges.Add((e.OldValue, e.NewValue));
            };

            // Act

            cls.IntValue = 44;
            cls.IntValue = 99;

            // Assert

            Assert.AreEqual(44, registeredChanges.Last().oldValue);
            Assert.AreEqual(99, registeredChanges.Last().newValue);
        }

        [TestMethod]
        public void ManagedPropertyEffectiveValueNotificationWithAnimationTest()
        {
            // Arrange

            List<(object oldValue, object newValue)> registeredChanges = new();

            var cls = new SimplePropertyClass();
            cls.IntValueChanged += (s, e) =>
            {
                registeredChanges.Add((e.OldValue, e.NewValue));
            };

            // Act

            cls.IntValue = 44;
            cls.SetAnimatedValue(SimplePropertyClass.IntValueProperty, 99);

            // Assert

            Assert.AreEqual(44, registeredChanges.Last().oldValue);
            Assert.AreEqual(99, registeredChanges.Last().newValue);
        }

        [TestMethod]
        public void ManagedPropertyAnimatedValueOverrideTest()
        {
            // Arrange

            var cls = new SimplePropertyClass();
            cls.IntValue = 44;

            // Act

            cls.SetAnimatedValue(SimplePropertyClass.IntValueProperty, 99);

            // Assert

            Assert.AreEqual(99, cls.IntValue);
        }

        [TestMethod]
        public void ManagedPropertyAnimatedValueResetTest()
        {
            // Arrange

            var cls = new SimplePropertyClass();
            cls.IntValue = 44;
            cls.SetAnimatedValue(SimplePropertyClass.IntValueProperty, 99);

            // Act

            cls.ResetAnimatedValue(SimplePropertyClass.IntValueProperty);

            // Assert

            Assert.AreEqual(44, cls.IntValue);
        }

        [TestMethod]
        public void ManagedPropertyCoertionNotRequiredTest()
        {
            // Arrange

            var cls = new SimpleCoercedPropertyClass();

            // Act

            cls.Max10 = 8;

            // Assert

            Assert.AreEqual(8, cls.Max10);
        }

        [TestMethod]
        public void ManagedPropertyCoertionTest()
        {
            // Arrange

            var cls = new SimpleCoercedPropertyClass();

            // Act

            cls.Max10 = 20;

            // Assert

            Assert.AreEqual(10, cls.Max10);
        }

        [TestMethod]
        public void ManagedPropertyCoertionWithAnimationTest()
        {
            // Arrange

            var cls = new SimpleCoercedPropertyClass();

            // Act

            cls.Max10 = 5;
            cls.SetAnimatedValue(SimpleCoercedPropertyClass.Max10Property, 20);

            // Assert

            Assert.AreEqual(10, cls.Max10);
        }

        [TestMethod]
        public void ManagedCollectionAddItemsTest()
        {
            // Arrange

            var cls = new SimpleCollectionPropertyClass();

            // Act

            cls.Collection.Add(44);

            // Assert

            Assert.AreEqual(44, cls.Collection[0]);
        }

        [TestMethod]
        public void RangeCoercionTest()
        {
            // Arrange

            var cls = new MinMaxClass();
            cls.Min = 10;
            cls.Max = 20;

            // Act & Assert

            Assert.AreEqual(10, cls.Min);
            Assert.AreEqual(20, cls.Max);

            cls.Max = 0;

            Assert.AreEqual(10, cls.Min);
            Assert.AreEqual(10, cls.Max);

            cls.Min = 20;

            Assert.AreEqual(20, cls.Min);
            Assert.AreEqual(20, cls.Max);
        }

        [TestMethod]
        public void CollectionChangeNotificationTestForAdd()
        {
            // Arrange

            var changes = new List<ManagedCollectionChangedEventArgs>();

            var cls = new IntDataClass();
            cls.CollectionChanged += (s, a) => changes.Add(a);

            // Act

            cls.IntCollection.Add(42);

            // Assert

            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual(CollectionChange.ItemsAdded, changes[0].Change);
            Assert.AreEqual(1, changes[0].ItemsAdded.Count);
            Assert.AreEqual(42, changes[0].ItemsAdded[0]);
        }

        [TestMethod]
        public void CollectionChangeNotificationTestForRemove()
        {
            // Arrange

            var changes = new List<ManagedCollectionChangedEventArgs>();

            var cls = new IntDataClass();
            cls.CollectionChanged += (s, a) => changes.Add(a);

            // Act

            cls.IntCollection.Add(42);
            cls.IntCollection.Remove(42);

            // Assert

            Assert.AreEqual(2, changes.Count);
            Assert.AreEqual(CollectionChange.ItemsRemoved, changes[1].Change);
            Assert.AreEqual(1, changes[1].ItemsRemoved.Count);
            Assert.AreEqual(42, changes[1].ItemsRemoved[0]);
        }

        [TestMethod]
        public void CollectionChangeNotificationTestForRemoveAt()
        {
            // Arrange

            var changes = new List<ManagedCollectionChangedEventArgs>();

            var cls = new IntDataClass();
            cls.CollectionChanged += (s, a) => changes.Add(a);

            // Act

            cls.IntCollection.Add(42);
            cls.IntCollection.RemoveAt(0);

            // Assert

            Assert.AreEqual(2, changes.Count);
            Assert.AreEqual(CollectionChange.ItemsRemoved, changes[1].Change);
            Assert.AreEqual(1, changes[1].ItemsRemoved.Count);
            Assert.AreEqual(42, changes[1].ItemsRemoved[0]);
        }

        [TestMethod]
        public void CollectionChangeNotificationTestForReplace()
        {
            // Arrange

            var changes = new List<ManagedCollectionChangedEventArgs>();

            var cls = new IntDataClass();
            cls.CollectionChanged += (s, a) => changes.Add(a);

            // Act

            cls.IntCollection.Add(42);
            cls.IntCollection[0] = 12;

            // Assert

            Assert.AreEqual(2, changes.Count);
            Assert.AreEqual(CollectionChange.ItemsReplaced, changes[1].Change);
            Assert.AreEqual(1, changes[1].ItemsRemoved.Count);
            Assert.AreEqual(42, changes[1].ItemsRemoved[0]);
            Assert.AreEqual(1, changes[1].ItemsAdded.Count);
            Assert.AreEqual(12, changes[1].ItemsAdded[0]);
        }

        [TestMethod]
        public void CollectionChangeNotificationTestForClear()
        {
            // Arrange

            var changes = new List<ManagedCollectionChangedEventArgs>();

            var cls = new IntDataClass();
            cls.CollectionChanged += (s, a) => changes.Add(a);

            // Act

            cls.IntCollection.Add(42);
            cls.IntCollection.Add(12);
            cls.IntCollection.Add(12000);
            cls.IntCollection.Clear(); ;

            // Assert

            Assert.AreEqual(4, changes.Count);
            Assert.AreEqual(CollectionChange.ItemsRemoved, changes[3].Change);
            Assert.AreEqual(3, changes[3].ItemsRemoved.Count);
            Assert.AreEqual(42, changes[3].ItemsRemoved[0]);
            Assert.AreEqual(12, changes[3].ItemsRemoved[1]);
            Assert.AreEqual(12000, changes[3].ItemsRemoved[2]);
        }

        [TestMethod]
        public void ReferencePropertyChangeNotificationTest()
        {
            // Arrange

            var cls = new SimpleCompositeClass();

            var value1 = new SimplePropertyClass();
            var value2 = new SimplePropertyClass();

            var changes = new List<PropertyValueChangedEventArgs>();

            cls.NestedObjectChanged += (s, a) => changes.Add(a);

            // Act

            cls.NestedObject = value1;
            cls.NestedObject = value2;

            // Assert

            Assert.AreEqual(2, changes.Count);
            Assert.AreEqual(null, changes[0].OldValue);
            Assert.AreEqual(value1, changes[0].NewValue);
            Assert.AreEqual(value1, changes[1].OldValue);
            Assert.AreEqual(value2, changes[1].NewValue);
        }

        // TODO add tests for parents

        [TestMethod]
        public void ChildInheritsDefaultValueTest()
        {
            // Arrange

            var child = new InheritanceChildClass();

            // Assert

            Assert.AreEqual(20, child.Value);
        }

        [TestMethod]
        public void ChildInheritsValueAfterAssigningToParentTest()
        {
            // Arrange

            var parent = new InheritanceParentClass();
            var child = new InheritanceChildClass();

            // Act & assert

            Assert.AreEqual(20, child.Value);
            parent.Child = child;
            Assert.AreEqual(10, child.Value);
        }

        [TestMethod]
        public void ChildInheritsValueAfterAddingToParentsCollectionTest()
        {
            // Arrange

            var parent = new InheritanceParentClass();
            var child = new InheritanceChildClass();

            // Act & assert

            Assert.AreEqual(20, child.Value);
            parent.Children.Add(child);
            Assert.AreEqual(10, child.Value);
        }

        [TestMethod]
        public void ChildsStopsInheritingValueAfterRemovingFromParentTest()
        {
            // Arrange

            var parent = new InheritanceParentClass();
            var child = new InheritanceChildClass();

            // Act & assert

            parent.Child = child;
            Assert.AreEqual(10, child.Value);

            parent.Child = null;
            Assert.AreEqual(20, child.Value);
        }

        [TestMethod]
        public void ChildStopsInheritingValueAfterRemovingFromParentsCollectionTest()
        {
            // Arrange

            var parent = new InheritanceParentClass();
            var child = new InheritanceChildClass();

            // Act & assert

            parent.Children.Add(child);
            Assert.AreEqual(10, child.Value);
            parent.Children.Remove(child);
            Assert.AreEqual(20, child.Value);
        }

        [TestMethod]
        public void InheritedValueChangesOnChildWhenOnParentTest1()
        {
            // Arrange

            var parent = new InheritanceParentClass();
            var child = new InheritanceChildClass();
            parent.Child = child;

            // Act & assert

            Assert.AreEqual(10, child.Value);
            parent.Value = 12;
            Assert.AreEqual(12, child.Value);
        }

        [TestMethod]
        public void InheritedValueChangesOnChildWhenOnParentTest2()
        {
            // Arrange

            var parent = new InheritanceParentClass();
            var child = new InheritanceChildClass();
            parent.Children.Add(child);

            // Act & assert

            Assert.AreEqual(10, child.Value);
            parent.Value = 12;
            Assert.AreEqual(12, child.Value);
        }

        [TestMethod]
        public void ExplicitValueOverridesInheritedTest1()
        {
            // Arrange

            var parent = new InheritanceParentClass();
            var child = new InheritanceChildClass();
            parent.Child = child;

            // Act & assert

            Assert.AreEqual(10, child.Value);
            child.Value = 12;
            Assert.AreEqual(12, child.Value);
        }

        [TestMethod]
        public void ExplicitValueOverridesInheritedTest2()
        {
            // Arrange

            var parent = new InheritanceParentClass();
            var child = new InheritanceChildClass();
            parent.Children.Add(child);

            // Act & assert

            Assert.AreEqual(10, child.Value);
            child.Value = 12;
            Assert.AreEqual(12, child.Value);
        }

        [TestMethod]
        public void AnimatedValueOverridesInheritedValueTest()
        {
            // Arrange

            var parent = new InheritanceParentClass();
            var child = new InheritanceChildClass();
            parent.Child = child;

            // Act & assert

            Assert.AreEqual(10, child.Value);
            child.SetAnimatedValue(InheritanceChildClass.ValueProperty, 42);
            Assert.AreEqual(42, child.Value);
        }

        [TestMethod]
        public void InheritedValueReturnsAfterClearingAnimatedValueTest()
        {
            // Arrange

            var parent = new InheritanceParentClass();
            var child = new InheritanceChildClass();
            parent.Child = child;

            // Act & assert

            Assert.AreEqual(10, child.Value);
            child.SetAnimatedValue(InheritanceChildClass.ValueProperty, 42);
            child.ResetAnimatedValue(InheritanceChildClass.ValueProperty);
            Assert.AreEqual(10, child.Value);
        }
    }
}
