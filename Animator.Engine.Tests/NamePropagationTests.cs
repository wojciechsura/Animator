using Animator.Engine.Elements;
using Animator.Engine.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Animator.Engine.Tests
{
    [TestClass]
    public class NamePropagationTests
    {
        [TestMethod]
        public void NamedItemAddTest()
        {
            // Arrange

            Scene scene = new Scene();

            Rectangle rectangle = new Rectangle();
            rectangle.Name = "Rectangle";

            // Act

            scene.Items.Add(rectangle);

            // Assert

            Assert.AreEqual(rectangle, scene.FindElement("Rectangle"));
        }

        [TestMethod]
        public void NameSetAfterAddingTest()
        {
            // Arrange

            Scene scene = new Scene();

            Rectangle rectangle = new Rectangle();
            scene.Items.Add(rectangle);

            // Act

            rectangle.Name = "Rectangle";

            // Assert

            Assert.AreEqual(rectangle, scene.FindElement("Rectangle"));
        }

        [TestMethod]
        public void NameClearedAfterRemovingTest()
        {
            // Arrange

            Scene scene = new Scene();

            Rectangle rectangle = new Rectangle();
            rectangle.Name = "Rectangle";
            scene.Items.Add(rectangle);

            // Act

            scene.Items.Remove(rectangle);

            // Assert

            bool exceptionThrown = false;
            try
            {
                scene.FindElement("Rectangle");
            }
            catch (AnimationException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Expected AnimationException was not thrown");
        }

        [TestMethod]
        public void NameSetAfterAssigningPropertyTest()
        {
            // Arrange

            Scene scene = new Scene();
            
            Rectangle rectangle = new Rectangle();
            rectangle.Name = "Rectangle";
            scene.Items.Add(rectangle);

            Pen pen = new Pen { Name = "MyPen" };

            // Act

            rectangle.Pen = pen;

            // Assert

            Assert.AreEqual(pen, scene.FindElement("Rectangle.MyPen"));
        }

        [TestMethod]
        public void NameClearedAfterClearingPropertyTest()
        {
            // Arrange

            Scene scene = new Scene();

            Rectangle rectangle = new Rectangle();
            scene.Items.Add(rectangle);

            Pen pen = new Pen { Name = "MyPen" };
            rectangle.Pen = pen;

            // Act

            rectangle.Pen = null;

            // Assert

            bool exceptionThrown = false;
            try
            {
                scene.FindElement("Rectangle.MyPen");
            }
            catch (AnimationException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Expected AnimationException was not thrown");
        }

        [TestMethod]
        public void AttemptToSetNameTwiceTest()
        {
            // Arrange

            Rectangle rectangle = new Rectangle();

            // Act

            rectangle.Name = "Test1";

            // Assert

            bool exceptionThrown = false;
            try
            {
                rectangle.Name = "Test2";
            }
            catch (InvalidOperationException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Expected InvalidOperationException was not thrown");
        }
    }
}
