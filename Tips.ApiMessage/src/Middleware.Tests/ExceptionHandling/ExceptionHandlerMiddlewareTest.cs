using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Support.Tests;
using Tips.Middleware.ErrorHandling;
using Tips.Middleware.ExceptionHandling;

namespace Middleware.Tests.ExceptionHandling
{
    [TestClass]
    public class ExceptionHandlerMiddlewareTest
    {
        private const string Https = "https";
        private const int InternalServerError = (int) HttpStatusCode.InternalServerError;

        private readonly IHeaderDictionary _headers = new HeaderDictionary();
        private readonly Mock<HttpRequest> _mockHttpRequest = new();
        private readonly Mock<HttpResponse> _mockHttpResponse = new();
        private readonly Mock<HttpContext> _mockHttpContext = new();
        private readonly Mock<ILogger<ExceptionHandlerMiddleware>> _mockLogger = new();
        private readonly IProblemDetailsFactory _problemDetailsFactory = new ProblemDetailsFactory(new ProblemDetailsConfiguration());
        private readonly Mock<RequestDelegate> _mockRequestDelegate = new();

        [TestMethod]
        public async Task InvokeAsyncSuccessTest()
        {
            var middleware = new ExceptionHandlerMiddleware(_mockRequestDelegate.Object, _mockLogger.Object, _problemDetailsFactory);
            await middleware.InvokeAsync(_mockHttpContext.Object);

            _mockRequestDelegate.Verify(requestDelegate => requestDelegate.Invoke(_mockHttpContext.Object), Times.Once);
        }

        [TestMethod]
        public async Task InvokeAsyncThrowsExceptionTest()
        {
            var expectedProblemDetails = _problemDetailsFactory.InternalServerError();
            var expectedSerializedProblemDetails = JsonSerializer.Serialize(expectedProblemDetails);
            var expectedProblemDetailsBytes = Encoding.UTF8.GetBytes(expectedSerializedProblemDetails);
            var expectedArgumentException = new ArgumentException("Test Message");

            SetupHttpRequest();
            SetupHttpResponse(expectedProblemDetailsBytes);
            SetupHttpContext();
            SetupRequestDelegate(expectedArgumentException);

            var middleware = new ExceptionHandlerMiddleware(_mockRequestDelegate.Object, _mockLogger.Object, _problemDetailsFactory);
            await middleware.InvokeAsync(_mockHttpContext.Object);

            AssertRequestDelegate();
            AssertHeaders();
            AssertHttpResponse(expectedProblemDetailsBytes);
            AssertLogger(expectedSerializedProblemDetails, expectedArgumentException);
        }

        private void SetupHttpRequest()
        {
            _mockHttpRequest.SetupGet(request => request.Protocol).Returns(Https);
        }

        private void SetupHttpResponse(byte[] expectedProblemDetailsBytes)
        {
            _mockHttpResponse.SetupGet(response => response.Body).Returns(new MemoryStream());
            _mockHttpResponse.SetupGet(response => response.Headers).Returns(_headers);
            _mockHttpResponse.SetupSet(response => response.StatusCode = InternalServerError);
            _mockHttpResponse.SetupGet(response => response.StatusCode).Returns(InternalServerError);

            // WriteAsync is an extension method, so mock the underlying method called.
            _mockHttpResponse.Setup(response => response.Body.WriteAsync(expectedProblemDetailsBytes, 0,
                expectedProblemDetailsBytes.Length, It.IsAny<CancellationToken>()));
        }

        private void SetupHttpContext()
        {
            _mockHttpContext.SetupGet(context => context.Request).Returns(_mockHttpRequest.Object);
            _mockHttpContext.SetupGet(context => context.Response).Returns(_mockHttpResponse.Object);
        }

        private void SetupRequestDelegate(ArgumentException expectedArgumentException) =>
            _mockRequestDelegate.Setup(requestDelegate => requestDelegate.Invoke(_mockHttpContext.Object))
                .ThrowsAsync(expectedArgumentException);

        private void AssertRequestDelegate() =>
            _mockRequestDelegate.Verify(requestDelegate => requestDelegate.Invoke(_mockHttpContext.Object), Times.Once);

        private void AssertHeaders()
        {
            Assert.AreEqual("no-cache", _headers["Cache-Control"].ToString());
            Assert.AreEqual("no-cache", _headers["Pragma"].ToString());
            Assert.AreEqual("-1", _headers["Expires"].ToString());
        }

        private void AssertHttpResponse(byte[] expectedProblemDetailsBytes)
        {
            // 1 for RequestDelegate, 3 for Headers, 1 for StatusCode, 1 for ContentType, 1 for WriteAsync, 1 for LogResponse
            _mockHttpContext.VerifyGet(context => context.Response, Times.Exactly(8));
            _mockHttpResponse.VerifySet(response => response.StatusCode = InternalServerError, Times.Once);
            _mockHttpResponse.VerifySet(response => response.ContentType = "application/problem+json", Times.Once);
            _mockHttpResponse.Verify(response => response.Body.WriteAsync(expectedProblemDetailsBytes, 0, expectedProblemDetailsBytes.Length, It.IsAny<CancellationToken>()), Times.Once);
        }

        private void AssertLogger(string expectedSerializedProblemDetails, ArgumentException expectedArgumentException)
        {
            MockLogger.VerifyLog(_mockLogger, LogLevel.Error, expectedSerializedProblemDetails, "ProblemDetails");
            MockLogger.VerifyLog(_mockLogger, LogLevel.Error, expectedArgumentException, "Uncaught Exception");
            MockLogger.VerifyLog(_mockLogger, LogLevel.Information, Https, "RequestProtocol");
            MockLogger.VerifyLog(_mockLogger, LogLevel.Information, InternalServerError, "StatusCode");
            MockLogger.VerifyLog(_mockLogger, LogLevel.Information, Enum.GetName(typeof(HttpStatusCode), HttpStatusCode.InternalServerError), "StatusCodeName");
            MockLogger.VerifyLog(_mockLogger, LogLevel.Information, "{RequestProtocol} {StatusCode} {StatusCodeName}", "{OriginalFormat}");
        }
    }
}
