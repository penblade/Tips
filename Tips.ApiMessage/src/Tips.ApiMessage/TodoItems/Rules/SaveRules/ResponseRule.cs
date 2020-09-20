using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Models;
using Tips.ApiMessage.TodoItems.Rules.Engine;

namespace Tips.ApiMessage.TodoItems.Rules.SaveRules
{
    internal class ResponseRule : BaseRule<Request<TodoItem>, Response<TodoItem>>
    {
        protected override void ProcessRule(Request<TodoItem> request, Response<TodoItem> response)
        {
            response.Result = new TodoItem
            {
                Id = request.Item.Id,
                IsComplete = request.Item.IsComplete
            };

            RulePassed();
        }
    }
}
