using Animator.Engine.Base.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animator.Engine.Base.Tests
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        public void SplitUnquotedTest1()
        {
            // Arrange

            string s = "abc,def,ghi";

            // Act

            string[] result = s.SplitUnquoted(',');

            // Assert

            Assert.AreEqual(3, result.Length);
            Assert.AreEqual("abc", result[0]);
            Assert.AreEqual("def", result[1]);
            Assert.AreEqual("ghi", result[2]);
        }

        [TestMethod]
        public void SplitUnquotedTest2()
        {
            // Arrange

            string s = "abc,'def',ghi";

            // Act

            string[] result = s.SplitUnquoted(',');

            // Assert

            Assert.AreEqual(3, result.Length);
            Assert.AreEqual("abc", result[0]);
            Assert.AreEqual("'def'", result[1]);
            Assert.AreEqual("ghi", result[2]);
        }

        [TestMethod]
        public void SplitUnquotedTest3()
        {
            // Arrange

            string s = "abc,'def,ghi'";

            // Act

            string[] result = s.SplitUnquoted(',');

            // Assert

            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("abc", result[0]);
            Assert.AreEqual("'def,ghi'", result[1]);
        }

        [TestMethod]
        public void SplitUnquotedTest4()
        {
            // Arrange

            string s = "abc,'a\\'b'";

            // Act

            string[] result = s.SplitUnquoted(',');

            // Assert

            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("abc", result[0]);
            Assert.AreEqual("'a\\'b'", result[1]);
        }

        [TestMethod]
        public void SplitUnquotedTest5()
        {
            // Arrange

            string s = "abc,,ghi";

            // Act

            string[] result = s.SplitUnquoted(',');

            // Assert

            Assert.AreEqual(3, result.Length);
            Assert.AreEqual("abc", result[0]);
            Assert.AreEqual("", result[1]);
            Assert.AreEqual("ghi", result[2]);
        }

        [TestMethod]
        public void SplitUnquotedTest6()
        {
            // Arrange

            string s = "abc,'',ghi";

            // Act

            string[] result = s.SplitUnquoted(',');

            // Assert

            Assert.AreEqual(3, result.Length);
            Assert.AreEqual("abc", result[0]);
            Assert.AreEqual("''", result[1]);
            Assert.AreEqual("ghi", result[2]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SplitUnquotedTest7()
        {
            // Arrange

            string s = "abc,'a";

            // Act

            string[] result = s.SplitUnquoted(',');            
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SplitUnquotedTest8()
        {
            // Arrange

            string s = "abc,'a\\";

            // Act

            string[] result = s.SplitUnquoted(',');
        }

        [TestMethod]
        public void ContainsUnquotedTest1()
        {
            // Arrange

            string s = "asd,fgh";

            // Act

            bool result = s.ContainsUnquoted(',');

            // Assert

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ContainsUnquotedTest2()
        {
            // Arrange

            string s = "as'd,f'gh";

            // Act

            bool result = s.ContainsUnquoted(',');

            // Assert

            Assert.IsFalse(result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ContainsUnquotedTest3()
        {
            // Arrange

            string s = "as'd,fgh";

            // Act

            bool result = s.ContainsUnquoted(',');
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ContainsUnquotedTest4()
        {
            // Arrange

            string s = "as'd,fgh\\";

            // Act

            bool result = s.ContainsUnquoted(',');
        }

        [TestMethod]
        public void ExpandQuotesTest1()
        {
            // Arrange

            string s = "asd";

            // Act

            string result = s.ExpandQuotes();

            // Assert

            Assert.AreEqual("asd", result);
        }

        [TestMethod]
        public void ExpandQuotesTest2()
        {
            // Arrange

            string s = "'asd'";

            // Act

            string result = s.ExpandQuotes();

            // Assert

            Assert.AreEqual("asd", result);
        }

        [TestMethod]
        public void ExpandQuotesTest3()
        {
            // Arrange

            string s = "'a\\'sd'";

            // Act

            string result = s.ExpandQuotes();

            // Assert

            Assert.AreEqual("a'sd", result);
        }

        [TestMethod]
        public void ExpandQuotesTest4()
        {
            // Arrange

            string s = "'a\\sd'";

            // Act

            string result = s.ExpandQuotes();

            // Assert

            Assert.AreEqual("asd", result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExpandQuotesTest5()
        {
            // Arrange

            string s = "'as";

            // Act

            string result = s.ExpandQuotes();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ExpandQuotesTest6()
        {
            // Arrange

            string s = "'as\\";

            // Act

            string result = s.ExpandQuotes();
        }

    }
}
