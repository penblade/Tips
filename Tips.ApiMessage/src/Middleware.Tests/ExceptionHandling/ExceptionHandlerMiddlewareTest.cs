using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Tips.Middleware.ErrorHandling;
using Tips.Middleware.ExceptionHandling;
using Tips.Support.Tests;

namespace Tips.Middleware.Tests.ExceptionHandling
{
    [TestClass]
    public class ExceptionHandlerMiddlewareTest
    {
        [TestMethod]
        public async Task InvokeAsyncSuccessTest()
        {
            var mockHttpContext = new Mock<HttpContext>();
            var mockRequestDelegate = new Mock<RequestDelegate>();
            mockRequestDelegate.Setup(requestDelegate => requestDelegate.Invoke(mockHttpContext.Object));
            var mockLogger = new Mock<ILogger<ExceptionHandlerMiddleware>>();

            var middleware = new ExceptionHandlerMiddleware(mockRequestDelegate.Object, mockLogger.Object, null);
            await middleware.InvokeAsync(mockHttpContext.Object);

            mockRequestDelegate.Verify(requestDelegate => requestDelegate.Invoke(mockHttpContext.Object), Times.Once);
            // If an exception was thrown, then this test will fail with null references as the required objects were not setup.
            // We could setup additional mocks to verify the methods weren't called.
            // However, due to complications with extension methods, as detailed in the throws exception case below,
            // I am fine with leaving this scenario simple with the null reference exceptions.
        }

        [TestMethod]
        public async Task InvokeAsyncThrowsExceptionTest()
        {
            var problemDetailsFactory = SetupProblemDetailsFactory();
            var expectedProblemDetails = problemDetailsFactory.InternalServerError();
            var expectedSerializedProblemDetails = JsonSerializer.Serialize(expectedProblemDetails);
            var expectedProblemDetailsBytes = Encoding.UTF8.GetBytes(expectedSerializedProblemDetails);
            var expectedArgumentException = new ArgumentException("Test Message");

            var headers = SetupHeaders();

            var mockLogger = new Mock<ILogger<ExceptionHandlerMiddleware>>();

            var mockHttpRequest = mockLogger.SetupHttpRequest();
            var mockHttpResponse = SetupHttpResponse(mockLogger, headers, expectedProblemDetailsBytes);
            var mockConnectionInfo = mockLogger.SetupConnectionInfo();
            var mockHttpContext = mockLogger.SetupHttpContext(mockHttpRequest, mockHttpResponse, mockConnectionInfo);
            var mockRequestDelegate = SetupRequestDelegate(mockHttpContext, expectedArgumentException);

            var middleware = new ExceptionHandlerMiddleware(mockRequestDelegate.Object, mockLogger.Object, problemDetailsFactory);
            await middleware.InvokeAsync(mockHttpContext.Object);

            VerifyRequestDelegate(mockHttpContext, mockRequestDelegate);
            AssertHeaders(headers);
            VerifyContext(mockHttpContext);
            VerifyHttpResponse(mockHttpResponse, expectedProblemDetailsBytes);
            VerifyLogs(mockLogger, expectedSerializedProblemDetails, expectedArgumentException);
            mockLogger.VerifyBeginScope("Internal Server Error");
            mockLogger.VerifyLogResponse();
        }

        private static IProblemDetailsFactory SetupProblemDetailsFactory() => new ProblemDetailsFactory(new ProblemDetailsConfiguration());

        private static IHeaderDictionary SetupHeaders() => new HeaderDictionary();

        private static Mock<RequestDelegate> SetupRequestDelegate(Mock<HttpContext> mockHttpContext, Exception expectedArgumentException)
        {
            var mockRequestDelegate = new Mock<RequestDelegate>();
            mockRequestDelegate.Setup(requestDelegate => requestDelegate.Invoke(mockHttpContext.Object)).ThrowsAsync(expectedArgumentException);
            return mockRequestDelegate;
        }

        public static Mock<HttpResponse> SetupHttpResponse<TCategory>(Mock<ILogger<TCategory>> mockLogger, IHeaderDictionary headers, byte[] expectedProblemDetailsBytes)
        {
            var mockHttpResponse = mockLogger.SetupHttpResponse();

            mockHttpResponse.SetupGet(response => response.Headers).Returns(headers);

            // WriteAsync is an extension method, so mock the underlying method called.
            mockHttpResponse.Setup(response => response.Body.WriteAsync(expectedProblemDetailsBytes, 0,
                expectedProblemDetailsBytes.Length, It.IsAny<CancellationToken>()));
            return mockHttpResponse;
        }

        private static void VerifyRequestDelegate(Mock<HttpContext> mockHttpContext, Mock<RequestDelegate> mockRequestDelegate) =>
            mockRequestDelegate.Verify(requestDelegate => requestDelegate.Invoke(mockHttpContext.Object), Times.Once);

        private static void AssertHeaders(IHeaderDictionary headers)
        {
            Assert.AreEqual("no-cache", headers["Cache-Control"].ToString());
            Assert.AreEqual("no-cache", headers["Pragma"].ToString());
            Assert.AreEqual("-1", headers["Expires"].ToString());
        }

        // 1 for RequestDelegate, 3 for Headers, 1 for StatusCode, 1 for ContentType, 1 for WriteAsync, 1 for LogResponse
        private static void VerifyContext(Mock<HttpContext> mockHttpContext) => mockHttpContext.VerifyGet(context => context.Response, Times.Exactly(8));

        private static void VerifyHttpResponse(Mock<HttpResponse> mockHttpResponse, byte[] expectedProblemDetailsBytes)
        {
            mockHttpResponse.VerifySet(response => response.StatusCode = MockLoggerVerifyLogRequestResponseExtensions.InternalServerError, Times.Once);
            mockHttpResponse.VerifySet(response => response.ContentType = "application/problem+json", Times.Once);
            mockHttpResponse.Verify(response => response.Body.WriteAsync(expectedProblemDetailsBytes, 0, expectedProblemDetailsBytes.Length, It.IsAny<CancellationToken>()), Times.Once);
        }

        private static void VerifyLogs<T>(Mock<ILogger<T>> mockLogger, string expectedSerializedProblemDetails, Exception expectedException)
        {
            mockLogger.VerifyLog(LogLevel.Error, expectedSerializedProblemDetails, "ProblemDetails");
            mockLogger.VerifyLog(LogLevel.Error, expectedException, "Uncaught Exception");
        }
    }
}
