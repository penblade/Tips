using Tips.ApiMessage.Infrastructure;
using Tips.ApiMessage.TodoItems.Context;
using Tips.ApiMessage.TodoItems.Context.Models;
using Tips.ApiMessage.TodoItems.Endpoint.Models;

namespace Tips.ApiMessage.TodoItems.Mappers
{
    internal static class TodoItemMapper
    {
        public static TodoItemEntity MapToTodoItemEntity(TodoItem source)
        {
            Guard.AgainstNull(source, nameof(source));

            var target = new TodoItemEntity();
            MapToTodoItemEntity(source, target);
            return target;
        }

        public static void MapToTodoItemEntity(TodoItem source, TodoItemEntity target)
        {
            Guard.AgainstNull(source, nameof(source));
            Guard.AgainstNull(target, nameof(target));

            target.Id = source.Id;
            target.Name = source.Name;
            target.Description = source.Description;
            target.Priority = source.Priority;
            target.IsComplete = source.IsComplete;
        }

        public static TodoItem MapToTodoItem(TodoItemEntity source)
        {
            Guard.AgainstNull(source, nameof(source));

            var target = new TodoItem();
            MapToTodoItem(source, target);
            return target;
        }

        public static void MapToTodoItem(TodoItemEntity source, TodoItem target)
        {
            Guard.AgainstNull(source, nameof(source));
            Guard.AgainstNull(target, nameof(target));

            target.Id = source.Id;
            target.Name = source.Name;
            target.Description = source.Description;
            target.Priority = source.Priority;
            target.IsComplete = source.IsComplete;
        }
    }
}
