using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tips.Middleware.Configuration;
using Tips.Middleware.ErrorHandling;
using Tips.Support.Tests;

namespace Tips.Middleware.Tests.Configuration
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

            DependencyRegistrarSupport.AssertServiceIsInstanceOfType<ProblemDetailsConfiguration, ProblemDetailsConfiguration>(serviceProvider);
            DependencyRegistrarSupport.AssertServiceIsInstanceOfType<IProblemDetailsFactory, ProblemDetailsFactory>(serviceProvider);
            AssertConfiguration(serviceProvider);
        }

        private static void AssertConfiguration(IServiceProvider serviceProvider)
        {
            var expectedConfiguration = CreateProblemDetailsConfiguration();

            var actualConfiguration = serviceProvider.GetService<ProblemDetailsConfiguration>();
            Assert.AreEqual(expectedConfiguration?.UrnName, actualConfiguration?.UrnName);
        }

        private static ProblemDetailsConfiguration CreateProblemDetailsConfiguration() =>
            new()
            {
                UrnName = "TestUrnName"
            };
    }
}
