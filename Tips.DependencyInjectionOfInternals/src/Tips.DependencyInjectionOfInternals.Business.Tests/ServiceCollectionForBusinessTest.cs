using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Tips.DependencyInjectionOfInternals.Business.Commands;
using Tips.DependencyInjectionOfInternals.Business.Configuration;

namespace Tips.DependencyInjectionOfInternals.Business.Tests
{
    [TestClass]
    public class ServiceCollectionForBusinessTest
    {
        private readonly ServiceProvider _serviceProvider;

        // Only use [ClassInitialize] when the class properties
        // should only be initalized once for all tests.

        // If the test methods change the properties,
        // use [TestInitialize] or add a public constructor.

        // If you're not sure, just use the unit test's
        // public constructor.
        public ServiceCollectionForBusinessTest()
        {
            // The mock configuration must setup all of the
            // expected properties or an exception is thrown.

            // System.ArgumentNullException: Value cannot be null.
            // Value cannot be null.\r\nParameter name: configuration

            var mockConfigurationSection = new Mock<IConfigurationSection>();
            mockConfigurationSection.SetupGet(x => x[It.IsAny<string>()]).Returns("expected configuration value");

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(mockConfigurationSection.Object);

            var serviceCollection = new ServiceCollection();

            var serviceCollectionForBusiness = new ServiceCollectionForBusiness();

            serviceCollectionForBusiness.RegisterDependencies(mockConfiguration.Object, serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [TestMethod]
        public void VerifyRegisterDependenciesForBusiness()
        {
            Assert.IsInstanceOfType(_serviceProvider.GetService<IBusinessService>(), typeof(BusinessService));
            Assert.IsInstanceOfType(_serviceProvider.GetService<ICommandFactory>(), typeof(CommandFactory));
            AssertCommandsWereRegistered();
        }

        private void AssertCommandsWereRegistered()
        {
            var expectedCommands = new List<Type> { typeof(CommandB), typeof(CommandA), typeof(CommandC) };

            var actualCommands = _serviceProvider.GetServices<ICommand>().ToList();

            Assert.IsNotNull(actualCommands);
            Assert.AreEqual(expectedCommands.Count, actualCommands.Count);

            for (var i = 0; i < actualCommands.Count; i++)
            {
                Assert.IsInstanceOfType(actualCommands[i], expectedCommands[i]);
            }
        }
    }
}
