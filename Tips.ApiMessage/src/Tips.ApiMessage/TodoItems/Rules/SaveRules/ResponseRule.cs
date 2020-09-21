using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.TodoItems.Context.Models;
using Tips.ApiMessage.TodoItems.Endpoint.Models;
using Tips.ApiMessage.TodoItems.Rules.Engine;

namespace Tips.ApiMessage.TodoItems.Rules.SaveRules
{
    internal class ResponseRule : BaseRule<Request<TodoItem>, Response<TodoItemEntity>>
    {
        protected override void ProcessRule(Request<TodoItem> request, Response<TodoItemEntity> response)
        {
            response.Item = new TodoItemEntity
            {
                Id = request.Item.Id,
                IsComplete = request.Item.IsComplete
            };

            RulePassed();
        }
    }
}
