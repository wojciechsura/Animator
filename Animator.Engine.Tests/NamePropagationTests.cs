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

            Assert.ThrowsException<AnimationException>(() => scene.FindElement("Rectangle"));
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

            Assert.ThrowsException<AnimationException>(() => scene.FindElement("Rectangle.MyPen"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AttemptToSetNameTwiceTest()
        {
            // Arrange

            Rectangle rectangle = new Rectangle();

            // Act

            rectangle.Name = "Test1";
            rectangle.Name = "Test2";
        }
    }
}
