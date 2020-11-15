using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tips.Pipeline.Extensions;

namespace Tips.Pipeline.Logging
{
    internal class LoggingBehavior : IPipelineBehavior
    {
        private readonly ILogger<LoggingBehavior> _logger;
        public LoggingBehavior(ILogger<LoggingBehavior> logger) => _logger = logger;

        public async Task<TResponse> HandleAsync<TRequest, TResponse>(TRequest request, RequestHandlerDelegate<TResponse> nextAsync)
        {
            LogRequest(request);
            var response = await nextAsync();
            LogResponse(response);
            return response;
        }

        private void LogRequest<TRequest>(TRequest request) =>
            _logger.LogAction("Request", () => _logger.LogInformation("{Request}", JsonSerializer.Serialize(request)));

        private void LogResponse<TResponse>(TResponse response) =>
            _logger.LogAction("Response", () => _logger.LogInformation("{Response}", JsonSerializer.Serialize(response)));
    }
}
