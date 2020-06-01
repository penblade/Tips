using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Tips.ApiMessage.Handlers
{
    public interface IActionResultHandler<in TRequest, TResponse> where TResponse : Response
    {
        Task<ActionResult> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next);
    }
}
