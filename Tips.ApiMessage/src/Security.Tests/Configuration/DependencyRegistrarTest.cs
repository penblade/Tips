using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Support.Tests;
using Tips.Security;
using Tips.Security.Configuration;

namespace Security.Tests.Configuration
{
    [TestClass]
    public class DependencyRegistrarTest
    {
        [TestMethod]
        public void RegisterTest()
        {
            var configurationRootFromJson = new ConfigurationBuilder().AddJsonFile(@"Configuration\appsettings.test.json").Build();

            var serviceCollection = new ServiceCollection();
            DependencyRegistrar.Register(serviceCollection, configurationRootFromJson);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            VerifyDependencyRegistrar(serviceProvider);
        }

        public static void VerifyDependencyRegistrar(ServiceProvider serviceProvider)
        {
            DependencyRegistrarSupport.AssertServiceIsInstanceOfType<ApiKeyConfiguration, ApiKeyConfiguration>(serviceProvider);
            DependencyRegistrarSupport.AssertServiceIsInstanceOfType<IApiKeyRepository, ApiKeyRepository>(serviceProvider);
            AssertApiKeyConfiguration(serviceProvider);
        }

        private static void AssertApiKeyConfiguration(IServiceProvider serviceProvider)
        {
            var expectedConfiguration = CreateExpectedApiKeyConfiguration();

            var actualConfiguration = serviceProvider.GetService<ApiKeyConfiguration>();
            Assert.AreEqual(expectedConfiguration.ApiHeader, actualConfiguration.ApiHeader);

            var expectedApiKeys = expectedConfiguration.ApiKeys.ToList();
            var actualApiKeys = actualConfiguration.ApiKeys.ToList();
            for (var i = 0; i < 2; i++)
            {
                AssertApiKey(expectedApiKeys[i], actualApiKeys[i]);
            }
        }

        private static void AssertApiKey(ApiKey expectedApiKey, ApiKey actualApiKey)
        {
            Assert.AreEqual(expectedApiKey.Id, actualApiKey.Id);
            Assert.AreEqual(expectedApiKey.Owner, actualApiKey.Owner);
            Assert.AreEqual(expectedApiKey.Key, actualApiKey.Key);
            Assert.AreEqual(expectedApiKey.Created, actualApiKey.Created);
        }

        private static ApiKeyConfiguration CreateExpectedApiKeyConfiguration() =>
        new()
        {
            ApiHeader = "TestApiHeader",
            ApiKeys = new[]
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
