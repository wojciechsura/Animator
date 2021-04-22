using Animator.Engine.Base;
using Animator.Engine.Tests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Animator.Engine.Tests
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
    }
}
