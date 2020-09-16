using System.Threading;
using System.Threading.Tasks;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Models;

namespace Tips.ApiMessage.TodoItems.UpdateTodoItem
{
    internal interface IUpdateTodoItemRepository
    {
        Task<Response> Save(Response<TodoItem> response, CancellationToken cancellationToken);
    }
}
