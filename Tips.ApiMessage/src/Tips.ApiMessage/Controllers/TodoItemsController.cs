using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tips.ApiMessage.Context;
using Tips.ApiMessage.Handlers;
using Tips.ApiMessage.Messages;
using Tips.ApiMessage.Models;

namespace Tips.ApiMessage.Controllers
{
    // Created based on the Tutorial: Create a web API with ASP.NET Core
    // https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-3.1&tabs=visual-studio

    [Route("api/TodoItems")]
    [ApiController]
    public class TodoItemsController : Controller
    {
        private readonly IRequestHandler<TodoItemsQuery, Response> _getTodoItemsRequestHandler;
        private readonly IRequestHandler<TodoItemQuery, Response> _getTodoItemRequestHandler;
        private readonly TodoContext _context;
        private string TraceId => HttpContext.TraceIdentifier;

        public TodoItemsController(IRequestHandler<TodoItemsQuery, Response> getTodoItemsRequestHandler, IRequestHandler<TodoItemQuery, Response> getTodoItemRequestHandler, TodoContext context)
        {
            _getTodoItemsRequestHandler = getTodoItemsRequestHandler;
            _getTodoItemRequestHandler = getTodoItemRequestHandler;
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<ActionResult> GetTodoItems() => await HandleRequest(async () => await ProcessGetTodoItems());
        //public async Task<IActionResult> GetTodoItems() => await _actionResultHandler.Handle(null, cancellationToken, () => _getTodoItemsRequestHandler.Handle(null, new CancellationToken()));
        public async Task<IActionResult> GetTodoItems() => await Handle((request) => _getTodoItemsRequestHandler.Handle(new TodoItemsQuery(), new CancellationToken()), null);

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetTodoItem(long id) => await HandleRequest(async () => await ProcessGetTodoItem(id));

        // PUT: api/TodoItems/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoItem(long id, TodoItem todoItem) => await ProcessUpdateTodoItem(id, todoItem);

        // POST: api/TodoItems
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateTodoItem(TodoItem todoItem) => await ProcessCreateTodoItem(todoItem);

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteTodoItem(long id) => await ProcessDeleteTodoItem(id);







        private async Task<ActionResult> HandleRequest(Func<Task<ActionResult>> method)
        {
            try
            {
                return await method();
            }
            catch (Exception ex)
            {
                // TODO: Log exception
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        private async Task<ActionResult> ProcessGetTodoItem(long id)
        {
            return Ok(await new GetTodoItemRequestHandler<TodoItemsQuery, Response>(_context).Handle(new TodoItemQuery { Id = id }, new CancellationToken()));
        }






        private async Task<ActionResult> ProcessCreateTodoItem(TodoItem todoItem)
        {
            var todoItemEntity = new TodoItemEntity
            {
                IsComplete = todoItem.IsComplete,
                Name = todoItem.Name
            };

            _context.TodoItems.Add(todoItemEntity);
            await _context.SaveChangesAsync();

            var response = new TodoItemResponse
            {
                ApiMessage = CreateApiMessage(HttpStatusCode.Created),
                TodoItem = ItemToResponse(todoItemEntity)
            };
            return CreatedAtAction(nameof(CreateTodoItem), new { id = todoItemEntity.Id }, response);
        }

        private async Task<IActionResult> ProcessUpdateTodoItem(long id, TodoItem todoItem)
        {
            if (id != todoItem.Id) return BadRequest();

            var todoItemEntity = await _context.TodoItems.FindAsync(id);
            if (todoItemEntity == null) return NotFound();

            todoItemEntity.Name = todoItem.Name;
            todoItemEntity.IsComplete = todoItem.IsComplete;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
            {
                var notification = new NotificationBuilder()
                    .Id("9E1A675F-3073-4D78-9A22-317ECB1D88DC")
                    .Severity(Severity.Error)
                    .Detail($"TodoItem {id} was not found.")
                    .Build();
                return NotFound(new Response
                {
                    ApiMessage = CreateApiMessage(HttpStatusCode.NotFound, new List<Notification> {notification})
                });
            }

            // I prefer to return an Ok response with a message that includes any possible notifications.
            // I find it's easier for the client to have a standard response to implement.
            // return NoContent();
            return Ok(new Response
            {
                ApiMessage = CreateApiMessage(HttpStatusCode.OK)
            });
        }

        private async Task<IActionResult> ProcessDeleteTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TodoItemExists(long id) => _context.TodoItems.Any(e => e.Id == id);

        private static TodoItem ItemToResponse(TodoItemEntity todoItem) =>
            new TodoItem
            {
                Id = todoItem.Id,
                Name = todoItem.Name,
                IsComplete = todoItem.IsComplete
            };

        private Messages.ApiMessage CreateApiMessage(HttpStatusCode httpStatusCode, IEnumerable<Notification> notifications = null) =>
            new Messages.ApiMessage
            {
                TraceId = TraceId,
                Status = (int) httpStatusCode,
                Notifications = notifications
            };
    }
}
