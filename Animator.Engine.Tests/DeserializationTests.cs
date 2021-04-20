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

        [TestMethod]
        public void NestedObjectDeserializationTest()
        {
            string xml = "<SimpleCompositeClass xmlns=\"assembly=Animator.Engine.Tests;namespace=Animator.Engine.Tests.TestClasses\">\r\n" +
                "    <SimpleCompositeClass.NestedObject>\r\n" +
                "        <SimplePropertyClass IntValue=\"42\" />\r\n" +
                "    </SimpleCompositeClass.NestedObject>\r\n" +
                "</SimpleCompositeClass>";

            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var serializer = new ManagedObjectSerializer();

            // Act

            SimpleCompositeClass deserialized = (SimpleCompositeClass)serializer.Deserialize(document);

            // Assert

            Assert.IsNotNull(deserialized);
            Assert.IsNotNull(deserialized.NestedObject);
            Assert.AreEqual(42, deserialized.NestedObject.IntValue);
        }

        [TestMethod]
        public void ContentPropertyDeserializationTest()
        {
            string xml = "<SimpleCompositeClass xmlns=\"assembly=Animator.Engine.Tests;namespace=Animator.Engine.Tests.TestClasses\">\r\n" +
                "    <SimplePropertyClass IntValue=\"42\" />\r\n" +
                "</SimpleCompositeClass>";

            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var serializer = new ManagedObjectSerializer();

            // Act

            SimpleCompositeClass deserialized = (SimpleCompositeClass)serializer.Deserialize(document);

            // Assert

            Assert.IsNotNull(deserialized);
            Assert.IsNotNull(deserialized.NestedObject);
            Assert.AreEqual(42, deserialized.NestedObject.IntValue);
        }

        [TestMethod]
        public void CollectionDeserializationTest()
        {
            string xml = "<SimpleCollectionClass xmlns=\"assembly=Animator.Engine.Tests;namespace=Animator.Engine.Tests.TestClasses\">\r\n" +
                "    <SimpleCollectionClass.Items>\r\n" +
                "       <SimplePropertyClass IntValue=\"10\" />\r\n" +
                "       <SimplePropertyClass IntValue=\"20\" />\r\n" +
                "       <SimplePropertyClass IntValue=\"30\" />\r\n" +
                "    </SimpleCollectionClass.Items>\r\n" +
                "</SimpleCollectionClass>";

            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var serializer = new ManagedObjectSerializer();

            // Act

            SimpleCollectionClass deserialized = (SimpleCollectionClass)serializer.Deserialize(document);

            // Assert

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(3, deserialized.Items.Count);
            Assert.AreEqual(10, deserialized.Items[0].IntValue);
        }

        [TestMethod]
        public void CollectionAsContentPropertyDeserializationTest()
        {
            string xml = "<SimpleCollectionClass xmlns=\"assembly=Animator.Engine.Tests;namespace=Animator.Engine.Tests.TestClasses\">\r\n" +
                "   <SimplePropertyClass IntValue=\"10\" />\r\n" +
                "   <SimplePropertyClass IntValue=\"20\" />\r\n" +
                "   <SimplePropertyClass IntValue=\"30\" />\r\n" +
                "</SimpleCollectionClass>";

            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var serializer = new ManagedObjectSerializer();

            // Act

            SimpleCollectionClass deserialized = (SimpleCollectionClass)serializer.Deserialize(document);

            // Assert

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(3, deserialized.Items.Count);
            Assert.AreEqual(10, deserialized.Items[0].IntValue);
        }

        [TestMethod]
        public void CustomPropertySerializationTest()
        {
            string xml = "<CustomSerializedClass xmlns=\"assembly=Animator.Engine.Tests;namespace=Animator.Engine.Tests.TestClasses\" " +
                "IntValue=\"-42\" />";

            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var serializer = new ManagedObjectSerializer();

            // Act

            CustomSerializedClass deserialized = (CustomSerializedClass)serializer.Deserialize(document);

            // Assert

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(42, deserialized.IntValue);
        }

        [TestMethod]
        public void CustomCollectionSerializationTest()
        {
            string xml = "<CustomSerializedClass xmlns=\"assembly=Animator.Engine.Tests;namespace=Animator.Engine.Tests.TestClasses\" " +
                "IntCollection=\"4,3,2,1\" />";

            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var serializer = new ManagedObjectSerializer();

            // Act

            CustomSerializedClass deserialized = (CustomSerializedClass)serializer.Deserialize(document);

            // Assert

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(4, deserialized.IntCollection.Count);
            Assert.AreEqual(4, deserialized.IntCollection[0]);
        }

        [TestMethod]
        public void CustomActivatorDeserializationTest()
        {
            string xml = "<NontrivialCtorClass xmlns=\"assembly=Animator.Engine.Tests;namespace=Animator.Engine.Tests.TestClasses\" />";

            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var serializer = new ManagedObjectSerializer();
            var options = new DeserializationOptions
            {
                CustomActivator = new NontrivialCtorCustomActivator()
            };

            // Act

            NontrivialCtorClass deserialized = (NontrivialCtorClass)serializer.Deserialize(document, options);

            // Assert

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(deserialized.Value, 42);
        }
    }
}
