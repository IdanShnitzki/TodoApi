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
        private readonly PaginationMetadata _paginationMetadata;

        public TodoRepository(TodoContext context, PaginationMetadata paginationMetadata)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _paginationMetadata = paginationMetadata ?? throw new ArgumentNullException(nameof(paginationMetadata));
        }

        public async Task<(IEnumerable<TodoEntity>, PaginationMetadata)> GetTodosAsync(string? title, string? searchQuery, int requestedPage, int pageSize)
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

            _paginationMetadata.TotalItemCount = await collection.CountAsync();
            _paginationMetadata.PageSize = pageSize;
            _paginationMetadata.CurrentPage = requestedPage;
            _paginationMetadata.TotalPageCount = (int)Math.Ceiling((_paginationMetadata.TotalItemCount / (double)pageSize));

            var collectionToReturn = await collection.OrderBy(t => t.Id)
                .Skip(pageSize * (requestedPage - 1))
                .Take(pageSize)
                .ToListAsync();

            return (collectionToReturn, _paginationMetadata);
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
