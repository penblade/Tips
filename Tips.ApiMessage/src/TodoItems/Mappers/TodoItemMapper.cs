using Tips.GuardClauses;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Models;

namespace Tips.TodoItems.Mappers
{
    internal static class TodoItemMapper
    {
        public static void MapToTodoItemEntity(TodoItemEntity source, TodoItemEntity target)
        {
            Guard.AgainstNull(source, nameof(source));
            Guard.AgainstNull(target, nameof(target));

            target.Id = source.Id;
            target.Name = source.Name;
            target.Description = source.Description;
            target.Priority = source.Priority;
            target.IsComplete = source.IsComplete;
            target.Reviewer = source.Reviewer;
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
