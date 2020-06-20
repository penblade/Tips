using Tips.ApiMessage.TodoItems.Models;

namespace Tips.ApiMessage.TodoItems.UpdateTodoItem
{
    public class UpdateTodoItemRequest
    {
        public long Id { get; set; }
        public TodoItem TodoItem { get; set; }
    }
}
