using Backend.Models;
using Microsoft.EntityFrameworkCore; 
using Backend.Data;

namespace Backend.Services
{
    public class ProviderB : ITodoProvider
    {
        private readonly TodoContext _db;

        public ProviderB(TodoContext db)
        {
            _db = db;
        }

       
        public async Task AddTodoAsync(Todo todo)
        {
            todo.Provider = "B";  
            await _db.Todos.AddAsync(todo); 
            await _db.SaveChangesAsync(); 
            
        }

       
        public async Task DeleteTodoAsync(int todoId)
        {
            var todo = await _db.Todos.FindAsync(todoId);
            if (todo != null)
            {
                _db.Todos.Remove(todo); 
                await _db.SaveChangesAsync(); 
               
            }
           
        }

       
        public async Task UpdateTodoAsync(int todoId, Todo todo)
        {
            var existingTodo = await _db.Todos.FindAsync(todoId);
            if (existingTodo != null)
            {
                existingTodo.Name = todo.Name;
                existingTodo.Description = todo.Description;
                existingTodo.DueDate = todo.DueDate;
                await _db.SaveChangesAsync(); 
               
            }
           
        }
    }
}
