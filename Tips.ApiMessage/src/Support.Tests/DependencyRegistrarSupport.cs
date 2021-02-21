using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Support.Tests
{
    public static class DependencyRegistrarSupport
    {
        public static void AddScopedLogger(IServiceCollection serviceCollection) => serviceCollection.AddScoped(typeof(ILogger<>), typeof(FakeLogger<>));

        public static void AssertServiceIsInstanceOfType<TServiceType, TImplementationsType>(IServiceProvider serviceProvider)
        {
            var service = serviceProvider.GetService<TServiceType>();
            Assert.IsInstanceOfType(service, typeof(TImplementationsType));
        }

        public static void AssertServiceIsNotNull<TServiceType>(IServiceProvider serviceProvider)
        {
            var service = serviceProvider.GetService<TServiceType>();
            Assert.IsNotNull(service);
        }
    }
}
