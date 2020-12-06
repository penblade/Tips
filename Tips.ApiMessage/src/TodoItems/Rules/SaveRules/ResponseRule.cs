using System.Threading.Tasks;
using Tips.Pipeline;
using Tips.Rules;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Models;

namespace Tips.TodoItems.Rules.SaveRules
{
    internal class ResponseRule : BaseRule<Request<TodoItem>, Response<TodoItemEntity>>
    {
        protected override Task ProcessRuleAsync(Request<TodoItem> request, Response<TodoItemEntity> response)
        {
            response.Item = new TodoItemEntity
            {
                Id = request.Item.Id,
                IsComplete = request.Item.IsComplete
            };

            Pass();
            return Task.CompletedTask;
        }
    }
}
