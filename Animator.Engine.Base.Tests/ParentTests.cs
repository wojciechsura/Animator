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
    public class ParentTests
    {
        [TestMethod]
        public void SetParentTest()
        {
            // Arrange

            var parent = new SimpleCompositeClass();
            var child = new SimplePropertyClass();

            // Act

            parent.NestedObject = child;

            // Assert
            Assert.AreEqual(parent, child.ParentInfo.Parent);
            Assert.AreEqual(SimpleCompositeClass.NestedObjectProperty, child.ParentInfo.Property);
        }

        [TestMethod]
        public void ClearParentTest()
        {
            // Arrange

            var parent = new SimpleCompositeClass();
            var child = new SimplePropertyClass();

            // Act

            parent.NestedObject = child;
            parent.NestedObject = null;

            // Assert
            Assert.AreEqual(null, child.ParentInfo);
        }

        [TestMethod]
        public void CollectionAddSetParentTest()
        {
            // Arrange

            var parent = new SimpleCollectionClass();
            var child = new SimplePropertyClass();

            // Act

            parent.Items.Add(child);

            // Assert

            Assert.AreEqual(parent, child.ParentInfo.Parent);
            Assert.AreEqual(SimpleCollectionClass.ItemsProperty, child.ParentInfo.Property);
        }

        [TestMethod]
        public void CollectionRemoveClearParentTest()
        {
            // Arrange

            var parent = new SimpleCollectionClass();
            var child = new SimplePropertyClass();

            // Act

            parent.Items.Add(child);
            parent.Items.Remove(child);

            // Assert

            Assert.AreEqual(null, child.ParentInfo);
        }

        [TestMethod]
        public void SwitchParentTest1()
        {
            // Arrange

            var parent1 = new SimpleCompositeClass();
            var parent2 = new SimpleCompositeClass();
            var child = new SimplePropertyClass();

            // Act

            parent1.NestedObject = child;
            parent1.NestedObject = null;
            parent2.NestedObject = child;

            // Assert
            Assert.AreEqual(parent2, child.ParentInfo.Parent);
            Assert.AreEqual(SimpleCompositeClass.NestedObjectProperty, child.ParentInfo.Property);
        }

        [TestMethod]
        public void SwitchParentTest2()
        {
            // Arrange

            var parent1 = new SimpleCompositeClass();
            var parent2 = new SimpleCollectionClass();
            var child = new SimplePropertyClass();

            // Act

            parent1.NestedObject = child;
            parent1.NestedObject = null;
            parent2.Items.Add(child);

            // Assert
            Assert.AreEqual(parent2, child.ParentInfo.Parent);
            Assert.AreEqual(SimpleCollectionClass.ItemsProperty, child.ParentInfo.Property);
        }
    }
}
