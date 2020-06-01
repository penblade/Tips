using System.Threading;
using System.Threading.Tasks;

namespace Tips.ApiMessage.Handlers
{
    public interface IRequestHandler<in TRequest, TResponse>
    {
        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }
}
