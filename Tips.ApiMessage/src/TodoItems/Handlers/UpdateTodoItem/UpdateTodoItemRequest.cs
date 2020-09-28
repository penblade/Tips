using Tips.Pipeline;
using Tips.TodoItems.Models;

namespace Tips.TodoItems.Handlers.UpdateTodoItem
{
    public class UpdateTodoItemRequest : Request<TodoItem>
    {
        public long Id { get; set; }
    }
}
