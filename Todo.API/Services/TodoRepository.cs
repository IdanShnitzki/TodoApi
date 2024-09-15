using Microsoft.EntityFrameworkCore;
using Todo.API.Data;
using Todo.API.Dtos;
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

        public async Task<TodoEntity> GetTodoAsync(int id)
        {
            return await _context.Todos.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<TodoEntity>> GetTodosAsync()
        {
            return await _context.Todos.OrderBy(t => t.Id).ToListAsync();
        }

        public void Create(TodoEntity todoEntity)
        {
            _context.Todos.Add(todoEntity);
        }

        public void Delete(TodoEntity todoEntity)
        {
            _context.Todos.Remove(todoEntity);
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() > 0;
        }

        public async  Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() > 0);
        }
    }
}
