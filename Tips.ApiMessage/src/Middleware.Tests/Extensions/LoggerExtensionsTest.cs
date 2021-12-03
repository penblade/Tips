using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Tips.Middleware.Extensions;
using Tips.Support.Tests;

namespace Tips.Middleware.Tests.Extensions
{
    [TestClass]
    public class LoggerExtensionsTest
    {
        [TestMethod]
        public void LogRequestTest()
        {
            const string expectedApiKeyOwner = "Test Owner";
            var mockLogger = new Mock<ILogger<LoggerExtensionsTest>>();
            var mockHttpRequest = mockLogger.SetupHttpRequest();
            var mockHttpResponse = mockLogger.SetupHttpResponse();
            var mockConnectionInfo = mockLogger.SetupConnectionInfo();
            var mockHttpContext = mockLogger.SetupHttpContext(mockHttpRequest, mockHttpResponse, mockConnectionInfo);

            var parentActivity = new Activity("ParentActivity").Start();
            var currentActivity = new Activity("CurrentActivity").Start();

            currentActivity.TraceStateString = "State2,State3";

            try
            {
                mockLogger.Object.LogRequest(mockHttpContext.Object, expectedApiKeyOwner);
                mockLogger.VerifyLogRequest(mockHttpContext, expectedApiKeyOwner);
            }
            finally
            {
                currentActivity.Stop();
                parentActivity.Stop();
            }
        }

        [TestMethod]
        public void LogResponseTest()
        {
            var mockLogger = new Mock<ILogger<LoggerExtensionsTest>>();
            var mockHttpRequest = mockLogger.SetupHttpRequest();
            var mockHttpResponse = mockLogger.SetupHttpResponse();
            var mockConnectionInfo = mockLogger.SetupConnectionInfo();
            var mockHttpContext = mockLogger.SetupHttpContext(mockHttpRequest, mockHttpResponse, mockConnectionInfo);

            mockLogger.Object.LogResponse(mockHttpContext.Object);

            mockLogger.VerifyLogResponse();
        }
    }
}
