using System;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Moq;
using Tips.Pipeline;

namespace Tips.Support.Tests
{
    public static class MockLoggerVerifyLogRequestResponseExtensions
    {
        private const string Post = "POST";
        private const string Https = "https";
        private const string SchemeDelimiter = "://";
        private static readonly HostString Host = new("www.example.com", 8080);
        private static readonly PathString PathBase = new("/api");
        private static readonly PathString Path = new("/some/path/to/file");
        private static readonly QueryString QueryString = new("?param1=value1&param2=value2&param3=value3");

        private static readonly IPAddress RemoteIpAddress = IPAddress.Parse("127.0.0.1");

        public const int InternalServerError = (int)HttpStatusCode.InternalServerError;

        public static Mock<HttpRequest> SetupHttpRequest<TCategory>(this Mock<ILogger<TCategory>> mockLogger)
        {
            var mockHttpRequest = new Mock<HttpRequest>();

            SetupHttpRequestForLogRequest<TCategory>(mockHttpRequest);
            SetupHttpRequestForLogResponse<TCategory>(mockHttpRequest);
            return mockHttpRequest;
        }

        private static void SetupHttpRequestForLogRequest<TCategory>(Mock<HttpRequest> mockHttpRequest)
        {
            mockHttpRequest.SetupGet(request => request.Method).Returns(Post);
            mockHttpRequest.SetupGet(request => request.Scheme).Returns(Https);
            mockHttpRequest.SetupGet(request => request.Host).Returns(Host);
            mockHttpRequest.SetupGet(request => request.PathBase).Returns(PathBase);
            mockHttpRequest.SetupGet(request => request.Path).Returns(Path);
            mockHttpRequest.SetupGet(request => request.QueryString).Returns(QueryString);
        }

        private static void SetupHttpRequestForLogResponse<TCategory>(Mock<HttpRequest> mockHttpRequest) =>
            mockHttpRequest.SetupGet(request => request.Protocol).Returns(Https);

        public static Mock<HttpResponse> SetupHttpResponse<TCategory>(this Mock<ILogger<TCategory>> mockLogger)
        {
            var mockHttpResponse = new Mock<HttpResponse>();
            mockHttpResponse.SetupGet(response => response.Body).Returns(new MemoryStream());
            mockHttpResponse.SetupSet(response => response.StatusCode = InternalServerError);
            mockHttpResponse.SetupGet(response => response.StatusCode).Returns(InternalServerError);
            return mockHttpResponse;
        }

        public static Mock<HttpContext> SetupHttpContext<TCategory>(this Mock<ILogger<TCategory>> mockLogger,
            Mock<HttpRequest> mockHttpRequest, Mock<HttpResponse> mockHttpResponse, Mock<ConnectionInfo> mockConnectionInfo)
        {
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.SetupGet(context => context.Request).Returns(mockHttpRequest.Object);
            mockHttpContext.SetupGet(context => context.Response).Returns(mockHttpResponse.Object);

            mockHttpContext.SetupGet(context => context.Connection).Returns(mockConnectionInfo.Object);
            return mockHttpContext;
        }

        public static Mock<ConnectionInfo> SetupConnectionInfo<TCategory>(this Mock<ILogger<TCategory>> mockLogger)
        {
            var mockConnectionInfo = new Mock<ConnectionInfo>();
            mockConnectionInfo.SetupGet(connectionInfo => connectionInfo.RemoteIpAddress).Returns(RemoteIpAddress);
            return mockConnectionInfo;
        }

        public static void VerifyLogRequest<TCategory>(this Mock<ILogger<TCategory>> mockLogger, Mock<HttpContext> mockHttpContext, string expectedApiKeyOwner)
        {
            mockLogger.VerifyLog(LogLevel.Information, mockHttpContext.Object.Request?.Method, "RequestMethod");
            mockLogger.VerifyLog(LogLevel.Information, mockHttpContext.Object.Request?.GetDisplayUrl(), "Url");
            mockLogger.VerifyLog(LogLevel.Information, expectedApiKeyOwner, "ApiKeyOwner");
            mockLogger.VerifyLog(LogLevel.Information, mockHttpContext.Object.Connection.RemoteIpAddress, "RemoteIpAddress");
            mockLogger.VerifyLog(LogLevel.Information, Tracking.TraceParentId, "TraceParentId");
            mockLogger.VerifyLog(LogLevel.Information, Tracking.TraceParentStateString, "TraceParentStateString");

            mockLogger.VerifyLog(LogLevel.Information, "{RequestMethod} {Url} | ApiKey.Owner: {ApiKeyOwner} | Remote IP Address: {RemoteIpAddress} | TraceParentId: {TraceParentId} | TraceParentStateString: {TraceParentStateString}", "{OriginalFormat}");
        }

        public static void VerifyLogResponse<TCategory>(this Mock<ILogger<TCategory>> mockLogger)
        {
            mockLogger.VerifyLog(LogLevel.Information, Https, "RequestProtocol");
            mockLogger.VerifyLog(LogLevel.Information, InternalServerError, "StatusCode");
            mockLogger.VerifyLog(LogLevel.Information, Enum.GetName(typeof(HttpStatusCode), HttpStatusCode.InternalServerError), "StatusCodeName");
            mockLogger.VerifyLog(LogLevel.Information, "{RequestProtocol} {StatusCode} {StatusCodeName}", "{OriginalFormat}");
        }
    }
}
