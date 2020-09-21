using System.Collections.Generic;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Context;
using Tips.ApiMessage.TodoItems.Context.Models;
using Tips.ApiMessage.TodoItems.Rules.Engine;
using Tips.ApiMessage.TodoItems.UpdateTodoItem;

namespace Tips.ApiMessage.TodoItems.Rules.UpdateRules
{
    internal class UpdateRulesFactory : IRulesFactory<UpdateTodoItemRequest, Response<TodoItemEntity>>
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
