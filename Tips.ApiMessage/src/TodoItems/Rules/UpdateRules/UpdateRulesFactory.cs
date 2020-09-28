using System.Collections.Generic;
using Tips.Pipeline;
using Tips.Rules;
using Tips.TodoItems.Context;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Handlers.UpdateTodoItem;

namespace Tips.TodoItems.Rules.UpdateRules
{
    public class UpdateRulesFactory : IRulesFactory<UpdateTodoItemRequest, Response<TodoItemEntity>>
    {
        private readonly TodoContext _context;

        public UpdateRulesFactory(TodoContext context) => _context = context;

        public IEnumerable<BaseRule<UpdateTodoItemRequest, Response<TodoItemEntity>>> Create()
        {
            yield return new TodoItemNotSameIdRule();
            yield return new TodoItemNotFoundRule(_context);
        }
    }
}
