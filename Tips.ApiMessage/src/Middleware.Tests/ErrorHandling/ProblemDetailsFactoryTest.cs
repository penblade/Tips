using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tips.Middleware.ErrorHandling;
using Tips.Pipeline;

namespace Tips.Middleware.Tests.ErrorHandling
{
    [TestClass]
    public class ProblemDetailsFactoryTest
    {
        [TestMethod]
        public void BadRequestTest()
        {
            var currentActivity = new Activity("CurrentActivity").Start();

            try
            {
                var configuration = CreateProblemDetailsConfiguration();

                var expected = CreateExpectedProblemDetailsWithNotifications(configuration);

                var problemDetailsFactory = new ProblemDetailsFactory(configuration);

                var actual = problemDetailsFactory.BadRequest(expected.Notifications);
                AssertProblemDetailsWithNotifications(expected, actual);
            }
            finally
            {
                currentActivity.Stop();
            }
        }

        private static ProblemDetailsWithNotifications CreateExpectedProblemDetailsWithNotifications(ProblemDetailsConfiguration configuration)
        {
            var expected = new ProblemDetailsWithNotifications
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Bad Request",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = "Review the notifications for details.",
                Instance = $"urn:{configuration.UrnName}:error:{ProblemDetailsFactory.BadRequestId}",
                Notifications = new List<Notification>()
            };

            expected.Extensions["traceId"] = Tracking.TraceId;
            return expected;
        }

        private static void AssertProblemDetailsWithNotifications(ProblemDetailsWithNotifications expected, ProblemDetailsWithNotifications actual)
        {
            Assert.AreEqual(expected.Type, actual.Type);
            Assert.AreEqual(expected.Title, actual.Title);
            Assert.AreEqual(expected.Status, actual.Status);
            Assert.AreEqual(expected.Detail, actual.Detail);
            Assert.AreEqual(expected.Instance, actual.Instance);
            Assert.AreEqual(expected.Extensions["traceId"], actual.Extensions["traceId"]);
            Assert.AreSame(expected.Notifications, actual.Notifications);
        }

        private static ProblemDetailsConfiguration CreateProblemDetailsConfiguration() =>
            new()
            {
                UrnName = "TestUrnName"
            };
    }
}
