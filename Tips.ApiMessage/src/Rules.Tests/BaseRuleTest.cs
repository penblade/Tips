using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tips.Rules;
using Tips.Support.Tests;

namespace Tips.Rules.Tests
{
    [TestClass]
    public class BaseRuleTest
    {
        private const bool ContinueProcessingIsTrue = true;
        private const bool ContinueProcessingIsFalse = false;

        [TestMethod]
        public async Task ProcessAsyncThrowsException()
        {
            var request = new FakeRequest();
            var response = new FakeResponse();
            var rules = new List<IBaseRule<FakeRequest, FakeResponse>>().ToList();

            var fakeRule = new NotProcessedRule();

            await Verify.ThrowsExceptionAsync(() => fakeRule.ProcessAsync(null, response, rules), new ArgumentNullException(nameof(request)));
            await Verify.ThrowsExceptionAsync(() => fakeRule.ProcessAsync(request, null, rules), new ArgumentNullException(nameof(response)));
            await Verify.ThrowsExceptionAsync(() => fakeRule.ProcessAsync(request, response, null), new ArgumentNullException(nameof(rules)));
        }

        [TestMethod]
        [DynamicData(nameof(SetupProcessAsyncChangesStatus), DynamicDataSourceType.Method)]
        [DynamicData(nameof(SetupProcessAsyncRequiredRules), DynamicDataSourceType.Method)]
        public async Task ProcessAsyncCanChangeStatusAndContinueProcessing(string scenario, bool expectedContinueProcessing,
            RuleStatusType expectedRuleStatusType, FakeRule rule, List<IBaseRule<FakeRequest, FakeResponse>> rules)
        {
            var request = new FakeRequest { ContinueProcessing = expectedContinueProcessing };
            var response = new FakeResponse();

            await rule.ProcessAsync(request, response, rules);

            Assert.AreEqual(expectedContinueProcessing, rule.ContinueProcessing, scenario);
            Assert.AreEqual(expectedRuleStatusType, rule.Status, scenario);
        }

        private static IEnumerable<object[]> SetupProcessAsyncChangesStatus()
        {
            var rules = new List<IBaseRule<FakeRequest, FakeResponse>>().ToList();

            yield return new object[] { "NotProcessed", ContinueProcessingIsTrue, RuleStatusType.NotProcessed, new NotProcessedRule(), rules };
            yield return new object[] { "Skipped", ContinueProcessingIsTrue, RuleStatusType.Skipped, new SkippedRule(), rules };
            yield return new object[] { "Failed", ContinueProcessingIsTrue, RuleStatusType.Failed, new FailedRule(), rules };
            yield return new object[] { "Passed", ContinueProcessingIsTrue, RuleStatusType.Passed, new PassedRule(), rules };

            yield return new object[] { "NotProcessed", ContinueProcessingIsFalse, RuleStatusType.NotProcessed, new NotProcessedRule(), rules };
            yield return new object[] { "Skipped", ContinueProcessingIsFalse, RuleStatusType.Skipped, new SkippedRule(), rules };
            yield return new object[] { "Failed", ContinueProcessingIsFalse, RuleStatusType.Failed, new FailedRule(), rules };
            yield return new object[] { "Passed", ContinueProcessingIsFalse, RuleStatusType.Passed, new PassedRule(), rules };
        }

