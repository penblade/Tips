using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tips.Pipeline;
using Tips.Rules;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Models;

namespace Tips.TodoItems.Handlers.UpdateTodoItem
{
    internal class UpdateTodoItemRequestHandler : IRequestHandler<UpdateTodoItemRequest, Response>
    {
        private readonly IRulesEngine _rulesEngine;
        private readonly IRulesFactory<Request<TodoItem>, Response<TodoItemEntity>> _saveRulesFactory;
        private readonly IRulesFactory<UpdateTodoItemRequest, Response<TodoItemEntity>> _updateRulesFactory;
        private readonly IUpdateTodoItemRepository _updateTodoItemRepository;

        public UpdateTodoItemRequestHandler(IRulesEngine rulesEngine,
            IRulesFactory<Request<TodoItem>, Response<TodoItemEntity>> saveRulesFactory,
            IRulesFactory<UpdateTodoItemRequest, Response<TodoItemEntity>> updateRulesFactory,
            IUpdateTodoItemRepository updateTodoItemRepository)
        {
            _rulesEngine = rulesEngine;
            _saveRulesFactory = saveRulesFactory;
            _updateRulesFactory = updateRulesFactory;
            _updateTodoItemRepository = updateTodoItemRepository;
        }

        public async Task<Response> HandleAsync(UpdateTodoItemRequest request, CancellationToken cancellationToken)
        {
            var todoItemResponse = new Response<TodoItemEntity>();

            // Query. Apply all validation and modification rules.  These rules can only query the database.
            await _rulesEngine.ProcessAsync(request, todoItemResponse, _updateRulesFactory.Create().ToList());
            if (todoItemResponse.HasErrors()) return new Response(todoItemResponse.Notifications);

            // Only process the save rules if the update rules passed with no errors.
            await _rulesEngine.ProcessAsync(request, todoItemResponse, _saveRulesFactory.Create().ToList());
            if (todoItemResponse.HasErrors()) return new Response(todoItemResponse.Notifications);

            // Command.  Save the data.
            await _updateTodoItemRepository.SaveAsync(todoItemResponse, cancellationToken);

            return new Response(todoItemResponse.Notifications);
        }
    }
}
