using NFireLogger.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests
{
    
    [TestClass()]
    public class StringExtensionsTest
    {
        [TestMethod()]   
        public void SplitToArrayTest()
        {
            var a = "abc".SplitToArray(100);
            Assert.AreEqual(1, a.Length);
            Assert.AreEqual("abc", a[0]);

            a = string.Empty.SplitToArray(100);
            Assert.AreEqual(0, a.Length);

            a = "0123456789012345".SplitToArray(10);
            Assert.AreEqual(2, a.Length);
            Assert.AreEqual("0123456789", a[0]);
            Assert.AreEqual("012345", a[1]);
        }
    }
}
