using System;
using System.Buffers.Text;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tips.Pipeline;
using Tips.Pipeline.Extensions;
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
        private readonly ILogger<UpdateTodoItemRequestHandler> _logger;

        public UpdateTodoItemRequestHandler(IRulesEngine rulesEngine,
            IRulesFactory<Request<TodoItem>, Response<TodoItemEntity>> saveRulesFactory,
            IRulesFactory<UpdateTodoItemRequest, Response<TodoItemEntity>> updateRulesFactory,
            IUpdateTodoItemRepository updateTodoItemRepository,
            ILogger<UpdateTodoItemRequestHandler> logger)
        {
            _rulesEngine = rulesEngine;
            _saveRulesFactory = saveRulesFactory;
            _updateRulesFactory = updateRulesFactory;
            _updateTodoItemRepository = updateTodoItemRepository;
            _logger = logger;
        }

        public async Task<Response> HandleAsync(UpdateTodoItemRequest request, CancellationToken cancellationToken)
        {
            var todoItemEntityResponse = new Response<TodoItemEntity>();

            // Query. Apply all validation and modification rules.  These rules can only query the database.
            await _rulesEngine.ProcessAsync(request, todoItemEntityResponse, _updateRulesFactory.Create().ToList());
            if (todoItemEntityResponse.HasErrors()) return new Response(todoItemEntityResponse.Notifications);

            // Only process the save rules if the update rules passed with no errors.
            await _rulesEngine.ProcessAsync(request, todoItemEntityResponse, _saveRulesFactory.Create().ToList());
            if (todoItemEntityResponse.HasErrors()) return new Response(todoItemEntityResponse.Notifications);

            // Command.  Save the data.
            await _updateTodoItemRepository.SaveAsync(todoItemEntityResponse, cancellationToken);
            LogTodoItemEntityResponse(todoItemEntityResponse);

            return new Response(todoItemEntityResponse.Notifications);
        }

        private void LogTodoItemEntityResponse(Response<TodoItemEntity> todoItemEntityResponse)
        {
            const string scope = "Updated TodoItemEntity";
            using (_logger.BeginScopeWithApiTraceParentId())
            using (_logger.BeginScopeWithApiTraceParentStateString())
            using (_logger.BeginScopeWithApiTraceId())
            using (_logger.BeginScopeWithApiTraceStateString(scope))
            using (_logger.BeginScopeWithApiScope(scope))
            {
                _logger.LogInformation("{TodoItemEntityResponse}", JsonSerializer.Serialize(todoItemEntityResponse));
            }
        }
    }
}
