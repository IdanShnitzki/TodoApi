using Microsoft.EntityFrameworkCore;
using Todo.API.Models;

namespace Todo.API.Data
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options) : base(options)
        {
            
        }

        public DbSet<Todos> Todos { get; set; }
    }
}
