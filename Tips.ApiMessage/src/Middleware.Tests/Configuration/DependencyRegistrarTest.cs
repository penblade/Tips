using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tips.Middleware.Configuration;
using Tips.Middleware.ErrorHandling;

namespace Middleware.Tests.Configuration
{
    [TestClass]
    public class DependencyRegistrarTest
    {
        [TestMethod]
        public void RegisterTest()
        {
            var configurationRootFromJson = new ConfigurationBuilder().AddJsonFile(@"Configuration\appsettings.json").Build();

            var serviceCollection = new ServiceCollection();
            DependencyRegistrar.Register(serviceCollection, configurationRootFromJson);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            AssertType<ProblemDetailsConfiguration, ProblemDetailsConfiguration>(serviceProvider);
            AssertType<IProblemDetailsFactory, ProblemDetailsFactory>(serviceProvider);
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

        private static void AssertType<TExpected, TActual>(IServiceProvider serviceProvider)
        {
            var service = serviceProvider.GetService<TExpected>();
            Assert.IsInstanceOfType(service, typeof(TActual));
        }
    }
}
