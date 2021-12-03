using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tips.Pipeline;

namespace Tips.Pipeline.Tests
{
    [TestClass]
    public class NotificationTest
    {
        private const string ExpectedId = "BDCD9400-2C37-4832-ACAE-008284FCB6E2";
        private const string ExpectedDetail = "Test Detail Test";

        [TestMethod]
        public void CreateErrorTest()
        {
            var notification = Notification.CreateError(ExpectedId, ExpectedDetail);
            AssertNotification(ExpectedId, notification, ExpectedDetail, Notification.SeverityType.Error);
        }

        [TestMethod]
        public void CreateInfoTest()
        {
            var notification = Notification.CreateInfo(ExpectedId, ExpectedDetail);
            AssertNotification(ExpectedId, notification, ExpectedDetail, Notification.SeverityType.Info);
        }

        [TestMethod]
        public void CreateWarningTest()
        {
            var notification = Notification.CreateWarning(ExpectedId, ExpectedDetail);
            AssertNotification(ExpectedId, notification, ExpectedDetail, Notification.SeverityType.Warning);
        }

        private static void AssertNotification(string expectedId, Notification notification, string expectedDetail, string expectedSeverity)
        {
            Assert.AreEqual(expectedId, notification.Id);
            Assert.AreEqual(expectedDetail, notification.Detail);
            Assert.AreEqual(expectedSeverity, notification.Severity);
        }
    }
}
