using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tips.TodoItems.Context;
using Tips.TodoItems.Context.Models;
using Tips.TodoItems.Tests.Support;

namespace Tips.TodoItems.Tests.Context
{
    public abstract class WithContext
    {
        // Inspiration: https://www.michalbialecki.com/2020/11/28/unit-tests-in-entity-framework-core-5/
        // This article gives a nice overview on how to unit test
        // with an in-memory database as recommended by Microsoft.
        // https://www.michalbialecki.com/2020/11/28/unit-tests-in-entity-framework-core-5/

        // In-memory databases perform fairly well for an
        // integration test.  Be aware of these issues
        // and gotchas before deciding to use an in-memory
        // database.  For these simple tests, this makes sense.

        // Microsoft recommends the following.
        // Instead we use the EF in-memory database when
        // unit testing something that uses DbContext.
        // In this case using the EF in-memory database is
        // appropriate because the test is not dependent on
        // database behavior. Just don't do this to test actual
        // database queries or updates.
        // https://docs.microsoft.com/en-us/ef/core/testing/

        // Microsoft also mentions that they use test doubles.
        // Consider testing a piece of business logic that might
        // need to use some data from a database, but is not
        // inherently testing the database interactions. One
        // option is to use a test double such as a mock or fake.

        // We use test doubles for internal testing of EF Core.
        // However, we never try to mock DbContext or IQueryable.
        // Doing so is difficult, cumbersome, and fragile. Don't do it.
        // This article gives a good example for making Fakes.
        // https://stackoverflow.com/questions/25960192/mocking-ef-dbcontext-with-moq

        // There are other nice articles if you want to mock
        // the DbContext and DbSet.  This is a fine approach
        // and avoids many of the pitfalls with different
        // database providers as also detailed by Microsoft.
        // Microsoft notes in the previous article that
        // mocking DbContext or IQueryable is difficult,
        // cumbersome, and fragile.  Don't do it.
        // Take that into consideration when reviewing this.
        // https://medium.com/@briangoncalves/dbcontext-dbset-mock-for-unit-test-in-c-with-moq-db5c270e68f3

        // In the end, it depends.  Use the test mechanism
        // that makes the most sense for your testing needs
        // whether your focused on simplicity, maintenance,
        // performance, or another need.

        internal TodoContext Context { get; private set; }

        [TestInitialize]
        public void Setup() => Context = CreateTodoContext();

        [TestCleanup]
        // Delete the entries in the database whether
        // the test passes, fails, or throws an exception.
        // The database Identity is not reset.
        // If that matters, then create a new database
        // with a using and a random guid for each test.
        // https://stackoverflow.com/questions/33490696/how-can-i-reset-an-ef7-inmemory-provider-between-unit-tests
        public void TearDown() => Context.Database.EnsureDeleted();

        private static TodoContext CreateTodoContext() => new(CreateDbContextOptions());

        private static DbContextOptions<TodoContext> CreateDbContextOptions() =>
            new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase(databaseName: "TodoList")
                .Options;

        internal async Task<List<TodoItemEntity>> PopulateTodoItems(int totalItems)
        {
            var todoItemEntities = TodoItemFactory.CreateTodoItemEntities(totalItems).ToList();
            await Context.AddRangeAsync(todoItemEntities);
            await Context.SaveChangesAsync();

            return todoItemEntities;
        }

        internal async Task<TodoItemEntity> GetTodoItem(int requestId) => await Context.TodoItems.SingleOrDefaultAsync(todoItem => todoItem.Id == requestId);
    }
}
