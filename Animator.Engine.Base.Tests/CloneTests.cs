using Animator.Engine.Base.Tests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base.Tests
{
    [TestClass]
    public class CloneTests
    {
        [TestMethod]
        public void SimplePropertyCloneTest()
        {
            // Arrange

            var cls = new SimplePropertyClass();
            cls.IntValue = 1234;

            // Act

            var clone = (SimplePropertyClass)cls.Clone();

            // Assert

            Assert.AreEqual(cls.IntValue, clone.IntValue);
        }

        [TestMethod]
        public void ReferencePropertyCloneTest()
        {
            // Arrange

            var cls = new SimpleCompositeClass();
            cls.NestedObject = new SimplePropertyClass();
            cls.NestedObject.IntValue = 1234;

            // Act

            var clone = (SimpleCompositeClass)cls.Clone();

            // Assert

            Assert.AreNotEqual(cls.NestedObject, clone.NestedObject);
            Assert.AreEqual(cls.NestedObject.IntValue, clone.NestedObject.IntValue);
        }

        [TestMethod]
        public void CollectionPropertyCloneTest()
        {
            // Arrange

            var cls = new SimpleCollectionClass();
            var child = new SimplePropertyClass();
            child.IntValue = 1234;
            cls.Items.Add(child);

            child = new SimplePropertyClass();
            child.IntValue = 2345;
            cls.Items.Add(child);

            // Act

            var clone = (SimpleCollectionClass)cls.Clone();

            // Assert

            Assert.AreEqual(cls.Items.Count, clone.Items.Count);
            Assert.AreNotEqual(cls.Items[0], clone.Items[0]);

            Assert.AreEqual(cls.Items[0].IntValue, clone.Items[0].IntValue);
            Assert.AreEqual(cls.Items[1].IntValue, clone.Items[1].IntValue);
        }
    }
}
