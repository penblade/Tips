using System.Threading;
using System.Threading.Tasks;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Context.Models;

namespace Tips.ApiMessage.TodoItems.UpdateTodoItem
{
    internal interface IUpdateTodoItemRepository
    {
        Task SaveAsync(Response<TodoItemEntity> response, CancellationToken cancellationToken);
    }
}
