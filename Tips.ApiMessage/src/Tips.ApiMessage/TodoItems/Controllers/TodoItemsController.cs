using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tips.ApiMessage.Handlers;
using Tips.ApiMessage.TodoItems.Context;
using Tips.ApiMessage.TodoItems.CreateTodoItems;
using Tips.ApiMessage.TodoItems.DeleteTodoItems;
using Tips.ApiMessage.TodoItems.GetTodoItem;
using Tips.ApiMessage.TodoItems.GetTodoItems;
using Tips.ApiMessage.TodoItems.Models;
using Tips.ApiMessage.TodoItems.UpdateTodoItem;

namespace Tips.ApiMessage.TodoItems.Controllers
{
    // Created based on the Tutorial: Create a web API with ASP.NET Core
    // https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-3.1&tabs=visual-studio

    [Route("api/TodoItems")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly IRequestHandler<GetTodoItemsRequest, GetTodoItemsResponse> _getTodoItemsRequestHandler;
        private readonly IRequestHandler<GetTodoItemRequest, GetTodoItemResponse> _getTodoItemRequestHandler;
        private readonly IRequestHandler<CreateTodoItemRequest, CreateTodoItemResponse> _createTodoItemRequestHandler;
        private readonly IRequestHandler<DeleteTodoItemRequest, DeleteTodoItemResponse> _deleteTodoItemRequestHandler;
        private readonly IRequestHandler<UpdateTodoItemRequest, UpdateTodoItemResponse> _updateTodoItemRequestHandler;
        private readonly TodoContext _context;
        private string TraceId => HttpContext.TraceIdentifier;

        public TodoItemsController(
            IRequestHandler<GetTodoItemsRequest, GetTodoItemsResponse> getTodoItemsRequestHandler,
            IRequestHandler<GetTodoItemRequest, GetTodoItemResponse> getTodoItemRequestHandler,
            IRequestHandler<CreateTodoItemRequest, CreateTodoItemResponse> createTodoItemRequestHandler,
            IRequestHandler<DeleteTodoItemRequest, DeleteTodoItemResponse> deleteTodoItemRequestHandler,
            IRequestHandler<UpdateTodoItemRequest, UpdateTodoItemResponse> updateTodoItemRequestHandler,
            TodoContext context)
        {
            _getTodoItemsRequestHandler = getTodoItemsRequestHandler;
            _getTodoItemRequestHandler = getTodoItemRequestHandler;
            _createTodoItemRequestHandler = createTodoItemRequestHandler;
            _deleteTodoItemRequestHandler = deleteTodoItemRequestHandler;
            _updateTodoItemRequestHandler = updateTodoItemRequestHandler;
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTodoItems(bool asProblemDetails, CancellationToken cancellationToken)
        {
            // TODO: Log call to service.
            // TODO: Log request if provided.
            var response = await _getTodoItemsRequestHandler.Handle(new GetTodoItemsRequest(), cancellationToken);

            return response.Status switch
            {
                (int) HttpStatusCode.OK when asProblemDetails => Ok(response),
                (int) HttpStatusCode.OK => Ok(response.TodoItems),
                _ => throw new Exception($"HttpStatusCode {response.Status} was not handled.")
            };

            // TODO: Log actionResult
        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTodoItem(long id, bool asProblemDetails, CancellationToken cancellationToken)
        {
            // TODO: Log call to service.
            // TODO: Log request if provided.
            var response = await _getTodoItemRequestHandler.Handle(new GetTodoItemRequest { Id = id }, cancellationToken);

            return response.Status switch
            {
                (int) HttpStatusCode.NotFound => NotFound(),
                (int) HttpStatusCode.OK when asProblemDetails => Ok(response),
                (int) HttpStatusCode.OK => Ok(response.TodoItem),
                _ => throw new Exception($"HttpStatusCode {response.Status} was not handled.")
            };

            // TODO: Log actionResult
        }

        // PUT: api/TodoItems/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoItem(long id, bool asProblemDetails, TodoItem todoItem, CancellationToken cancellationToken)
        {
            // TODO: Log call to service.
            // TODO: Log request if provided.
            var response = await _updateTodoItemRequestHandler.Handle(new UpdateTodoItemRequest { Id = id, TodoItem = todoItem }, cancellationToken);

            return response.Status switch
            {
                (int) HttpStatusCode.BadRequest => BadRequest(response),
                (int) HttpStatusCode.NotFound => NotFound(),
                (int) HttpStatusCode.NoContent when asProblemDetails => Ok(response),
                (int) HttpStatusCode.NoContent => NoContent(),
                _ => throw new Exception($"HttpStatusCode {response.Status} was not handled.")
            };

            // TODO: Log actionResult
        }

        // POST: api/TodoItems
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateTodoItem(TodoItem todoItem, bool asProblemDetails, CancellationToken cancellationToken)
        {
            // TODO: Log call to service.
            // TODO: Log request if provided.
            var response = await _createTodoItemRequestHandler.Handle(new CreateTodoItemRequest { TodoItem = todoItem }, cancellationToken);

            return response.Status switch
            {
                (int) HttpStatusCode.BadRequest => BadRequest(response),
                (int) HttpStatusCode.Created when asProblemDetails => CreatedAtAction(nameof(GetTodoItem), new { id = response.TodoItem.Id }, response),
                (int) HttpStatusCode.Created => CreatedAtAction(nameof(GetTodoItem), new { id = response.TodoItem.Id }, response.TodoItem),
                _ => throw new Exception($"HttpStatusCode {response.Status} was not handled.")
            };
            
            // TODO: Log actionResult
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteTodoItem(long id, bool asProblemDetails, CancellationToken cancellationToken)
        {
            // TODO: Log call to service.
            // TODO: Log request if provided.
            var response = await _deleteTodoItemRequestHandler.Handle(new DeleteTodoItemRequest { Id = id }, cancellationToken);

            return response.Status switch
            {
                (int) HttpStatusCode.NotFound => NotFound(),
                (int) HttpStatusCode.NoContent when asProblemDetails => Ok(response),
                (int) HttpStatusCode.NoContent => NoContent(),
                _ => throw new Exception($"HttpStatusCode {response.Status} was not handled.")
            };
            
            // TODO: Log actionResult
        }
    }
}
