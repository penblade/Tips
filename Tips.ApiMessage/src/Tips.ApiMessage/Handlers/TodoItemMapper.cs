using Tips.ApiMessage.Context;
using Tips.ApiMessage.Models;

namespace Tips.ApiMessage.Handlers
{
    internal class TodoItemMapper
    {
        public static TodoItem ItemToResponse(TodoItemEntity todoItem) =>
            new TodoItem
            {
                Id = todoItem.Id,
                Name = todoItem.Name,
                IsComplete = todoItem.IsComplete
            };
    }
}
