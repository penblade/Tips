using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Tips.Rules;

namespace Rules.Tests
{
    [TestClass]
    public class RulesEngineTest
    {
        private const bool ContinueProcessingIsTrue = true;
        private const bool ContinueProcessingIsFalse = false;

        [TestMethod]
        [DynamicData(nameof(SetupProcessAsyncNotProcessedTest), DynamicDataSourceType.Method)]
        [DynamicData(nameof(SetupProcessAsyncProcessedTest), DynamicDataSourceType.Method)]
        [DynamicData(nameof(SetupProcessAsyncSecondRuleAlreadyProcessedTest), DynamicDataSourceType.Method)]
        public async Task ProcessAsyncTest(string scenario, IList<RuleScenario> ruleScenarios)
        {
            var fakeRequest = new FakeRequest();
            var fakeResponse = new FakeResponse();

            var mockRules = ruleScenarios.Select(ruleScenario =>
                CreateMockRule(fakeRequest, fakeResponse, ruleScenario.RuleStatusType, ruleScenario.ContinueProcessing)).ToList();

            var rulesEngine = new RulesEngine();
            var rules = mockRules.Select(mockRule => mockRule.Object).ToList();
            await rulesEngine.ProcessAsync(fakeRequest, fakeResponse, rules);

            for (var i = 0; i < rules.Count; i++)
            {
                VerifyMockRule(mockRules[i], fakeRequest, fakeResponse, ruleScenarios[i].ExpectedTimes);
            }
        }

