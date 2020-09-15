using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.Pipeline;
using Tips.ApiMessage.TodoItems.Context;
using Tips.ApiMessage.TodoItems.Mappers;
using Tips.ApiMessage.TodoItems.Models;
using Tips.ApiMessage.TodoItems.Rules.Engine;

namespace Tips.ApiMessage.TodoItems.UpdateTodoItem
{
    internal class UpdateTodoItemRequestHandler : IRequestHandler<UpdateTodoItemRequest, Response>
    {
        private readonly TodoContext _context;
        private readonly IRulesEngine _rulesEngine;
        private readonly IRulesFactory<SaveTodoItemRequest, Response<TodoItem>> _saveRulesFactory;
        private readonly IRulesFactory<UpdateTodoItemRequest, Response<TodoItem>> _updateRulesFactory;

        public UpdateTodoItemRequestHandler(TodoContext context, IRulesEngine rulesEngine,
            IRulesFactory<SaveTodoItemRequest, Response<TodoItem>> saveRulesFactory,
            IRulesFactory<UpdateTodoItemRequest, Response<TodoItem>> updateRulesFactory)
        {
            _context = context;
            _rulesEngine = rulesEngine;
            _saveRulesFactory = saveRulesFactory;
            _updateRulesFactory = updateRulesFactory;
        }

        public async Task<Response> Handle(UpdateTodoItemRequest request, CancellationToken cancellationToken)
        {
            var response = new Response<TodoItem>();

            if (ProcessRules(request, response, _updateRulesFactory.Create().ToList())) return response;
            if (ProcessRules(request, response, _saveRulesFactory.Create().ToList())) return response;

            // Command.  Save the data.
            return await Save(response, cancellationToken);
        }

        private bool ProcessRules<TRequest>(TRequest request, Response<TodoItem> response, IReadOnlyCollection<BaseRule<TRequest, Response<TodoItem>>> rules)
        {
            _rulesEngine.Process(request, response, rules);
            var rulesFailed = rules.Any(rule => rule.Failed);
            if (rulesFailed && response.IsStatusNotSet()) response.SetStatusToBadRequest();
            return rulesFailed;
        }

        private async Task<Response> Save(Response<TodoItem> response, CancellationToken cancellationToken)
        {
            TodoItemEntity todoItemEntity;
            try
            {
                todoItemEntity = await _context.TodoItems.FindAsync(response.Result.Id);

                TodoItemMapper.MapToTodoItemEntity(response.Result, todoItemEntity);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) when (!TodoItemExists(response.Result.Id))
            {
                response.Add(NotFoundWhenSavingNotification(response.Result.Id));
                response.SetStatusToNotFound();
                return response;
            }

            response.Result = TodoItemMapper.MapToTodoItem(todoItemEntity);
            response.SetStatusToNoContent();
            return response;
        }

        private bool TodoItemExists(long id) => _context.TodoItems.Any(e => e.Id == id);

        internal const string NotFoundWhenSavingNotificationId = "8FD46D5D-1CB3-4ECB-B27B-724813A0406C";

        private static Notification NotFoundWhenSavingNotification(long id) =>
            Notification.CreateError(NotFoundWhenSavingNotificationId, $"TodoItem {id} was not found when saving.");
    }
}
