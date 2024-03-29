﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tips.Middleware.ErrorHandling;
using Tips.Pipeline;
using Tips.TodoItems.Handlers.CreateTodoItem;
using Tips.TodoItems.Handlers.DeleteTodoItem;
using Tips.TodoItems.Handlers.GetTodoItem;
using Tips.TodoItems.Handlers.GetTodoItems;
using Tips.TodoItems.Handlers.UpdateTodoItem;
using Tips.TodoItems.Models;

namespace Tips.Api.Controllers
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

        /// <summary>
        /// Get a list of to do items.
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTodoItems(
            [FromServices] IRequestHandler<GetTodoItemsRequest, Response<List<TodoItem>>> handler)
        {
            var request = new GetTodoItemsRequest();

            var response = await HandleAsync(handler, request);

            return Ok(response.Item);
        }

        /// <summary>
        /// Get a single to do item.
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="id"></param>
        /// <returns>A to do item</returns>
        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTodoItem(
            [FromServices] IRequestHandler<GetTodoItemRequest, Response<TodoItem>> handler,
            long id)
        {
            var request = new GetTodoItemRequest { Id = id };

            var response = await HandleAsync(handler, request);

            if (response.IsNotFound()) return NotFound();

            return Ok(response.Item);
        }

        /// <summary>
        /// Update a single to do item.
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="problemDetailsFactory"></param>
        /// <param name="id"></param>
        /// <param name="todoItem"></param>
        /// <returns></returns>
        // PUT: api/TodoItems/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoItem(
            [FromServices] IRequestHandler<UpdateTodoItemRequest, Response> handler,
            [FromServices] IProblemDetailsFactory problemDetailsFactory,
            long id, TodoItem todoItem)
        {
            var request = new UpdateTodoItemRequest { Id = id, Item = todoItem };

            var response = await HandleAsync(handler, request);

            if (response.IsNotFound()) return NotFound();

            if (response.HasErrors()) return BadRequest(problemDetailsFactory.BadRequest(response.Notifications));

            return NoContent();
        }


        /// <summary>
        /// Create a to do item.
        /// </summary>
        /// <param name="handler">The create to do item handler</param>
        /// <param name="problemDetailsFactory"></param>
        /// <param name="todoItem"></param>
        /// <returns></returns>
        // POST: api/TodoItems
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateTodoItem(
            [FromServices] IRequestHandler<CreateTodoItemRequest, Response<TodoItem>> handler,
            [FromServices] IProblemDetailsFactory problemDetailsFactory,
            TodoItem todoItem)
        {
            var request = new CreateTodoItemRequest { Item = todoItem };

            var response = await HandleAsync(handler, request);

            if (response.HasErrors()) return BadRequest(problemDetailsFactory.BadRequest(response.Notifications));

            return CreatedAtAction(nameof(GetTodoItem), new {id = response.Item.Id}, response.Item);
        }

        /// <summary>
        /// Delete a to do item.
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteTodoItem(
            [FromServices] IRequestHandler<DeleteTodoItemRequest, Response> handler,
            long id)
        {
            var request = new DeleteTodoItemRequest { Id = id };

            var response = await HandleAsync(handler, request);

            if (response.IsNotFound()) return NotFound();

            return NoContent();
        }

        private async Task<TResponse> HandleAsync<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler, TRequest request) =>
            await _loggingBehavior.HandleAsync(request, () => handler.HandleAsync(request));
    }
}
