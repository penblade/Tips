using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tips.ApiMessage.Contracts;
using Tips.ApiMessage.Handlers;
using Tips.ApiMessage.TodoItems.Context;
using Tips.ApiMessage.TodoItems.CreateTodoItems;
using Tips.ApiMessage.TodoItems.DeleteTodoItems;
using Tips.ApiMessage.TodoItems.GetTodoItem;
using Tips.ApiMessage.TodoItems.GetTodoItems;
using Tips.ApiMessage.TodoItems.Models;

namespace Tips.ApiMessage.TodoItems.Controllers
{
    // Created based on the Tutorial: Create a web API with ASP.NET Core
    // https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-3.1&tabs=visual-studio

    [Route("api/TodoItems")]
    [ApiController]
    public class TodoItemsController : Controller
    {
        private readonly IRequestHandler<GetTodoItemsRequest, GetTodoItemsResponse> _getTodoItemsRequestHandler;
        private readonly IRequestHandler<GetTodoItemRequest, GetTodoItemResponse> _getTodoItemRequestHandler;
        private readonly IRequestHandler<CreateTodoItemRequest, CreateTodoItemResponse> _createTodoItemRequestHandler;
        private readonly IRequestHandler<DeleteTodoItemRequest, DeleteTodoItemResponse> _deleteTodoItemRequestHandler;
        private readonly TodoContext _context;
        private string TraceId => HttpContext.TraceIdentifier;

        public TodoItemsController(
            IRequestHandler<GetTodoItemsRequest, GetTodoItemsResponse> getTodoItemsRequestHandler,
            IRequestHandler<GetTodoItemRequest, GetTodoItemResponse> getTodoItemRequestHandler,
            IRequestHandler<CreateTodoItemRequest, CreateTodoItemResponse> createTodoItemRequestHandler,
            IRequestHandler<DeleteTodoItemRequest, DeleteTodoItemResponse> deleteTodoItemRequestHandler,
            TodoContext context)
        {
            _getTodoItemsRequestHandler = getTodoItemsRequestHandler;
            _getTodoItemRequestHandler = getTodoItemRequestHandler;
            _createTodoItemRequestHandler = createTodoItemRequestHandler;
            _deleteTodoItemRequestHandler = deleteTodoItemRequestHandler;
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTodoItems() => await Handle(_getTodoItemsRequestHandler.Handle, new GetTodoItemsRequest(), new CancellationToken());

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTodoItem(long id) => await Handle(_getTodoItemRequestHandler.Handle, new GetTodoItemRequest { Id = id }, new CancellationToken());

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
        public async Task<IActionResult> CreateTodoItem(TodoItem todoItem) => await Handle(_createTodoItemRequestHandler.Handle, new CreateTodoItemRequest { TodoItem = todoItem }, new CancellationToken());

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteTodoItem(long id) => await Handle(_deleteTodoItemRequestHandler.Handle, new DeleteTodoItemRequest { Id = id }, new CancellationToken());

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

        private bool TodoItemExists(long id) => _context.TodoItems.Any(e => e.Id == id);

        private Contracts.ApiMessage CreateApiMessage(HttpStatusCode httpStatusCode, IEnumerable<Notification> notifications = null) =>
            new Contracts.ApiMessage
            {
                TraceId = TraceId,
                Status = (int) httpStatusCode,
                Notifications = notifications
            };
    }
}