        private static IEnumerable<object[]> SetupProcessAsyncRequiredRules()
        {
            var requiredRules = new List<IBaseRule<FakeRequest, FakeResponse>> { new RequiredRule1() };
            var rules = new List<IBaseRule<FakeRequest, FakeResponse>> { new NotProcessedRule(), new SkippedRule(), new FailedRule(), new PassedRule() };
            yield return new object[] {"1 required rule, 0 required rules in list - Skipped",
                ContinueProcessingIsTrue, RuleStatusType.Skipped, new RequiresRulesRule(requiredRules), rules};

            requiredRules = new List<IBaseRule<FakeRequest, FakeResponse>> { new RequiredRule1() };
            rules = new List<IBaseRule<FakeRequest, FakeResponse>> { new NotProcessedRule(), new SkippedRule(), new FailedRule(), new PassedRule(), new RequiredRule1() };
            yield return new object[] {"1 required rule, 1 required rule in list - Passed",
                ContinueProcessingIsTrue, RuleStatusType.Passed, new RequiresRulesRule(requiredRules), rules};

            requiredRules = new List<IBaseRule<FakeRequest, FakeResponse>> { new RequiredRule1(), new RequiredRule2() };
            rules = new List<IBaseRule<FakeRequest, FakeResponse>> { new NotProcessedRule(), new SkippedRule(), new FailedRule(), new PassedRule(), new RequiredRule1() };
            yield return new object[] {"2 required rules, 1 required rules in list - Skipped",
                ContinueProcessingIsTrue, RuleStatusType.Skipped, new RequiresRulesRule(requiredRules), rules};

            requiredRules = new List<IBaseRule<FakeRequest, FakeResponse>> { new RequiredRule1(), new RequiredRule2() };
            rules = new List<IBaseRule<FakeRequest, FakeResponse>> { new NotProcessedRule(), new SkippedRule(), new RequiredRule2(), new FailedRule(), new PassedRule(), new RequiredRule1() };
            yield return new object[] {"2 required rules, 2 required rules in list - Passed",
                ContinueProcessingIsTrue, RuleStatusType.Passed, new RequiresRulesRule(requiredRules), rules};

            requiredRules = new List<IBaseRule<FakeRequest, FakeResponse>> { new RequiredRule1(), new RequiredRule2(), new RequiredRule3() };
            rules = new List<IBaseRule<FakeRequest, FakeResponse>> { new NotProcessedRule(), new SkippedRule(), new FailedRule(), new PassedRule(), new RequiredRule1() };
            yield return new object[] {"3 required rules, 3 required rules in list - Skipped",
                ContinueProcessingIsTrue, RuleStatusType.Skipped, new RequiresRulesRule(requiredRules), rules};

            requiredRules = new List<IBaseRule<FakeRequest, FakeResponse>> { new RequiredRule1(), new RequiredRule2(), new RequiredRule3() };
            rules = new List<IBaseRule<FakeRequest, FakeResponse>> { new RequiredRule3(), new NotProcessedRule(), new SkippedRule(), new RequiredRule2(), new FailedRule(), new PassedRule(), new RequiredRule1() };
            yield return new object[] {"3 required rules, 3 required rules in list - Passed",
                ContinueProcessingIsTrue, RuleStatusType.Passed, new RequiresRulesRule(requiredRules), rules};
        }

        private class NotProcessedRule : FakeRule
        {
            protected override Task ProcessRuleAsync(FakeRequest request, FakeResponse response) => DoWork(request);
        }

        private class SkippedRule : FakeRule
        {
            protected override Task ProcessRuleAsync(FakeRequest request, FakeResponse response) => DoWork(request, Skip);
        }

        private class FailedRule : FakeRule
        {
            protected override Task ProcessRuleAsync(FakeRequest request, FakeResponse response) => DoWork(request, Fail);
        }

        private class PassedRule : FakeRule
        {
            protected override Task ProcessRuleAsync(FakeRequest request, FakeResponse response) => DoWork(request, Pass);
        }

        private class RequiresRulesRule : PassedRule
        {
            public RequiresRulesRule(IEnumerable<IBaseRule<FakeRequest, FakeResponse>> rules)
            {
                RequiredRules.AddRange(rules.Select(rule => rule.GetType()));
            }
        }
        private class RequiredRule1 : PassedRule { public RequiredRule1() => Pass(); }
        private class RequiredRule2 : PassedRule { public RequiredRule2() => Pass(); }
        private class RequiredRule3 : PassedRule { public RequiredRule3() => Pass(); }

        public abstract class FakeRule : BaseRule<FakeRequest, FakeResponse>
        {
            protected Task DoWork(FakeRequest request, Action method = null)
            {
                ContinueProcessing = request.ContinueProcessing;
                method?.Invoke();
                return Task.CompletedTask;
            }
        }

        public class FakeRequest
        {
            public bool ContinueProcessing { get; set; } = true;
        }
        public class FakeResponse { }
    }
}
