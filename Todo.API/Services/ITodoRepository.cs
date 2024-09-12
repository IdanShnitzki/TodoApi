using Todo.API.Models;

namespace Todo.API.Services
{
    public interface ITodoRepository
    {
        Task<IEnumerable<Todos>> GetTodosAsync();

        Task<Todos> GetTodoAsync(int id);
    }
}
