using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tips.Pipeline;

namespace Tips.Pipeline.Tests
{
    [TestClass]
    public class ResponseTest
    {
        private const string ExpectedId = "BDCD9400-2C37-4832-ACAE-008284FCB6E2";
        private const string ExpectedDetail = "Test Detail Test";

        [TestMethod]
        [DynamicData(nameof(SetupHasErrorsTest), DynamicDataSourceType.Method)]
        public void HasErrorsTest(string scenario, List<Notification> notifications, bool expectedHasErrors)
        {
            var response = new Response(notifications);
            Assert.AreEqual(expectedHasErrors, response.HasErrors());
        }

        private static IEnumerable<object[]> SetupHasErrorsTest()
        {
            const bool hasErrorsTrue = true;
            const bool hasErrorsFalse = false;

            yield return new object[]
            {
                "3 errors",
                new List<Notification>
                {
                    Notification.CreateError(ExpectedId, ExpectedDetail),
                    Notification.CreateError(ExpectedId, ExpectedDetail),
                    NotFoundNotification.Create(ExpectedId, ExpectedDetail)
                },
                hasErrorsTrue
            };

            yield return new object[]
            {
                "2 errors",
                new List<Notification>
                {
                    Notification.CreateInfo(ExpectedId, ExpectedDetail),
                    Notification.CreateError(ExpectedId, ExpectedDetail),
                    NotFoundNotification.Create(ExpectedId, ExpectedDetail)
                },
                hasErrorsTrue
            };

            yield return new object[]
            {
                "1 errors",
                new List<Notification>
                {
                    Notification.CreateInfo(ExpectedId, ExpectedDetail),
                    Notification.CreateError(ExpectedId, ExpectedDetail),
                    Notification.CreateWarning(ExpectedId, ExpectedDetail)
                },
                hasErrorsTrue
            };

            yield return new object[]
            {
                "0 errors",
                new List<Notification>
                {
                    Notification.CreateInfo(ExpectedId, ExpectedDetail),
                    Notification.CreateInfo(ExpectedId, ExpectedDetail),
                    Notification.CreateWarning(ExpectedId, ExpectedDetail)
                },
                hasErrorsFalse
            };
        }

        [TestMethod]
        [DynamicData(nameof(SetupIsNotFoundTest), DynamicDataSourceType.Method)]
        public void IsNotFoundTest(string scenario, List<Notification> notifications, bool expectedIsNotFound)
        {
            var response = new Response(notifications);
            Assert.AreEqual(expectedIsNotFound, response.IsNotFound());
        }

        private static IEnumerable<object[]> SetupIsNotFoundTest()
        {
            const bool isNotFoundTrue = true;
            const bool isNotFoundFalse = false;

            yield return new object[]
            {
                "3 not found",
                new List<Notification>
                {
                    NotFoundNotification.Create(ExpectedId, ExpectedDetail),
                    NotFoundNotification.Create(ExpectedId, ExpectedDetail),
                    NotFoundNotification.Create(ExpectedId, ExpectedDetail)
                },
                isNotFoundTrue
            };

            yield return new object[]
            {
                "2 not found",
                new List<Notification>
                {
                    Notification.CreateError(ExpectedId, ExpectedDetail),
                    NotFoundNotification.Create(ExpectedId, ExpectedDetail),
                    NotFoundNotification.Create(ExpectedId, ExpectedDetail)
                },
                isNotFoundTrue
            };

            yield return new object[]
            {
                "1 not found",
                new List<Notification>
                {
                    Notification.CreateError(ExpectedId, ExpectedDetail),
                    NotFoundNotification.Create(ExpectedId, ExpectedDetail),
                    Notification.CreateWarning(ExpectedId, ExpectedDetail)
                },
                isNotFoundTrue
            };

            yield return new object[]
            {
                "0 not found",
                new List<Notification>
                {
                    Notification.CreateError(ExpectedId, ExpectedDetail),
                    Notification.CreateInfo(ExpectedId, ExpectedDetail),
                    Notification.CreateWarning(ExpectedId, ExpectedDetail)
                },
                isNotFoundFalse
            };
        }
    }
}
