using Animator.Engine.Base;
using Animator.Engine.Base.Persistence.Types;
using Animator.Engine.Base.Persistence;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Collections.ObjectModel;
using Animator.Engine.Base.Tests.TestClasses;
using System.Reflection;

namespace Animator.Engine.Base.Tests
{
    [TestClass]
    public class DeserializationTests
    {
        [TestMethod]
        public void SimpleDeserializationTest()
        {
            // Arrange

            string xml = $"<SimplePropertyClass xmlns=\"assembly={Assembly.GetExecutingAssembly().FullName};namespace={typeof(SimplePropertyClass).Namespace}\" />";
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
                DefaultNamespace = new NamespaceDefinition(Assembly.GetExecutingAssembly().FullName, typeof(SimplePropertyClass).Namespace)
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

            string xml = $"<SimplePropertyClass xmlns=\"assembly={Assembly.GetExecutingAssembly().FullName};namespace={typeof(SimplePropertyClass).Namespace}\" " +
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

            string xml = $"<SimpleCoercedPropertyClass xmlns=\"assembly={Assembly.GetExecutingAssembly().FullName};namespace={typeof(SimplePropertyClass).Namespace}\" " +
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
            string xml = $"<SimpleCompositeClass xmlns=\"assembly={Assembly.GetExecutingAssembly().FullName};namespace={typeof(SimplePropertyClass).Namespace}\">\r\n" +
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
            string xml = $"<SimpleCompositeClass xmlns=\"assembly={Assembly.GetExecutingAssembly().FullName};namespace={typeof(SimplePropertyClass).Namespace}\">\r\n" +
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
            string xml = $"<SimpleCollectionClass xmlns=\"assembly={Assembly.GetExecutingAssembly().FullName};namespace={typeof(SimplePropertyClass).Namespace}\">\r\n" +
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
            string xml = $"<SimpleCollectionClass xmlns=\"assembly={Assembly.GetExecutingAssembly().FullName};namespace={typeof(SimplePropertyClass).Namespace}\">\r\n" +
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
            string xml = $"<IntDataClass xmlns=\"assembly={Assembly.GetExecutingAssembly().FullName};namespace={typeof(SimplePropertyClass).Namespace}\" " +
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
            string xml = $"<IntDataClass xmlns=\"assembly={Assembly.GetExecutingAssembly().FullName};namespace={typeof(SimplePropertyClass).Namespace}\" " +
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
            string xml = $"<CustomSerializedIntDataClass xmlns=\"assembly={Assembly.GetExecutingAssembly().FullName};namespace={typeof(SimplePropertyClass).Namespace}\" " +
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
            string xml = $"<CustomSerializedIntDataClass xmlns=\"assembly={Assembly.GetExecutingAssembly().FullName};namespace={typeof(SimplePropertyClass).Namespace}\" " +
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
            string xml = $"<CustomSerializedIntDataClass xmlns=\"assembly={Assembly.GetExecutingAssembly().FullName};namespace={typeof(SimplePropertyClass).Namespace}\">" +
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
            string xml = $"<CustomSerializedIntDataClass xmlns=\"assembly={Assembly.GetExecutingAssembly().FullName};namespace={typeof(SimplePropertyClass).Namespace}\">" +
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
            string xml = $"<NontrivialCtorClass xmlns=\"assembly={Assembly.GetExecutingAssembly().FullName};namespace={typeof(SimplePropertyClass).Namespace}\" />";

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

        [TestMethod]
        public void MarkupExtensionDeserializationTest1()
        {
            // Arrange

            string xml = $"<IntDataClass xmlns=\"assembly={Assembly.GetExecutingAssembly().FullName};namespace={typeof(SimplePropertyClass).Namespace}\" IntValue=\"{{MarkupExtension}}\" />";

            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var serializer = new ManagedObjectSerializer();

            // Act

            IntDataClass data = (IntDataClass)serializer.Deserialize(document);

            // Assert

            Assert.IsNotNull(data);
            Assert.AreEqual(0, data.IntValue);
        }

        [TestMethod]
        public void MarkupExtensionDeserializationTest2()
        {
            // Arrange

            string xml = $"<IntDataClass xmlns=\"assembly={Assembly.GetExecutingAssembly().FullName};namespace={typeof(SimplePropertyClass).Namespace}\" IntValue=\"{{MarkupExtension 8}}\" />";

            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var serializer = new ManagedObjectSerializer();

            // Act

            IntDataClass data = (IntDataClass)serializer.Deserialize(document);

            // Assert

            Assert.IsNotNull(data);
            Assert.AreEqual(8, data.IntValue);
        }

        [TestMethod]
        public void MarkupExtensionDeserializationTest3()
        {
            // Arrange

            string xml = $"<IntDataClass xmlns=\"assembly={Assembly.GetExecutingAssembly().FullName};namespace={typeof(SimplePropertyClass).Namespace}\" IntValue=\"{{MarkupExtension Value=8}}\" />";

            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var serializer = new ManagedObjectSerializer();

            // Act

            IntDataClass data = (IntDataClass)serializer.Deserialize(document);

            // Assert

            Assert.IsNotNull(data);
            Assert.AreEqual(8, data.IntValue);
        }

        [TestMethod]
        public void MarkupExtensionDeserializationTest4()
        {
            // Arrange

            string xml = $"<IntDataClass xmlns=\"assembly={Assembly.GetExecutingAssembly().FullName};namespace={typeof(SimplePropertyClass).Namespace}\" IntValue=\"{{MarkupExtension 4, Offset1=1}}\" />";

            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var serializer = new ManagedObjectSerializer();

            // Act

            IntDataClass data = (IntDataClass)serializer.Deserialize(document);

            // Assert

            Assert.IsNotNull(data);
            Assert.AreEqual(5, data.IntValue);
        }

        [TestMethod]
        public void MarkupExtensionDeserializationTest5()
        {
            // Arrange

            string xml = $"<IntDataClass xmlns=\"assembly={Assembly.GetExecutingAssembly().FullName};namespace={typeof(SimplePropertyClass).Namespace}\" IntValue=\"{{MarkupExtension Value=4, Offset1=1}}\" />";

            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var serializer = new ManagedObjectSerializer();

            // Act

            IntDataClass data = (IntDataClass)serializer.Deserialize(document);

            // Assert

            Assert.IsNotNull(data);
            Assert.AreEqual(5, data.IntValue);
        }

        [TestMethod]
        public void MarkupExtensionDeserializationTest6()
        {
            // Arrange

            string xml = $"<IntDataClass xmlns=\"assembly={Assembly.GetExecutingAssembly().FullName};namespace={typeof(SimplePropertyClass).Namespace}\" IntValue=\"{{MarkupExtension 4, Offset1=1, Offset2=1}}\" />";

            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var serializer = new ManagedObjectSerializer();

            // Act

            IntDataClass data = (IntDataClass)serializer.Deserialize(document);

            // Assert

            Assert.IsNotNull(data);
            Assert.AreEqual(6, data.IntValue);
        }

        [TestMethod]
        public void MarkupExtensionDeserializationTest7()
        {
            // Arrange

            string xml = $"<IntDataClass xmlns=\"assembly={Assembly.GetExecutingAssembly().FullName};namespace={typeof(SimplePropertyClass).Namespace}\" IntValue=\"{{MarkupExtension Value=4, Offset1=1, Offset2=1}}\" />";

            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var serializer = new ManagedObjectSerializer();

            // Act

            IntDataClass data = (IntDataClass)serializer.Deserialize(document);

            // Assert

            Assert.IsNotNull(data);
            Assert.AreEqual(6, data.IntValue);
        }

        [TestMethod]
        public void SimpleMacroTest()
        {
            // Arrange

            string xml = $"<SimpleCollectionClass xmlns=\"assembly={Assembly.GetExecutingAssembly().FullName};namespace={typeof(SimplePropertyClass).Namespace}\"\n" +
                "    xmlns:x=\"https://spooksoft.pl/animator\">\n" +
                "   <x:Macros>\n" +
                "       <SimplePropertyClass x:Key=\"TestMacro\" IntValue=\"44\" />\n" +
                "   </x:Macros>\n" +
                "   <SimpleCollectionClass.Items>\n" +
                "       <x:Macro x:Key=\"TestMacro\" />\n" +
                "   </SimpleCollectionClass.Items>\n" +
                "</SimpleCollectionClass>";

            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var serializer = new ManagedObjectSerializer();

            // Act

            SimpleCollectionClass data = (SimpleCollectionClass)serializer.Deserialize(document);

            // Assert

            Assert.IsNotNull(data);
            Assert.AreEqual(1, data.Items.Count);
            Assert.AreEqual(44, data.Items[0].IntValue);
        }

        [TestMethod]
        public void PropertyOverridingMacroTest()
        {
            // Arrange

            string xml = $"<SimpleCollectionClass xmlns=\"assembly={Assembly.GetExecutingAssembly().FullName};namespace={typeof(SimplePropertyClass).Namespace}\"\n" +
                "    xmlns:x=\"https://spooksoft.pl/animator\">\n" +
                "   <x:Macros>\n" +
                "       <SimplePropertyClass x:Key=\"TestMacro\" />\n" +
                "   </x:Macros>\n" +
                "   <SimpleCollectionClass.Items>\n" +
                "       <x:Macro x:Key=\"TestMacro\" IntValue=\"44\" />\n" +
                "   </SimpleCollectionClass.Items>\n" +
                "</SimpleCollectionClass>";

            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var serializer = new ManagedObjectSerializer();

            // Act

            SimpleCollectionClass data = (SimpleCollectionClass)serializer.Deserialize(document);

            // Assert

            Assert.IsNotNull(data);
            Assert.AreEqual(1, data.Items.Count);
            Assert.AreEqual(44, data.Items[0].IntValue);
        }

        [TestMethod]
        public void ExtendedMarkupExtensionParametersTest1()
        {
            // Arrange

            string xml = $"<StringDataClass xmlns=\"assembly={Assembly.GetExecutingAssembly().FullName};namespace={typeof(SimplePropertyClass).Namespace}\" StringValue=\"{{StringMarkupExtension 'Test'}}\" />";

            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var serializer = new ManagedObjectSerializer();

            // Act

            StringDataClass data = (StringDataClass)serializer.Deserialize(document);

            // Assert

            Assert.IsNotNull(data);
            Assert.AreEqual("Test", data.StringValue);
        }

        [TestMethod]
        public void ExtendedMarkupExtensionParametersTest2()
        {
            // Arrange

            string xml = $"<StringDataClass xmlns=\"assembly={Assembly.GetExecutingAssembly().FullName};namespace={typeof(SimplePropertyClass).Namespace}\" StringValue=\"{{StringMarkupExtension 'Test', Optional=Test2}}\" />";

            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var serializer = new ManagedObjectSerializer();

            // Act

            StringDataClass data = (StringDataClass)serializer.Deserialize(document);

            // Assert

            Assert.IsNotNull(data);
            Assert.AreEqual("TestTest2", data.StringValue);
        }

        [TestMethod]
        public void ExtendedMarkupExtensionParametersTest3()
        {
            // Arrange

            string xml = $"<StringDataClass xmlns=\"assembly={Assembly.GetExecutingAssembly().FullName};namespace={typeof(SimplePropertyClass).Namespace}\" StringValue=\"{{StringMarkupExtension 'Test', Optional='Test2'}}\" />";

            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var serializer = new ManagedObjectSerializer();

            // Act

            StringDataClass data = (StringDataClass)serializer.Deserialize(document);

            // Assert

            Assert.IsNotNull(data);
            Assert.AreEqual("TestTest2", data.StringValue);
        }

        [TestMethod]
        public void ExtendedMarkupExtensionParametersTest4()
        {
            // Arrange

            string xml = $"<StringDataClass xmlns=\"assembly={Assembly.GetExecutingAssembly().FullName};namespace={typeof(SimplePropertyClass).Namespace}\" StringValue=\"{{StringMarkupExtension 'Test\\'', Optional='Test2\\''}}\" />";

            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var serializer = new ManagedObjectSerializer();

            // Act

            StringDataClass data = (StringDataClass)serializer.Deserialize(document);

            // Assert

            Assert.IsNotNull(data);
            Assert.AreEqual("Test'Test2'", data.StringValue);
        }

        [TestMethod]
        public void ExtendedMarkupExtensionParametersTest5()
        {
            // Arrange

            string xml = $"<StringDataClass xmlns=\"assembly={Assembly.GetExecutingAssembly().FullName};namespace={typeof(SimplePropertyClass).Namespace}\" StringValue=\"{{StringMarkupExtension 'Test,', Optional='Test2='}}\" />";

            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var serializer = new ManagedObjectSerializer();

            // Act

            StringDataClass data = (StringDataClass)serializer.Deserialize(document);

            // Assert

            Assert.IsNotNull(data);
            Assert.AreEqual("Test,Test2=", data.StringValue);
        }

        [TestMethod]
        public void ExtendedMarkupExtensionParametersTest6()
        {
            // Arrange

            string xml = $"<StringDataClass xmlns=\"assembly={Assembly.GetExecutingAssembly().FullName};namespace={typeof(SimplePropertyClass).Namespace}\" StringValue=\"{{StringMarkupExtension 'Test=', Optional='Test2,'}}\" />";

            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var serializer = new ManagedObjectSerializer();

            // Act

            StringDataClass data = (StringDataClass)serializer.Deserialize(document);

            // Assert

            Assert.IsNotNull(data);
            Assert.AreEqual("Test=Test2,", data.StringValue);
        }

        [TestMethod]
        public void GeneratorTest()
        {
            // Arrange

            string xml = $"<x:Generate xmlns:x=\"https://spooksoft.pl/animator\"><SimpleGenerator xmlns=\"assembly={Assembly.GetExecutingAssembly().FullName};namespace={typeof(SimpleGenerator).Namespace}\" /></x:Generate>";

            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            var serializer = new ManagedObjectSerializer();

            // Act

            ManagedObject obj = serializer.Deserialize(document);

            // Assert

            Assert.IsNotNull(obj);
            Assert.IsInstanceOfType(obj, typeof(SimplePropertyClass));

            var propClass = (SimplePropertyClass)obj;
            Assert.AreEqual(99, propClass.IntValue);
        }
    }
}
