using Todo.API.Dtos;
using Todo.API.Models;

namespace Todo.API.Services
{
    public interface ITodoRepository
    {
        Task<IEnumerable<TodoEntity>> GetTodosAsync();
        Task<TodoEntity> GetTodoAsync(int id);
        void Create(TodoEntity todoEntity);
        void Delete(TodoEntity todoEntity);
        bool SaveChanges();
        Task<bool> SaveChangesAsync();
    }
}
