using Tips.ApiMessage.TodoItems.Context;
using Tips.ApiMessage.TodoItems.Models;

namespace Tips.ApiMessage.TodoItems.Mappers
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
