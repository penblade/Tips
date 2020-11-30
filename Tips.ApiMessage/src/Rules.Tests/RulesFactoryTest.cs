using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tips.Rules;

namespace Rules.Tests
{
    [TestClass]
    public class RulesFactoryTest
    {
        [TestMethod]
        [DataRow(new[] { 1, 2, 3 })]
        [DataRow(new[] { 3, 1, 2 })]
        [DataRow(new[] { 2, 3, 1 })]
        public void CreateTest(int[] expectedRuleIds)
        {
            var services = new ServiceCollection();
            var expectedRules = CreateFakeRules(expectedRuleIds).ToList();
            var count = expectedRules.Count();

            for (var i = 0; i < count; i++)
            {
                services.AddScoped(typeof(BaseRule<FakeRequest, FakeResponse>), expectedRules[i].GetType());
            }
            
            var serviceProvider = services.BuildServiceProvider();
            var factory = new RulesFactory<FakeRequest, FakeResponse>(serviceProvider);
            var actualRules = factory.Create().ToList();

            for (var i = 0; i < count; i++)
            {
                Assert.IsInstanceOfType(actualRules[i], expectedRules[i].GetType());
            }
        }

        private static IEnumerable<BaseRule<FakeRequest, FakeResponse>> CreateFakeRules(int[] ruleIds)
        {
            return ruleIds.Select(ruleId => (BaseRule<FakeRequest, FakeResponse>) (ruleId switch
                {
                    1 => new FakeRule1<FakeRequest, FakeResponse>(),
                    2 => new FakeRule2<FakeRequest, FakeResponse>(),
                    3 => new FakeRule3<FakeRequest, FakeResponse>(),
                    _ => throw new ArgumentOutOfRangeException(nameof(ruleIds))
                }))
                .ToList();
        }

        public class FakeRequest {}

        public class FakeResponse {}

        private class FakeRule1<TRequest, TResponse> : BaseRule<TRequest, TResponse>
        {
            protected override Task ProcessRuleAsync(TRequest request, TResponse response) => Task.CompletedTask;
        }

        private class FakeRule2<TRequest, TResponse> : BaseRule<TRequest, TResponse>
        {
            protected override Task ProcessRuleAsync(TRequest request, TResponse response) => Task.CompletedTask;
        }

        private class FakeRule3<TRequest, TResponse> : BaseRule<TRequest, TResponse>
        {
            protected override Task ProcessRuleAsync(TRequest request, TResponse response) => Task.CompletedTask;
        }
    }
}
