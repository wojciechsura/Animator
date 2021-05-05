using Animator.Engine.Elements;
using Animator.Engine.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Tests
{
    [TestClass]
    public class ReferenceSearchingTests
    {
        [TestMethod]
        public void ValidElementSearch()
        {
            // Arrange
            var scene = new Scene();
            var rectangle = new Rectangle();
            rectangle.Name = "Rectangle";
            scene.Items.Add(rectangle);

            // Act
            var element = scene.FindElement("Rectangle");

            // Assert
            Assert.AreEqual(rectangle, element);
        }

        [TestMethod]
        public void ValidPropertySearch()
        {
            // Arrange
            var scene = new Scene();
            var rectangle = new Rectangle();
            rectangle.Name = "Rectangle";
            scene.Items.Add(rectangle);

            // Act
            (var element, var property) = scene.FindProperty("Rectangle.Pen");

            // Assert
            Assert.AreEqual(rectangle, element);
            Assert.AreEqual(Rectangle.PenProperty, property);
        }

        [TestMethod]
        public void ValidPropertySearchOnSelf()
        {
            // Arrange
            var scene = new Scene();

            // Act
            (var element, var property) = scene.FindProperty("Duration");

            // Assert
            Assert.AreEqual(scene, element);
            Assert.AreEqual(Scene.DurationProperty, property);
        }

        [TestMethod]
        public void ChildNameEqualsToPropertyName()
        {
            // Arrange
            var scene = new Scene();
            var rectangle = new Rectangle();
            rectangle.Name = "Rectangle";
            scene.Items.Add(rectangle);
            Pen pen = new Pen();
            pen.Name = "Pen";
            rectangle.Pen = pen;

            // Act & Assert
            Assert.ThrowsException<AnimationException>(() =>
            {
                var element = scene.FindElement("Rectangle.Pen");
            });            
        }

        [TestMethod]
        public void ChildDoesNotExist()
        {
            // Arrange
            var scene = new Scene();

            // Act & Assert
            Assert.ThrowsException<AnimationException>(() =>
            {
                var element = scene.FindElement("Rectangle");
            });
        }

        [TestMethod]
        public void PropertyDoesNotExist()
        {
            // Arrange
            var scene = new Scene();
            var rectangle = new Rectangle();
            rectangle.Name = "Rectangle";
            scene.Items.Add(rectangle);

            // Act & Assert
            Assert.ThrowsException<AnimationException>(() =>
            {
                var element = scene.FindProperty("Rectangle._");
            });
        }

        [TestMethod]
        public void ElementIsNotBaseElement()
        {
            // Arrange
            var scene = new Scene();
            var rectangle = new Rectangle();
            rectangle.Name = "Rectangle";
            scene.Items.Add(rectangle);

            // Act & Assert
            Assert.ThrowsException<AnimationException>(() =>
            {
                var element = scene.FindElement("Duration");
            });
        }
    }
}
