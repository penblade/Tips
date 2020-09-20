using System.Collections.Generic;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Context;
using Tips.ApiMessage.TodoItems.Endpoint.Models;
using Tips.ApiMessage.TodoItems.Rules.Engine;
using Tips.ApiMessage.TodoItems.UpdateTodoItem;

namespace Tips.ApiMessage.TodoItems.Rules.UpdateRules
{
    internal class UpdateRulesFactory : IRulesFactory<UpdateTodoItemRequest, Response<TodoItem>>
    {
        private readonly TodoContext _context;

        public UpdateRulesFactory(TodoContext context) => _context = context;

        public IEnumerable<BaseRule<UpdateTodoItemRequest, Response<TodoItem>>> Create()
        {
            yield return new TodoItemNotSameIdRule();
            yield return new TodoItemNotFoundRule(_context);
        }
    }
}
