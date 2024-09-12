using Microsoft.EntityFrameworkCore;
using Todo.API.Data;
using Todo.API.Models;

namespace Todo.API.Services
{
    public class TodoRepository : ITodoRepository
    {
        private readonly TodoContext _context;

        public TodoRepository(TodoContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Todos> GetTodoAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Todos>> GetTodosAsync()
        {
            var todos = await _context.Todos.OrderBy(t => t.Id).ToListAsync();

            return todos; 
        }
    }
}
