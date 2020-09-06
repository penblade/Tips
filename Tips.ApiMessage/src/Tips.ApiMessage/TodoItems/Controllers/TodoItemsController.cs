using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tips.ApiMessage.Contracts;
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
        public async Task<IActionResult> GetTodoItems([FromServices] IRequestHandler<GetTodoItemsRequest, Response<List<TodoItem>>> handler,
            bool withDetails, CancellationToken cancellationToken)
        {
            var request = new GetTodoItemsRequest();

            var response = await _loggingBehavior.Handle(request, cancellationToken, () => handler.Handle(request, cancellationToken));

            return response.Status switch
            {
                (int) HttpStatusCode.OK when withDetails => Ok(response),
                (int) HttpStatusCode.OK => Ok(response.Result),
                _ => UnhandledHttpStatusCode(response)
            };
        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTodoItem([FromServices] IRequestHandler<GetTodoItemRequest, Response<TodoItem>> handler,
            bool withDetails, long id, CancellationToken cancellationToken)
        {
            var request = new GetTodoItemRequest { Id = id };

            var response = await _loggingBehavior.Handle(request, cancellationToken, () => handler.Handle(request, cancellationToken));

            return response.Status switch
            {
                (int) HttpStatusCode.NotFound => NotFound(),
                (int) HttpStatusCode.OK when withDetails => Ok(response),
                (int) HttpStatusCode.OK => Ok(response.Result),
                _ => UnhandledHttpStatusCode(response)
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
        public async Task<IActionResult> UpdateTodoItem([FromServices] IRequestHandler<UpdateTodoItemRequest, Response> handler,
            bool withDetails, long id, TodoItem todoItem, CancellationToken cancellationToken)
        {
            var request = new UpdateTodoItemRequest { Id = id, TodoItem = todoItem };

            var response = await _loggingBehavior.Handle(request, cancellationToken, () => handler.Handle(request, cancellationToken));

            return response.Status switch
            {
                (int) HttpStatusCode.BadRequest => BadRequest(response),
                (int) HttpStatusCode.NotFound => NotFound(),
                (int) HttpStatusCode.NoContent when withDetails => Ok(response),
                (int) HttpStatusCode.NoContent => NoContent(),
                _ => UnhandledHttpStatusCode(response)
            };
        }

        // POST: api/TodoItems
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateTodoItem([FromServices] IRequestHandler<CreateTodoItemRequest, Response<TodoItem>> handler,
            bool withDetails, TodoItem todoItem, CancellationToken cancellationToken)
        {
            var request = new CreateTodoItemRequest { TodoItem = todoItem };

            var response = await _loggingBehavior.Handle(request, cancellationToken, () => handler.Handle(request, cancellationToken));

            return response.Status switch
            {
                (int) HttpStatusCode.BadRequest => BadRequest(response),
                (int) HttpStatusCode.Created when withDetails => CreatedAtAction(nameof(GetTodoItem), new { id = response.Result.Id }, response),
                (int) HttpStatusCode.Created => CreatedAtAction(nameof(GetTodoItem), new { id = response.Result.Id }, response.Result),
                _ => UnhandledHttpStatusCode(response)
            };
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteTodoItem([FromServices] IRequestHandler<DeleteTodoItemRequest, Response> handler,
            bool withDetails, long id, CancellationToken cancellationToken)
        {
            var request = new DeleteTodoItemRequest { Id = id };

            var response = await _loggingBehavior.Handle(request, cancellationToken, () => handler.Handle(request, cancellationToken));

            return response.Status switch
            {
                (int) HttpStatusCode.NotFound => NotFound(),
                (int) HttpStatusCode.NoContent when withDetails => Ok(response),
                (int) HttpStatusCode.NoContent => NoContent(),
                _ => UnhandledHttpStatusCode(response)
            };
        }

        private static IActionResult UnhandledHttpStatusCode(Response response) => throw new Exception($"HttpStatusCode {response.Status} was not handled.");
    }
}
