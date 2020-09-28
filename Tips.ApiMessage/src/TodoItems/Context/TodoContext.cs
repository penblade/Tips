using Microsoft.EntityFrameworkCore;
using Tips.TodoItems.Context.Models;

namespace Tips.TodoItems.Context
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options) : base(options)
        {
        }

        public DbSet<TodoItemEntity> TodoItems { get; set; }
    }
}
