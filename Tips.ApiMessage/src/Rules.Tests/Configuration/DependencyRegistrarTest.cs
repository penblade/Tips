using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tips.Support.Tests;
using Tips.Rules;
using Tips.Rules.Configuration;

namespace Tips.Rules.Tests.Configuration
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
            VerifyDependencyRegistrar(serviceProvider);
        }

        public static void VerifyDependencyRegistrar(ServiceProvider serviceProvider)
        {
            DependencyRegistrarSupport.AssertServiceIsInstanceOfType<IRulesEngine, RulesEngine>(serviceProvider);
            DependencyRegistrarSupport.AssertServiceIsInstanceOfType<IRulesFactory<string, int>, RulesFactory<string, int>>(serviceProvider);
        }
    }
}
