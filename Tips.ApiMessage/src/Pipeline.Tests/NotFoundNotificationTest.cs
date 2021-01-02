using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tips.Pipeline;

namespace Pipeline.Tests
{
    [TestClass]
    public class LoggerExtensionsTest
    {
        [TestMethod]
        public void CreateTest()
        {
            const string expectedId = "BDCD9400-2C37-4832-ACAE-008284FCB6E2";
            const string expectedDetail = "Test Detail Test";
            var notification = NotFoundNotification.Create(expectedId, expectedDetail);

            Assert.AreEqual(expectedId, notification.Id);
            Assert.AreEqual(expectedDetail, notification.Detail);
            Assert.AreEqual(Notification.SeverityType.Error, notification.Severity);
        }
    }
}
