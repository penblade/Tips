using Tips.ApiMessage.TodoItems.Models;

namespace Tips.ApiMessage.TodoItems.UpdateTodoItem
{
    public class UpdateTodoItemRequest : SaveTodoItemRequest
    {
        public long Id { get; set; }
    }
}
