using Microsoft.EntityFrameworkCore;

namespace Tips.ApiMessage.TodoItems.Context
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options) : base(options)
        {
        }

        public DbSet<TodoItemEntity> TodoItems { get; set; }
    }
}
