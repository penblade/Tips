using System.Threading;
using System.Threading.Tasks;
using Tips.Pipeline;
using Tips.TodoItems.Context.Models;

namespace Tips.TodoItems.Handlers.CreateTodoItems
{
    public interface ICreateTodoItemRepository
    {
        Task SaveAsync(Response<TodoItemEntity> response, CancellationToken cancellationToken);
    }
}
