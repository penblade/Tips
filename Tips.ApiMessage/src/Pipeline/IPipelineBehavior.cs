using System.Threading;
using System.Threading.Tasks;

namespace Tips.Pipeline
{
    public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();

    public interface IPipelineBehavior
    {
        public Task<TResponse> HandleAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> nextAsync);
    }
}
