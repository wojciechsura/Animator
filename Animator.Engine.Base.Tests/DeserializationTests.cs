﻿using Animator.Engine.Base;
using Animator.Engine.Base.Persistence.Types;
using Animator.Engine.Base.Persistence;
using Animator.Engine.Tests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Collections.ObjectModel;

namespace Animator.Engine.Base.Tests
{
    [TestClass]
    public class DeserializationTests
    {
        [TestMethod]
        public void SimpleDeserializationTest()
        {
            // Arrange

            string xml = "<SimplePropertyClass xmlns=\"assembly=Animator.Engine.Base.Tests;namespace=Animator.Engine.Tests.TestClasses\" />";
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
                DefaultNamespace = new NamespaceDefinition("Animator.Engine.Base.Tests", "Animator.Engine.Tests.TestClasses")
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

            string xml = "<SimplePropertyClass xmlns=\"assembly=Animator.Engine.Base.Tests;namespace=Animator.Engine.Tests.TestClasses\" " +
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

            string xml = "<SimpleCoercedPropertyClass xmlns=\"assembly=Animator.Engine.Base.Tests;namespace=Animator.Engine.Tests.TestClasses\" " +
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
            string xml = "<SimpleCompositeClass xmlns=\"assembly=Animator.Engine.Base.Tests;namespace=Animator.Engine.Tests.TestClasses\">\r\n" +
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
            string xml = "<SimpleCompositeClass xmlns=\"assembly=Animator.Engine.Base.Tests;namespace=Animator.Engine.Tests.TestClasses\">\r\n" +
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
            string xml = "<SimpleCollectionClass xmlns=\"assembly=Animator.Engine.Base.Tests;namespace=Animator.Engine.Tests.TestClasses\">\r\n" +
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
            string xml = "<SimpleCollectionClass xmlns=\"assembly=Animator.Engine.Base.Tests;namespace=Animator.Engine.Tests.TestClasses\">\r\n" +
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
        public void GlobalCustomPropertySerializationTest()
        {
            string xml = "<IntDataClass xmlns=\"assembly=Animator.Engine.Base.Tests;namespace=Animator.Engine.Tests.TestClasses\" " +
                "IntValue=\"-42\" />";

            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var serializer = new ManagedObjectSerializer();
            var options = new DeserializationOptions
            {
                CustomSerializers = new Dictionary<Type, TypeSerializer>
                {
                    { typeof(int), new CustomIntSerializer() },
                }
            };

            // Act

            IntDataClass deserialized = (IntDataClass)serializer.Deserialize(document, options);

            // Assert

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(42, deserialized.IntValue);
        }

        [TestMethod]
        public void GlobalCustomCollectionSerializationTest()
        {
            string xml = "<IntDataClass xmlns=\"assembly=Animator.Engine.Base.Tests;namespace=Animator.Engine.Tests.TestClasses\" " +
                "IntCollection=\"4,3,2,1\" />";

            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var serializer = new ManagedObjectSerializer();
            var options = new DeserializationOptions
            {
                CustomSerializers = new Dictionary<Type, TypeSerializer>
                {
                    { typeof(ManagedCollection<int>), new CustomIntListSerializer() }
                }
            };

            // Act

            IntDataClass deserialized = (IntDataClass)serializer.Deserialize(document, options);

            // Assert

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(4, deserialized.IntCollection.Count);
            Assert.AreEqual(4, deserialized.IntCollection[0]);
        }

        [TestMethod]
        public void PerPropertyCustomSerializationTest1()
        {
            string xml = "<CustomSerializedIntDataClass xmlns=\"assembly=Animator.Engine.Base.Tests;namespace=Animator.Engine.Tests.TestClasses\" " +
                "IntValue=\"-42\" />";

            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var serializer = new ManagedObjectSerializer();

            // Act

            CustomSerializedIntDataClass deserialized = (CustomSerializedIntDataClass)serializer.Deserialize(document);

            // Assert

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(42, deserialized.IntValue);
        }

        [TestMethod]
        public void PerPropertyCustomCollectionSerializationTest1()
        {
            string xml = "<CustomSerializedIntDataClass xmlns=\"assembly=Animator.Engine.Base.Tests;namespace=Animator.Engine.Tests.TestClasses\" " +
                "IntCollection=\"4,3,2,1\" />";

            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var serializer = new ManagedObjectSerializer();

            // Act

            CustomSerializedIntDataClass deserialized = (CustomSerializedIntDataClass)serializer.Deserialize(document);

            // Assert

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(4, deserialized.IntCollection.Count);
            Assert.AreEqual(4, deserialized.IntCollection[0]);
        }

        [TestMethod]
        public void PerPropertyCustomSerializationTest2()
        {
            string xml = "<CustomSerializedIntDataClass xmlns=\"assembly=Animator.Engine.Base.Tests;namespace=Animator.Engine.Tests.TestClasses\">" +
                "  <CustomSerializedIntDataClass.IntValue>-42</CustomSerializedIntDataClass.IntValue>" +
                "</CustomSerializedIntDataClass>";

            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var serializer = new ManagedObjectSerializer();

            // Act

            CustomSerializedIntDataClass deserialized = (CustomSerializedIntDataClass)serializer.Deserialize(document);

            // Assert

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(42, deserialized.IntValue);
        }

        [TestMethod]
        public void PerPropertyCustomCollectionSerializationTest2()
        {
            string xml = "<CustomSerializedIntDataClass xmlns=\"assembly=Animator.Engine.Base.Tests;namespace=Animator.Engine.Tests.TestClasses\">" +
                "  <CustomSerializedIntDataClass.IntCollection>4,3,2,1</CustomSerializedIntDataClass.IntCollection>" +
                "</CustomSerializedIntDataClass>";

            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var serializer = new ManagedObjectSerializer();

            // Act

            CustomSerializedIntDataClass deserialized = (CustomSerializedIntDataClass)serializer.Deserialize(document);

            // Assert

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(4, deserialized.IntCollection.Count);
            Assert.AreEqual(4, deserialized.IntCollection[0]);
        }

        [TestMethod]
        public void CustomActivatorDeserializationTest()
        {
            string xml = "<NontrivialCtorClass xmlns=\"assembly=Animator.Engine.Base.Tests;namespace=Animator.Engine.Tests.TestClasses\" />";

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