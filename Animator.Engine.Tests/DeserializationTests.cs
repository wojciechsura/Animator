using Animator.Engine.Base;
using Animator.Engine.Persistence;
using Animator.Engine.Tests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Animator.Engine.Tests
{
    [TestClass]
    public class DeserializationTests
    {
        [TestMethod]
        public void SimpleDeserializationTest()
        {
            // Arrange

            string xml = "<SimplePropertyClass xmlns=\"assembly=Animator.Engine.Tests;namespace=Animator.Engine.Tests.TestClasses\" />";
            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var serializer = new ManagedObjectSerializer();

            // Act

            ManagedObject deserialized = serializer.Deserialize(document);

            // Assert

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(typeof(SimplePropertyClass), deserialized.GetType());
        }

        [TestMethod]
        public void SimpleDeserializationWithDefaultNamespaceTest()
        {
            // Arrange

            string xml = "<SimplePropertyClass />";
            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var serializer = new ManagedObjectSerializer();

            // Act

            var options = new DeserializationOptions
            {
                DefaultNamespace = new NamespaceDefinition("Animator.Engine.Tests", "Animator.Engine.Tests.TestClasses")
            };
            ManagedObject deserialized = serializer.Deserialize(document, options);

            // Assert

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(typeof(SimplePropertyClass), deserialized.GetType());
        }

        [TestMethod]
        public void AttributeDeserializationTest()
        {
            // Arrange

            string xml = "<SimplePropertyClass xmlns=\"assembly=Animator.Engine.Tests;namespace=Animator.Engine.Tests.TestClasses\" " +                
                "IntValue=\"42\" />";
            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var serializer = new ManagedObjectSerializer();

            // Act

            SimplePropertyClass deserialized = (SimplePropertyClass)serializer.Deserialize(document);

            // Assert

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(42, deserialized.IntValue);
        }

        [TestMethod]
        public void CoercedAttributeDeserializationTest()
        {
            // Arrange

            string xml = "<SimpleCoercedPropertyClass xmlns=\"assembly=Animator.Engine.Tests;namespace=Animator.Engine.Tests.TestClasses\" " +
                "Max10=\"42\" />";
            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var serializer = new ManagedObjectSerializer();

            // Act

            SimpleCoercedPropertyClass deserialized = (SimpleCoercedPropertyClass)serializer.Deserialize(document);

            // Assert

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(10, deserialized.Max10);
        }


    }
}
