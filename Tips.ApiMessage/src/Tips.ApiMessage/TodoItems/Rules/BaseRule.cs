using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.Infrastructure;
using Tips.ApiMessage.TodoItems.Models;

namespace Tips.ApiMessage.TodoItems.Rules
{
    internal abstract class BaseRule
    {
        // Template method pattern
        // https://en.wikipedia.org/wiki/Template_method_pattern
        public void Process(SaveTodoItemRequest request, Response<TodoItem> response)
        {
            Guard.AgainstNull(request, "request");
            Guard.AgainstNull(request.TodoItem, "request.TodoItem");
            Guard.AgainstNull(response, "response");
            Guard.AgainstNull(response.Result, "response.Result");

            ProcessRule(request, response);
        }

        protected abstract void ProcessRule(SaveTodoItemRequest request, Response<TodoItem> response);
    }
}
