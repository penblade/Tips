using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tips.ApiMessage.Pipeline;
using Tips.ApiMessage.TodoItems.CreateTodoItems;
using Tips.ApiMessage.TodoItems.DeleteTodoItems;
using Tips.ApiMessage.TodoItems.GetTodoItem;
using Tips.ApiMessage.TodoItems.GetTodoItems;
using Tips.ApiMessage.TodoItems.Models;
using Tips.ApiMessage.TodoItems.UpdateTodoItem;

namespace Tips.ApiMessage.TodoItems.Controllers
{
    // Initially created based on the Tutorial: Create a web API with ASP.NET Core
    // https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-3.1&tabs=visual-studio

    // [FromServices] attribute is recommended to reduce constructor bloat by injecting directly into the method that uses the dependency.
    // https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/dependency-injection?view=aspnetcore-3.1#action-injection-with-fromservices

    [Route("api/TodoItems")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly IPipelineBehavior _loggingBehavior;

        public TodoItemsController(IPipelineBehavior loggingBehavior) => _loggingBehavior = loggingBehavior;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTodoItems([FromServices] IRequestHandler<GetTodoItemsRequest, GetTodoItemsResponse> handler,
            bool asProblemDetails, CancellationToken cancellationToken)
        {
            var request = new GetTodoItemsRequest();

            var response = await _loggingBehavior.Handle(request, cancellationToken, () => handler.Handle(request, cancellationToken));

            return response.Status switch
            {
                (int) HttpStatusCode.OK when asProblemDetails => Ok(response),
                (int) HttpStatusCode.OK => Ok(response.TodoItems),
                _ => throw new Exception($"HttpStatusCode {response.Status} was not handled.")
            };
        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTodoItem([FromServices] IRequestHandler<GetTodoItemRequest, GetTodoItemResponse> handler,
            long id, bool asProblemDetails, CancellationToken cancellationToken)
        {
            var request = new GetTodoItemRequest { Id = id };

            var response = await _loggingBehavior.Handle(request, cancellationToken, () => handler.Handle(request, cancellationToken));

            return response.Status switch
            {
                (int) HttpStatusCode.NotFound => NotFound(),
                (int) HttpStatusCode.OK when asProblemDetails => Ok(response),
                (int) HttpStatusCode.OK => Ok(response.TodoItem),
                _ => throw new Exception($"HttpStatusCode {response.Status} was not handled.")
            };
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
        public async Task<IActionResult> UpdateTodoItem([FromServices] IRequestHandler<UpdateTodoItemRequest, UpdateTodoItemResponse> handler,
            long id, bool asProblemDetails, TodoItem todoItem, CancellationToken cancellationToken)
        {
            var request = new UpdateTodoItemRequest { Id = id, TodoItem = todoItem };

            var response = await _loggingBehavior.Handle(request, cancellationToken, () => handler.Handle(request, cancellationToken));

            return response.Status switch
            {
                (int) HttpStatusCode.BadRequest => BadRequest(response),
                (int) HttpStatusCode.NotFound => NotFound(),
                (int) HttpStatusCode.NoContent when asProblemDetails => Ok(response),
                (int) HttpStatusCode.NoContent => NoContent(),
                _ => throw new Exception($"HttpStatusCode {response.Status} was not handled.")
            };
        }

        // POST: api/TodoItems
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateTodoItem([FromServices] IRequestHandler<CreateTodoItemRequest, CreateTodoItemResponse> handler,
            TodoItem todoItem, bool asProblemDetails, CancellationToken cancellationToken)
        {
            var request = new CreateTodoItemRequest { TodoItem = todoItem };

            var response = await _loggingBehavior.Handle(request, cancellationToken, () => handler.Handle(request, cancellationToken));

            return response.Status switch
            {
                (int) HttpStatusCode.BadRequest => BadRequest(response),
                (int) HttpStatusCode.Created when asProblemDetails => CreatedAtAction(nameof(GetTodoItem), new { id = response.TodoItem.Id }, response),
                (int) HttpStatusCode.Created => CreatedAtAction(nameof(GetTodoItem), new { id = response.TodoItem.Id }, response.TodoItem),
                _ => throw new Exception($"HttpStatusCode {response.Status} was not handled.")
            };
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteTodoItem([FromServices] IRequestHandler<DeleteTodoItemRequest, DeleteTodoItemResponse> handler,
            long id, bool asProblemDetails, CancellationToken cancellationToken)
        {
            var request = new DeleteTodoItemRequest { Id = id };

            var response = await _loggingBehavior.Handle(request, cancellationToken, () => handler.Handle(request, cancellationToken));

            return response.Status switch
            {
                (int) HttpStatusCode.NotFound => NotFound(),
                (int) HttpStatusCode.NoContent when asProblemDetails => Ok(response),
                (int) HttpStatusCode.NoContent => NoContent(),
                _ => throw new Exception($"HttpStatusCode {response.Status} was not handled.")
            };
        }
    }
}