        private static IEnumerable<object[]> SetupProcessAsyncNotProcessedTest()
        {
            yield return new object[]
            {
                "NotProcessedTest: NP/CPT/1, NP/CPT/1, NP/CPT/1",
                new List<RuleScenario>
                {
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsTrue, Times.Once()),
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsTrue, Times.Once()),
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsTrue, Times.Once())
                }
            };
            yield return new object[]
            {
                "NotProcessedTest: NP/CPT/1, NP/CPT/1, NP/CPF/1",
                new List<RuleScenario>
                {
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsTrue, Times.Once()),
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsTrue, Times.Once()),
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsFalse, Times.Once())
                }
            };
            yield return new object[]
            {
                "NotProcessedTest: NP/CPT/1, NP/CPF/1, NP/CPT/0",
                new List<RuleScenario>
                {
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsTrue, Times.Once()),
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsFalse, Times.Once()),
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsTrue, Times.Never())
                }
            };
            yield return new object[]
            {
                "NotProcessedTest: NP/CPF/1, NP/CPT/0, NP/CPT/0",
                new List<RuleScenario>
                {
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsFalse, Times.Once()),
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsTrue, Times.Never()),
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsTrue, Times.Never())
                }
            };
            yield return new object[]
            {
                "NotProcessedTest: NP/CPT/1, NP/CPF/1, NP/CPF/0",
                new List<RuleScenario>
                {
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsTrue, Times.Once()),
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsFalse, Times.Once()),
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsFalse, Times.Never())
                }
            };
            yield return new object[]
            {
                "NotProcessedTest: NP/CPF/1, NP/CPT/0, NP/CPF/0",
                new List<RuleScenario>
                {
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsFalse, Times.Once()),
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsTrue, Times.Never()),
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsFalse, Times.Never())
                }
            };
            yield return new object[]
            {
                "NotProcessedTest: NP/CPF/1, NP/CPF/0, NP/CPT/0",
                new List<RuleScenario>
                {
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsFalse, Times.Once()),
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsFalse, Times.Never()),
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsTrue, Times.Never())
                }
            };
            yield return new object[]
            {
                "NotProcessedTest: NP/CPF/1, NP/CPF/0, NP/CPF/0",
                new List<RuleScenario>
                {
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsFalse, Times.Once()),
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsFalse, Times.Never()),
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsFalse, Times.Never())
                }
            };
        }

        private static IEnumerable<object[]> SetupProcessAsyncProcessedTest()
        {
            yield return new object[]
            {
                "ProcessedTest: P/CPT/0, P/CPT/0, P/CPT/0",
                new List<RuleScenario>
                {
                    CreateRuleScenario(RuleStatusType.Passed, ContinueProcessingIsTrue, Times.Never()),
                    CreateRuleScenario(RuleStatusType.Passed, ContinueProcessingIsTrue, Times.Never()),
                    CreateRuleScenario(RuleStatusType.Passed, ContinueProcessingIsTrue, Times.Never())
                }
            };
            yield return new object[]
            {
                "ProcessedTest: P/CPT/0, P/CPT/0, P/CPF/0",
                new List<RuleScenario>
                {
                    CreateRuleScenario(RuleStatusType.Passed, ContinueProcessingIsTrue, Times.Never()),
                    CreateRuleScenario(RuleStatusType.Passed, ContinueProcessingIsTrue, Times.Never()),
                    CreateRuleScenario(RuleStatusType.Passed, ContinueProcessingIsFalse, Times.Never())
                }
            };
            yield return new object[]
            {
                "ProcessedTest: P/CPT/0, P/CPF/0, P/CPT/0",
                new List<RuleScenario>
                {
                    CreateRuleScenario(RuleStatusType.Passed, ContinueProcessingIsTrue, Times.Never()),
                    CreateRuleScenario(RuleStatusType.Passed, ContinueProcessingIsFalse, Times.Never()),
                    CreateRuleScenario(RuleStatusType.Passed, ContinueProcessingIsTrue, Times.Never())
                }
            };
            yield return new object[]
            {
                "ProcessedTest: P/CPF/0, P/CPT/0, P/CPT/0",
                new List<RuleScenario>
                {
                    CreateRuleScenario(RuleStatusType.Passed, ContinueProcessingIsFalse, Times.Never()),
                    CreateRuleScenario(RuleStatusType.Passed, ContinueProcessingIsTrue, Times.Never()),
                    CreateRuleScenario(RuleStatusType.Passed, ContinueProcessingIsTrue, Times.Never())
                }
            };
            yield return new object[]
            {
                "ProcessedTest: P/CPT/0, P/CPF/0, P/CPF/0",
                new List<RuleScenario>
                {
                    CreateRuleScenario(RuleStatusType.Passed, ContinueProcessingIsTrue, Times.Never()),
                    CreateRuleScenario(RuleStatusType.Passed, ContinueProcessingIsFalse, Times.Never()),
                    CreateRuleScenario(RuleStatusType.Passed, ContinueProcessingIsFalse, Times.Never())
                }
            };
            yield return new object[]
            {
                "ProcessedTest: P/CPF/0, P/CPT/0, P/CPF/0",
                new List<RuleScenario>
                {
                    CreateRuleScenario(RuleStatusType.Passed, ContinueProcessingIsFalse, Times.Never()),
                    CreateRuleScenario(RuleStatusType.Passed, ContinueProcessingIsTrue, Times.Never()),
                    CreateRuleScenario(RuleStatusType.Passed, ContinueProcessingIsFalse, Times.Never())
                }
            };
            yield return new object[]
            {
                "ProcessedTest: P/CPF/0, P/CPF/0, P/CPT/0",
                new List<RuleScenario>
                {
                    CreateRuleScenario(RuleStatusType.Passed, ContinueProcessingIsFalse, Times.Never()),
                    CreateRuleScenario(RuleStatusType.Passed, ContinueProcessingIsFalse, Times.Never()),
                    CreateRuleScenario(RuleStatusType.Passed, ContinueProcessingIsTrue, Times.Never())
                }
            };
            yield return new object[]
            {
                "ProcessedTest: P/CPF/0, P/CPF/0, P/CPF/0",
                new List<RuleScenario>
                {
                    CreateRuleScenario(RuleStatusType.Passed, ContinueProcessingIsFalse, Times.Never()),
                    CreateRuleScenario(RuleStatusType.Passed, ContinueProcessingIsFalse, Times.Never()),
                    CreateRuleScenario(RuleStatusType.Passed, ContinueProcessingIsFalse, Times.Never())
                }
            };
        }

        private static IEnumerable<object[]> SetupProcessAsyncSecondRuleAlreadyProcessedTest()
        {
            yield return new object[]
            {
                "SecondRuleAlreadyProcessedTest: NP/CPT/1, P/CPT/0, NP/CPT/1",
                new List<RuleScenario>
                {
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsTrue, Times.Once()),
                    CreateRuleScenario(RuleStatusType.Passed, ContinueProcessingIsTrue, Times.Never()),
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsTrue, Times.Once())
                }
            };
            yield return new object[]
            {
                "SecondRuleAlreadyProcessedTest: NP/CPT/1, P/CPT/0, NP/CPF/1",
                new List<RuleScenario>
                {
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsTrue, Times.Once()),
                    CreateRuleScenario(RuleStatusType.Passed, ContinueProcessingIsTrue, Times.Never()),
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsFalse, Times.Once())
                }
            };
            yield return new object[]
            {
                "SecondRuleAlreadyProcessedTest: NP/CPT/1, P/CPF/0, NP/CPT/1",
                new List<RuleScenario>
                {
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsTrue, Times.Once()),
                    CreateRuleScenario(RuleStatusType.Passed, ContinueProcessingIsFalse, Times.Never()),
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsTrue, Times.Once())
                }
            };
            yield return new object[]
            {
                "SecondRuleAlreadyProcessedTest: NP/CPF/1, P/CPT/0, NP/CPT/0",
                new List<RuleScenario>
                {
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsFalse, Times.Once()),
                    CreateRuleScenario(RuleStatusType.Passed, ContinueProcessingIsTrue, Times.Never()),
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsTrue, Times.Never())
                }
            };
            yield return new object[]
            {
                "SecondRuleAlreadyProcessedTest: NP/CPT/1, P/CPF/0, NP/CPF/1",
                new List<RuleScenario>
                {
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsTrue, Times.Once()),
                    CreateRuleScenario(RuleStatusType.Passed, ContinueProcessingIsFalse, Times.Never()),
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsFalse, Times.Once())
                }
            };
            yield return new object[]
            {
                "SecondRuleAlreadyProcessedTest: NP/CPF/1, P/CPT/0, NP/CPF/0",
                new List<RuleScenario>
                {
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsFalse, Times.Once()),
                    CreateRuleScenario(RuleStatusType.Passed, ContinueProcessingIsTrue, Times.Never()),
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsFalse, Times.Never())
                }
            };
            yield return new object[]
            {
                "SecondRuleAlreadyProcessedTest: NP/CPF/1, P/CPF/0, NP/CPT/0",
                new List<RuleScenario>
                {
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsFalse, Times.Once()),
                    CreateRuleScenario(RuleStatusType.Passed, ContinueProcessingIsFalse, Times.Never()),
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsTrue, Times.Never())
                }
            };
            yield return new object[]
            {
                "SecondRuleAlreadyProcessedTest: NP/CPF/1, P/CPF/0, NP/CPF/0",
                new List<RuleScenario>
                {
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsFalse, Times.Once()),
                    CreateRuleScenario(RuleStatusType.Passed, ContinueProcessingIsFalse, Times.Never()),
                    CreateRuleScenario(RuleStatusType.NotProcessed, ContinueProcessingIsFalse, Times.Never())
                }
            };
        }

        private static RuleScenario CreateRuleScenario(RuleStatusType ruleStatusType, bool continueProcessingIsTrue, Times expectedTimes) =>
            new RuleScenario
            {
                RuleStatusType = ruleStatusType,
                ContinueProcessing = continueProcessingIsTrue,
                ExpectedTimes = expectedTimes
            };

        private static Mock<IBaseRule<FakeRequest, FakeResponse>>
            CreateMockRule(FakeRequest fakeRequest, FakeResponse fakeResponse, RuleStatusType statusType, bool continueProcessing)
        {
            var mockRule = new Mock<IBaseRule<FakeRequest, FakeResponse>>();
            mockRule.Setup(x => x.ProcessAsync(fakeRequest, fakeResponse, ItIsAnyRules())).Returns(Task.CompletedTask);
            mockRule.Setup(x => x.Status).Returns(statusType);
            mockRule.Setup(x => x.IsNotProcessed()).Returns(statusType == RuleStatusType.NotProcessed);
            mockRule.Setup(x => x.ContinueProcessing).Returns(continueProcessing);
            return mockRule;
        }

        private static void VerifyMockRule(Mock<IBaseRule<FakeRequest, FakeResponse>> mockRule, FakeRequest fakeRequest, FakeResponse fakeResponse, Times times) =>
            mockRule.Verify(x => x.ProcessAsync(fakeRequest, fakeResponse, ItIsAnyRules()), times);

        private static IEnumerable<IBaseRule<FakeRequest, FakeResponse>> ItIsAnyRules() => It.IsAny<IEnumerable<IBaseRule<FakeRequest, FakeResponse>>>();

        public class FakeRequest {}

        public class FakeResponse {}

        public class RuleScenario
        {
            public RuleStatusType RuleStatusType { get; set; }
            public bool ContinueProcessing { get; set; }
            public Times ExpectedTimes { get; set; }
        }
    }
}
