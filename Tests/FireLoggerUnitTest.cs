using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NFireLogger;

namespace Tests
{
    [TestClass]
    public class FireLoggerUnitTest
    {

        public static Mock<HttpContextBase> PrepareHttpContextForRequestTests(bool addHeaders)
        {
            var httpContext = new Mock<HttpContextBase>();
            var httpRequest = new Mock<HttpRequestBase>();
            var httpResponse = new Mock<HttpResponseBase>();
            var headers = new NameValueCollection();

            if (addHeaders)
            {
                headers["X-FireLogger"] = "1.0";
            }

            httpContext.SetupGet(c => c.Request).Returns(httpRequest.Object);
            httpContext.SetupGet(c => c.Response).Returns(httpResponse.Object);
            httpRequest.SetupGet(r => r.Headers).Returns(headers);

            return httpContext;
        }

        public static Mock<FireLogger> PrepareFileLoggerForMessageTest(Action<LogMessage> callback)
        {
            var http = PrepareHttpContextForRequestTests(true);
            var mock = new Mock<FireLogger>(http.Object);

            mock.Setup(fn => fn.Log(It.IsAny<LogMessage>())).Callback(callback);

            return mock;
        }


        [TestMethod]
        public void TestLogMessageContent()
        {
            StackFrame fr = new StackFrame(0, true);

            var mock = PrepareFileLoggerForMessageTest(msg =>
            {
                Assert.AreEqual(fr.GetFileName(), msg.PathName);
                Assert.AreEqual(fr.GetFileLineNumber() + 1, msg.LineNo);
                Assert.AreEqual(Level.Info, msg.Level);
                Assert.AreEqual(FireLogger.DEFAULT_NAME, msg.Name);
                Assert.AreEqual("lorem ipsum", msg.Message);
            });

            // next 2 lines has to be immediately after each other
            fr = new StackFrame(0, true);
            mock.Object.Log(Level.Info, "lorem ipsum");
        }




        [TestMethod]
        public void TestAutoDetectState()
        {
            var httpContextA = PrepareHttpContextForRequestTests(false);
            var flogA = new FireLogger(httpContextA.Object);
            Assert.IsFalse(flogA.Enabled);

            var httpContextB = PrepareHttpContextForRequestTests(true);
            var flogB = new FireLogger(httpContextB.Object);
            Assert.IsTrue(flogB.Enabled);
        }



        [TestMethod]
        public void TestSend()
        {
            var httpContext = PrepareHttpContextForRequestTests(true);
            var flog = new FireLogger(httpContext.Object);

            flog.Log(Level.Info, "Test");

            flog.Flush();


        }
    }
}
