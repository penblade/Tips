using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Support.Tests;
using Tips.Middleware.Logging;
using Tips.Security;

namespace Middleware.Tests.Logging
{
    [TestClass]
    public class HttpInfoLoggerMiddlewareTest
    {
        [TestMethod]
        public async Task InvokeAsyncTest()
        {
            const string expectedApiKeyOwner = "Test Owner";
            var mockLogger = new Mock<ILogger<HttpInfoLoggerMiddleware>>();
            var mockHttpRequest = mockLogger.SetupHttpRequest();
            var mockHttpResponse = mockLogger.SetupHttpResponse();
            var mockConnectionInfo = mockLogger.SetupConnectionInfo();
            var mockHttpContext = mockLogger.SetupHttpContext(mockHttpRequest, mockHttpResponse, mockConnectionInfo);

            var mockRequestDelegate = new Mock<RequestDelegate>();
            mockRequestDelegate.Setup(requestDelegate => requestDelegate.Invoke(mockHttpContext.Object));

            var expectedApiKey = new ApiKey { Created = new DateTime(2021, 01, 01), Id = 3, Key = "SomeUniqueId", Owner = expectedApiKeyOwner };
            var mockApiKeyRepository = new Mock<IApiKeyRepository>();
            mockApiKeyRepository.Setup(repository => repository.GetApiKeyFromHeaders(mockHttpContext.Object)).Returns(expectedApiKey);

            var parentActivity = new Activity("ParentActivity").Start();
            var currentActivity = new Activity("CurrentActivity").Start();

            currentActivity.TraceStateString = "State2,State3";

            try
            {
                var middleware = new HttpInfoLoggerMiddleware(mockRequestDelegate.Object, mockLogger.Object, mockApiKeyRepository.Object);

                await middleware.InvokeAsync(mockHttpContext.Object);

                mockLogger.VerifyBeginScope("Processing Request");
                mockLogger.VerifyLogRequest(mockHttpContext, expectedApiKeyOwner);
                mockRequestDelegate.Verify(next => next.Invoke(mockHttpContext.Object), Times.Once);
                mockLogger.VerifyBeginScope("Returning Response");
                mockLogger.VerifyLogResponse();
            }
            finally
            {
                currentActivity.Stop();
                parentActivity.Stop();
            }
        }
    }
}
