using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Tips.Security;

namespace Security.Tests
{
    [TestClass]
    public class ApiRepositoryTest
    {
        private const bool TryGetApiKeyIdFromHeadersReturnsTrue = true;
        private const bool TryGetApiKeyIdFromHeadersReturnsFalse = false;
        [TestMethod]
        [DataRow(null, TryGetApiKeyIdFromHeadersReturnsTrue)]
        [DataRow(0, TryGetApiKeyIdFromHeadersReturnsTrue)]
        [DataRow(1, TryGetApiKeyIdFromHeadersReturnsTrue)]
        [DataRow(null, TryGetApiKeyIdFromHeadersReturnsFalse)]
        [DataRow(0, TryGetApiKeyIdFromHeadersReturnsFalse)]
        [DataRow(1, TryGetApiKeyIdFromHeadersReturnsFalse)]
        public void GetApiKeyFromHeadersTest(int? expectedApiKeyId, bool tryGetApiKeyIdFromHeadersReturns)
        {
            var expectedApiKeyConfiguration = CreateExpectedApiKeyConfiguration();
            var expectedApiKey = GetExpectedApiKey(expectedApiKeyConfiguration, expectedApiKeyId);
            var expectedApiKeyIdInHeaders = GetExpectedApiKeyId(expectedApiKey);

            var mockHttpContext = SetupMockHttpContext(expectedApiKeyConfiguration, expectedApiKeyIdInHeaders, tryGetApiKeyIdFromHeadersReturns);

            var apiRepository = new ApiRepository(expectedApiKeyConfiguration);
            var actualApiKey = apiRepository.GetApiKeyFromHeaders(mockHttpContext.Object);

            if (expectedApiKeyId != null && tryGetApiKeyIdFromHeadersReturns)
            {
                AssertApiKey(expectedApiKey, actualApiKey);
            }
            else
            {
                Assert.IsNull(actualApiKey);
            }

            mockHttpContext.Verify(context => context.Request.Headers.TryGetValue(expectedApiKeyConfiguration.ApiHeader, out expectedApiKeyIdInHeaders), Times.Once);
        }

        [TestMethod]
        [DataRow(null, TryGetApiKeyIdFromHeadersReturnsTrue)]
        [DataRow(0, TryGetApiKeyIdFromHeadersReturnsTrue)]
        [DataRow(1, TryGetApiKeyIdFromHeadersReturnsTrue)]
        [DataRow(null, TryGetApiKeyIdFromHeadersReturnsFalse)]
        [DataRow(0, TryGetApiKeyIdFromHeadersReturnsFalse)]
        [DataRow(1, TryGetApiKeyIdFromHeadersReturnsFalse)]
        public void GetApiKeysFromHeadersTest(int? expectedApiKeyId, bool tryGetApiKeyIdFromHeadersReturns)
        {
            var expectedApiKeyConfiguration = CreateExpectedApiKeyConfiguration();
            var expectedApiKey = GetExpectedApiKey(expectedApiKeyConfiguration, expectedApiKeyId);
            var expectedApiKeyIdInHeaders = GetExpectedApiKeyId(expectedApiKey);

            var mockHttpContext = SetupMockHttpContext(expectedApiKeyConfiguration, expectedApiKeyIdInHeaders, tryGetApiKeyIdFromHeadersReturns);

            var apiRepository = new ApiRepository(expectedApiKeyConfiguration);
            var actualApiKeys = apiRepository.GetApiKeysFromHeaders(mockHttpContext.Object).ToList();

            if (expectedApiKeyId != null && tryGetApiKeyIdFromHeadersReturns)
            {
                Assert.AreEqual(1, actualApiKeys.Count);
                AssertApiKey(expectedApiKey, actualApiKeys.First());
            }
            else
            {
                Assert.AreEqual(0, actualApiKeys.Count);
            }

            mockHttpContext.Verify(context => context.Request.Headers.TryGetValue(expectedApiKeyConfiguration.ApiHeader, out expectedApiKeyIdInHeaders), Times.Once);
        }

        private static Mock<HttpContext> SetupMockHttpContext(ApiKeyConfiguration expectedApiKeyConfiguration, StringValues expectedApiKeyIdInHeaders,
            bool tryGetApiKeyIdFromHeadersReturns)
        {
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(context => context.Request.Headers.TryGetValue(expectedApiKeyConfiguration.ApiHeader, out expectedApiKeyIdInHeaders))
                .Returns(tryGetApiKeyIdFromHeadersReturns);
            return mockHttpContext;
        }

        private static StringValues GetExpectedApiKeyId(ApiKey expectedApiKey) => new StringValues(expectedApiKey?.Key);

        private static ApiKey GetExpectedApiKey(ApiKeyConfiguration expectedApiKeyConfiguration, int? expectedApiKeyId) =>
            expectedApiKeyId != null ? expectedApiKeyConfiguration.ApiKeys.ToList()[(int) expectedApiKeyId] : null;

        private static void AssertApiKey(ApiKey expectedApiKey, ApiKey actualApiKey)
        {
            Assert.AreEqual(expectedApiKey.Id, actualApiKey.Id);
            Assert.AreEqual(expectedApiKey.Owner, actualApiKey.Owner);
            Assert.AreEqual(expectedApiKey.Key, actualApiKey.Key);
            Assert.AreEqual(expectedApiKey.Created, actualApiKey.Created);
        }

        private static ApiKeyConfiguration CreateExpectedApiKeyConfiguration() =>
            new ApiKeyConfiguration
            {
                ApiHeader = "TestApiHeader",
                ApiKeys = new ApiKey[]
                {
                    new ApiKey
                    {
                        Id = 1,
                        Owner = "TestOwner1",
                        Key = "TestKey1",
                        Created = new DateTime(2020, 03, 15)
                    },
                    new ApiKey
                    {
                        Id = 2,
                        Owner = "TestOwner2",
                        Key = "TestKey2",
                        Created = new DateTime(2020, 04, 15)
                    }
                }
            };
    }
}
