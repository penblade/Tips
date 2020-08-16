using System.Threading;
using System.Threading.Tasks;

namespace Tips.ApiMessage.Pipeline
{
    public interface IRequestHandler<in TRequest, TResponse>
    {
        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }
}
