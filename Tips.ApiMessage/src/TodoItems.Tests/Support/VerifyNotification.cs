using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tips.Pipeline;

namespace Tips.TodoItems.Tests.Support
{
    internal static class VerifyNotification
    {
        public static void AssertResponseNotifications(Response expectedResponse, Response actualResponse)
        {
            Assert.IsNotNull(expectedResponse);
            Assert.IsNotNull(actualResponse);

            AssertNotifications(expectedResponse.Notifications, actualResponse.Notifications);
        }

        private static void AssertNotifications(List<Notification> expectedNotifications, List<Notification> actualNotifications)
        {
            Assert.IsNotNull(expectedNotifications);
            Assert.IsNotNull(actualNotifications);

            var expectedCount = expectedNotifications.Count;
            var actualCount = actualNotifications.Count;
            Assert.AreEqual(expectedCount, actualCount);

            for (var i = 0; i < expectedCount; i++)
            {
                AssertNotification(expectedNotifications[i], actualNotifications[i]);
            }
        }

        private static void AssertNotification(Notification expectedNotification, Notification actualNotification)
        {
            Assert.IsNotNull(expectedNotification);
            Assert.IsNotNull(actualNotification);

            Assert.IsInstanceOfType(actualNotification, expectedNotification.GetType());
            Assert.AreEqual(expectedNotification.Severity, actualNotification.Severity);
            Assert.AreEqual(expectedNotification.Id, actualNotification.Id);
            Assert.AreEqual(expectedNotification.Detail, actualNotification.Detail);

            AssertNotifications(expectedNotification.Notifications, actualNotification.Notifications);
        }
    }
}
