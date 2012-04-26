using System;
using System.Collections.Specialized;
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

        private static Mock<HttpContextBase> PrepareHttpContextForRequestTests(bool addHeaders)
        {
            var httpContext = new Mock<HttpContextBase>();
            var httpRequest = new Mock<HttpRequestBase>();
            var headers = new NameValueCollection();

            if (addHeaders)
            {
                headers["X-FireLogger"] = "1.0";
            }

            httpContext.SetupGet(c => c.Request).Returns(httpRequest.Object);
            httpRequest.SetupGet(r => r.Headers).Returns(headers);

            return httpContext;
        }

        [TestMethod]
        public void TestSend()
        {
            var httpContext = PrepareHttpContextForRequestTests(true);
            var flog = new FireLogger(httpContext.Object);

            flog.Log(Level.Info, "Test");

            flog.Send();


        }
    }
}
