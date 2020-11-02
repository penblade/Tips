using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tips.Pipeline.Extensions;

namespace Tips.Pipeline.Logging
{
    internal class LoggingBehavior : IPipelineBehavior
    {
        private readonly ILogger<LoggingBehavior> _logger;
        public LoggingBehavior(ILogger<LoggingBehavior> logger) => _logger = logger;

        public async Task<TResponse> HandleAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> nextAsync)
        {
            LogRequest(request);
            var response = await nextAsync();
            LogResponse(response);

            return response;
        }

        private void LogRequest<TRequest>(TRequest request)
        {
            const string scope = "Request";
            using (_logger.BeginScopeWithApiTraceParentId())
            using (_logger.BeginScopeWithApiTraceId())
            using (_logger.BeginScopeWithApiTraceParentStateString())
            using (_logger.BeginScopeWithApiTraceStateString(scope))
            using (_logger.BeginScopeWithApiScope(scope))
            {
                _logger.LogInformation("{Request}", JsonSerializer.Serialize(request));
            }
        }

        private void LogResponse<TResponse>(TResponse response)
        {
            const string scope = "Response";
            using (_logger.BeginScopeWithApiTraceParentId())
            using (_logger.BeginScopeWithApiTraceId())
            using (_logger.BeginScopeWithApiTraceParentStateString())
            using (_logger.BeginScopeWithApiTraceStateString(scope))
            using (_logger.BeginScopeWithApiScope(scope))
            {
                _logger.LogInformation("{Response}", JsonSerializer.Serialize(response));
            }
        }
    }
}
