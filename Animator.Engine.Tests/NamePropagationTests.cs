using Animator.Engine.Elements;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            Assert.AreEqual(rectangle, scene.FindSingleByName("Rectangle"));
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

            Assert.AreEqual(rectangle, scene.FindSingleByName("Rectangle"));
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

            Assert.AreEqual(null, scene.FindSingleByName("Rectangle"));

        }

        [TestMethod]
        public void NameSetAfterAssigningPropertyTest()
        {
            // Arrange

            Scene scene = new Scene();
            
            Rectangle rectangle = new Rectangle();
            scene.Items.Add(rectangle);

            Pen pen = new Pen { Name = "Pen" };

            // Act

            rectangle.Pen = pen;

            // Assert

            Assert.AreEqual(pen, scene.FindSingleByName("Pen"));
        }

        [TestMethod]
        public void NameClearedAfterClearingPropertyTest()
        {
            // Arrange

            Scene scene = new Scene();

            Rectangle rectangle = new Rectangle();
            scene.Items.Add(rectangle);

            Pen pen = new Pen { Name = "Pen" };
            rectangle.Pen = pen;

            // Act

            rectangle.Pen = null;

            // Assert

            Assert.AreEqual(null, scene.FindSingleByName("Pen"));
        }
    }
}
