using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Tips.Pipeline;
using Tips.Pipeline.Extensions;

namespace Pipeline.Tests.Extensions
{
    [TestClass]
    public class LoggerExtensionsTest
    {
        [TestMethod]
        public void CreateTest()
        {
            const string traceStateStringValue = "ThisScope";
            var parentActivity = new Activity("ParentActivity").Start();
            var currentActivity = new Activity("CurrentActivity").Start();

            var mockProcessor = new Mock<ITestProcessor>();

            try
            {
                var mockLogger = new Mock<ILogger>();

                mockLogger.Object.LogAction(traceStateStringValue, () => mockProcessor.Object.Process());

                mockLogger.Verify(logger => logger.BeginScope(It.Is<object>(x => x.ToString() == Tracking.TraceId)), Times.Once);
                mockLogger.Verify(logger => logger.BeginScope(It.Is<object>(x => x.ToString() == Tracking.TraceStateString(traceStateStringValue))), Times.Once);
                mockLogger.Verify(logger => logger.BeginScope(It.Is<object>(x => x.ToString() == traceStateStringValue)), Times.Once);
                mockProcessor.Verify(processor => processor.Process(), Times.Once);
            }
            finally
            {
                currentActivity.Stop();
                parentActivity.Stop();
            }
        }

        public interface ITestProcessor
        {
            public void Process();
        }
    }
}
