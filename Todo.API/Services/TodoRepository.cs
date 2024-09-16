using Microsoft.EntityFrameworkCore;
using System.Linq;
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

        public async Task<IEnumerable<TodoEntity>> GetTodosAsync(string? title, string? searchQuery)
        {
            var collection = _context.Todos as IQueryable<TodoEntity>;

            if (!string.IsNullOrWhiteSpace(title))
            {
                title = title.Trim();
                collection = collection.Where(t => t.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                collection = collection.Where(t => t.Title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                             (!string.IsNullOrEmpty(t.Description) && t.Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)));
            }

            return await collection.OrderBy(t => t.Id).ToListAsync();
        }

        public async Task<TodoEntity> GetTodoAsync(int id)
        {
            return await _context.Todos.FirstOrDefaultAsync(t => t.Id == id);
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
