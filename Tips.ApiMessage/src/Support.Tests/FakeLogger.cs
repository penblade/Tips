using System;
using Microsoft.Extensions.Logging;

namespace Support.Tests
{
    public class FakeLogger<TCategoryName> : ILogger<TCategoryName>
    {
        public virtual void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            => throw new NotImplementedException();

        public virtual bool IsEnabled(LogLevel logLevel) => throw new NotImplementedException();

        public virtual IDisposable BeginScope<TState>(TState state) => throw new NotImplementedException();
    }
}
