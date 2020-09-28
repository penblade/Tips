using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Tips.Pipeline
{
    internal class LoggingBehavior : IPipelineBehavior
    {
        private readonly ILogger<LoggingBehavior> _logger;
        public LoggingBehavior(ILogger<LoggingBehavior> logger) => _logger = logger;

        public async Task<TResponse> HandleAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> nextAsync)
        {
            using var scope = _logger.BeginScope(request);
            _logger.LogInformation(CreateLogMessageForRequest(JsonSerializer.Serialize(request)), request);
            var response = await nextAsync();
            _logger.LogInformation(CreateLogMessageForResponse(JsonSerializer.Serialize(response)), response);
            return response;
        }

        private static string CreateLogMessageForRequest(string request) => @$"TraceId: {Tracking.TraceId} | Request: {LogFormatter.FormatForLogging(request)}";
        private static string CreateLogMessageForResponse(string response) => @$"TraceId: {Tracking.TraceId} | Response: {LogFormatter.FormatForLogging(response)}";
    }
}
