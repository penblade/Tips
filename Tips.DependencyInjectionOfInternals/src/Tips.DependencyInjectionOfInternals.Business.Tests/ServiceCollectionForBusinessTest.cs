using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");
            var configuration = configurationBuilder.Build();

            var serviceCollection = new ServiceCollection();

            var serviceCollectionForBusiness = new ServiceCollectionForBusiness(configuration, configurationBuilder);
            serviceCollectionForBusiness.RegisterDependencies(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [TestMethod]
        public void VerifyRegisterDependenciesForBusiness()
        {
            VerifyRegisterByConvention();
            VerifyRegisterBusinessConfiguration();
            VerifyRegisterDependencyConfiguration();
        }

        private void VerifyRegisterByConvention()
        {
            Assert.IsInstanceOfType(_serviceProvider.GetService<IBusinessService>(), typeof(BusinessService));
            Assert.IsInstanceOfType(_serviceProvider.GetService<ICommandFactory>(), typeof(CommandFactory));
        }

        private void VerifyRegisterBusinessConfiguration()
        {
            var expectedBusinessConfiguration = new BusinessConfiguration
            {
                ConnectionString = "Super Secret Database Connection String that should be hidden by managing User Secrets.",
                DocumentPath = "A path to server storage.",
                IocFiles = new List<string> { "dependencyConfiguration.json" }
            };
            var actualBusinessConfiguration = _serviceProvider.GetServices<BusinessConfiguration>().Single();
            Assert.AreEqual(expectedBusinessConfiguration.ConnectionString, actualBusinessConfiguration.ConnectionString);
            Assert.AreEqual(expectedBusinessConfiguration.DocumentPath, actualBusinessConfiguration.DocumentPath);
            Assert.AreEqual(expectedBusinessConfiguration.IocFiles.Single(), actualBusinessConfiguration.IocFiles.Single());
        }

        private void VerifyRegisterDependencyConfiguration()
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
