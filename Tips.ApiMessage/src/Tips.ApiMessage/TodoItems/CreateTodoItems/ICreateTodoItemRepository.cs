using System.Threading;
using System.Threading.Tasks;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Models;

namespace Tips.ApiMessage.TodoItems.CreateTodoItems
{
    internal interface ICreateTodoItemRepository
    {
        Task<Response<TodoItem>> Save(Response<TodoItem> response, CancellationToken cancellationToken);
    }
}
