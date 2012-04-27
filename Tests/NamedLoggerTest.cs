using System.Diagnostics;
using Moq;
using NFireLogger;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests
{
    
    [TestClass()]
    public class NamedLoggerTest
    {
        [TestMethod]
        public void TestLogMessage()
        {
            var thisFilePath = new StackFrame(0, true).GetFileName();

            var mock = FireLoggerUnitTest.PrepareFileLoggerForMessageTest(msg =>
            {
                Assert.AreEqual(thisFilePath, msg.PathName);
                Assert.AreEqual(30, msg.LineNo);
                Assert.AreEqual(Level.Info, msg.Level);
                Assert.AreEqual("name", msg.Name);
                Assert.AreEqual("info", msg.Message);
            });

            var n = new NamedLogger("name", mock.Object);

            // next line has number 30! 
            n.Info("info");
        }

    }
}
