using System.Threading;
using System.Threading.Tasks;

namespace Tips.ApiMessage.Pipeline
{
    public interface IRequestHandler<in TRequest, TResponse>
    {
        public Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
    }
}
