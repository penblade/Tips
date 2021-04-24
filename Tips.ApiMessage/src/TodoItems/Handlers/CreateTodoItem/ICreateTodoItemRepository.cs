using System.Threading.Tasks;
using Tips.Pipeline;
using Tips.TodoItems.Context.Models;

namespace Tips.TodoItems.Handlers.CreateTodoItem
{
    internal interface ICreateTodoItemRepository
    {
        Task SaveAsync(Response<TodoItemEntity> response);
    }
}
