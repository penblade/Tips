using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tips.Pipeline;
using Tips.Pipeline.Configuration;
using Tips.Pipeline.Logging;

namespace Pipeline.Tests.Configuration
{
    [TestClass]
    public class DependencyRegistrarTest
    {
        [TestMethod]
        public void RegisterTest()
        {
            var serviceCollection = new ServiceCollection();

            // Given that the LoggingBehavior class has a dependency
            // on an ILogger<> implementation, we could build the
            // service provider and verify that the registration
            // was added, but that would enforce an order of events
            // on service registration.  Skip for now.
            serviceCollection.AddScoped(typeof(ILogger<>), typeof(FakeLogger<>));

            DependencyRegistrar.Register(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            AssertType<ILogger<LoggingBehavior>, FakeLogger<LoggingBehavior>>(serviceProvider);
            AssertType<IPipelineBehavior, LoggingBehavior>(serviceProvider);
        }

        private static void AssertType<TServiceType, TImplementationsType>(IServiceProvider serviceProvider)
        {
            var service = serviceProvider.GetService<TServiceType>();
            Assert.IsInstanceOfType(service, typeof(TImplementationsType));
        }

        private class FakeLogger<TCategoryName> : ILogger<TCategoryName>
        {
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
                => throw new NotImplementedException();

            public bool IsEnabled(LogLevel logLevel) => throw new NotImplementedException();

            public IDisposable BeginScope<TState>(TState state) => throw new NotImplementedException();
        }
    }
}
