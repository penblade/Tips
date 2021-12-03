using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tips.Support.Tests;
using Tips.Pipeline;
using Tips.Pipeline.Configuration;
using Tips.Pipeline.Logging;

namespace Tips.Pipeline.Tests.Configuration
{
    [TestClass]
    public class DependencyRegistrarTest
    {
        [TestMethod]
        public void RegisterTest()
        {
            // Given that the LoggingBehavior class has a dependency
            // on an ILogger<> implementation, we could build the
            // service provider and verify that the registration
            // was added, but that would enforce an order of events
            // on service registration.  Skip for now.
            var serviceCollection = new ServiceCollection();
            DependencyRegistrarSupport.AddScopedLogger(serviceCollection);
            DependencyRegistrar.Register(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            DependencyRegistrarSupport.AssertServiceIsInstanceOfType<ILogger<LoggingBehavior>, FakeLogger<LoggingBehavior>>(serviceProvider);
            DependencyRegistrarSupport.AssertServiceIsInstanceOfType<IPipelineBehavior, LoggingBehavior>(serviceProvider);
        }
    }
}
