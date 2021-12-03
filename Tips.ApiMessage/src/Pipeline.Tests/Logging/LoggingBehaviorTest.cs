using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Tips.Support.Tests;
using Tips.Pipeline;
using Tips.Pipeline.Logging;

namespace Tips.Pipeline.Tests.Logging
{
    [TestClass]
    public class LoggingBehaviorTest
    {
        [TestMethod]
        public async Task HandleAsyncTest()
        {
            const string requestString = "Request";
            const string responseString = "Response";

            var fakeRequest = new FakeRequest { FakeIntProperty = 42, FakeStringProperty = "Test String" };
            var serializedFakeRequest = JsonSerializer.Serialize(fakeRequest);

            var fakeResponse = new FakeResponse { FakeBoolProperty = true, FakeDecimalProperty = 3.6M };
            var serializedFakeResponse = JsonSerializer.Serialize(fakeResponse);

            var mockLogger = new Mock<ILogger<LoggingBehavior>>();

            //var loggingBehavior = new LoggingBehavior(fakeLogger);
            var loggingBehavior = new LoggingBehavior(mockLogger.Object);
            var mockRequestHandlerDelegate = new Mock<RequestHandlerDelegate<FakeResponse>>();
            mockRequestHandlerDelegate.Setup(x => x.Invoke()).ReturnsAsync(fakeResponse);

            var actualResponse = await loggingBehavior.HandleAsync(fakeRequest, mockRequestHandlerDelegate.Object);

            mockLogger.VerifyBeginScope(requestString);
            mockLogger.VerifyBeginScope(responseString);
            mockLogger.VerifyLog(LogLevel.Information, serializedFakeRequest, requestString);
            mockLogger.VerifyLog(LogLevel.Information, serializedFakeResponse, responseString);

            Assert.AreSame(fakeResponse, actualResponse);
        }

        public class FakeRequest
        {
            public int FakeIntProperty { get; set; }
            public string FakeStringProperty { get; set; }
        }

        public class FakeResponse
        {
            public bool FakeBoolProperty { get; set; }
            public decimal FakeDecimalProperty { get; set; }
        }
    }
}
