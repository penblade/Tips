using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;

namespace Support.Tests
{
    public class MockLogger
    {
        public static void VerifyBeginScope<TCategory>(Mock<ILogger<TCategory>> mockLogger, string state) =>
            mockLogger.Verify(logger => logger.BeginScope(It.Is<object>(request => request.ToString() == state)), Times.Once);

        public static void VerifyLog<TCategory>(Mock<ILogger<TCategory>> mockLogger, LogLevel logLevel, string message, string key) =>
            mockLogger.Verify(logger =>
                    logger.Log(
                        logLevel,
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((state, t) => CheckValue(state, message, key)),
                        It.IsAny<Exception>(),
                        (Func<It.IsAnyType, Exception, string>)It.IsAny<object>())
                , Times.Once);

        /// <summary>
        /// Checks that a given key exists in the given collection, and that the value matches
        /// *** Updated to get FirstOrDefault. ***
        /// Credit: https://christianfindlay.com/2020/07/03/ilogger/
        /// Code: https://github.com/MelbourneDeveloper/Samples/blob/650ba4bc6cba631da651c2c6732bce337e6a7d8e/ILoggerSamples/ILoggerSamples/ILoggerTests.cs#L44
        /// </summary>
        private static bool CheckValue(object state, object expectedValue, string key)
        {
            var keyValuePairList = (IReadOnlyList<KeyValuePair<string, object>>)state;

            var actualValue = keyValuePairList.FirstOrDefault(kvp =>
                string.Compare(kvp.Key, key, StringComparison.Ordinal) == 0).Value;

            return expectedValue.Equals(actualValue);
        }
    }
}
