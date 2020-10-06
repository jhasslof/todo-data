using Microsoft.EntityFrameworkCore;
using todo.db.api.Models;

namespace todo.db.api.Database
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        {
        }

        public DbSet<TodoItem> TodoItems { get; set; }
    }
}
