using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tips.Rules;
using Tips.Rules.Configuration;

namespace Rules.Tests.Configuration
{
    [TestClass]
    public class DependencyRegistrarTest
    {
        [TestMethod]
        public void RegisterTest()
        {
            var serviceCollection = new ServiceCollection();
            DependencyRegistrar.Register(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            AssertType<IRulesEngine, RulesEngine>(serviceProvider);
            AssertType<IRulesFactory<string, int>, RulesFactory<string, int>>(serviceProvider);
        }

        private static void AssertType<TExpected, TActual>(IServiceProvider serviceProvider)
        {
            var service = serviceProvider.GetService<TExpected>();
            Assert.IsInstanceOfType(service, typeof(TActual));
        }
    }
}
