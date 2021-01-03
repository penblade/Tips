using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Support.Tests;
using Tips.Security;

namespace Security.Tests
{
    [TestClass]
    public class ApiKeyHandlerMiddlewareTest
    {
        private readonly Mock<HttpResponse> _mockHttpResponse = new();
        private readonly Mock<HttpContext> _mockHttpContext = new();
        private readonly Mock<IApiKeyRepository> _mockApiKeyRepository = new();
        private readonly Mock<RequestDelegate> _mockRequestDelegate = new();

        [TestMethod]
        public async Task InvokeApiKeyFoundTest()
        {
            SetupMocks(new List<ApiKey> { new() });

            var middleware = new ApiKeyHandlerMiddleware(_mockRequestDelegate.Object, _mockApiKeyRepository.Object);
            await middleware.Invoke(_mockHttpContext.Object);

            VerifyMocks(Times.Once(), Times.Once(), Times.Never(), Times.Never());
        }

        [TestMethod]
        public async Task InvokeApiKeyNotFoundTest()
        {
            SetupMocks(new List<ApiKey>());

            var middleware = new ApiKeyHandlerMiddleware(_mockRequestDelegate.Object, _mockApiKeyRepository.Object);
            await middleware.Invoke(_mockHttpContext.Object);

            VerifyMocks(Times.Once(), Times.Never(), Times.Once(), Times.Once());
        }

        [TestMethod]
        public async Task InvokeApiKeyHasDuplicatesTest()
        {
            const string testOwner1 = "TestOwner1";
            const string testOwner2 = "TestOwner2";

            SetupMocks(new List<ApiKey> { new() { Owner = testOwner1 }, new() { Owner = testOwner2 } });

            var middleware = new ApiKeyHandlerMiddleware(_mockRequestDelegate.Object, _mockApiKeyRepository.Object);

            var errorMessage = $"The following ApiKey's share the same id.  Fix immediately!!! | {testOwner1}, {testOwner2}";
            var expectedException = new InvalidOperationException(errorMessage);

            await Verify.ThrowsExceptionAsync(() => middleware.Invoke(_mockHttpContext.Object), expectedException);

            VerifyMocks(Times.Once(), Times.Never(), Times.Never(), Times.Never());
        }

        private void SetupMocks(IEnumerable<ApiKey> apiKeys)
        {
            _mockHttpResponse.SetupSet(response => response.StatusCode = (int) HttpStatusCode.Unauthorized);
            _mockHttpContext.SetupGet(context => context.Response).Returns(_mockHttpResponse.Object);
            _mockApiKeyRepository.Setup(repository => repository.GetApiKeysFromHeaders(_mockHttpContext.Object)).Returns(apiKeys);
        }

        private void VerifyMocks(Times timesApiKeyRepositoryCalled, Times timesRequestDelegateCalled,
            Times timesHttpContextCalled, Times timesHttpResponseCalled)
        {
            _mockApiKeyRepository.Verify(repository => repository.GetApiKeysFromHeaders(_mockHttpContext.Object), timesApiKeyRepositoryCalled);
            _mockRequestDelegate.Verify(requestDelegate => requestDelegate.Invoke(_mockHttpContext.Object), timesRequestDelegateCalled);
            _mockHttpContext.VerifyGet(context => context.Response, timesHttpContextCalled);
            _mockHttpResponse.VerifySet(response => response.StatusCode = (int)HttpStatusCode.Unauthorized, timesHttpResponseCalled);
        }
    }
}
