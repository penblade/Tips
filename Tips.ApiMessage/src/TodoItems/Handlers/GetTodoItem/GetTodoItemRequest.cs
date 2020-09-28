using Tips.Pipeline;

namespace Tips.TodoItems.Handlers.GetTodoItem
{
    public class GetTodoItemRequest : Request
    {
        public long Id { get; set; }
    }
}
