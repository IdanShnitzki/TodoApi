using Todo.API.Dtos;
using Todo.API.Models;

namespace Todo.API.Services
{
    public interface ITodoRepository
    {
        Task<IEnumerable<TodoEntity>> GetTodosAsync();
        Task<TodoEntity> GetTodoAsync(int id);
        void CreateTodo(TodoEntity todoCreateDto);
        bool SaveChanges();
        Task<bool> SaveChangesAsync();
    }
}
