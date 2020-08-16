using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Tips.ApiMessage.Pipeline
{
    public class LoggingBehavior : IPipelineBehavior
    {
        private readonly ILogger<LoggingBehavior> _logger;
        public LoggingBehavior(ILogger<LoggingBehavior> logger) => _logger = logger;

        public async Task<TResponse> Handle<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            using var scope = _logger.BeginScope(request);
            _logger.LogInformation(CreateLogMessageForRequest(JsonSerializer.Serialize(request)), request);
            var response = await next();
            _logger.LogInformation(CreateLogMessageForResponse(JsonSerializer.Serialize(response)), response);
            return response;
        }

        private static string TraceId => Activity.Current?.Id;
        private static string CreateLogMessageForRequest(string request) => @$"TraceId: {TraceId} | Request: {FormatForLogging(request)}";
        private static string CreateLogMessageForResponse(string response) => @$"TraceId: {TraceId} | Response: {FormatForLogging(response)}";
        private static string FormatForLogging(string message) => message.Replace("{", "{{").Replace("}", "}}");
    }
}
