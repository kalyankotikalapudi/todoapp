using Backend.Models; 

namespace Backend.Services
{
    public interface ITodoProvider
    {
        Task AddTodoAsync(Todo todo);
        Task DeleteTodoAsync(int todoId);
        Task UpdateTodoAsync(int todoId, Todo todo);
    }
}
