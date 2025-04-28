using Backend.Models;
using Microsoft.EntityFrameworkCore; 
using Backend.Data;

namespace Backend.Services
{
    public class ProviderA : ITodoProvider
    {
        private readonly TodoContext _db;

        public ProviderA(TodoContext db)
        {
            _db = db; // here injecting  todoContext to interact with db
        }

        // For adding todo
        public async Task AddTodoAsync(Todo todo)
        {
            todo.Provider = "A";  
            await _db.Todos.AddAsync(todo); 
            await _db.SaveChangesAsync(); 
            
        }

        // for deleting todo
        public async Task DeleteTodoAsync(int todoId)
        {
            var todo = await _db.Todos.FindAsync(todoId);
            if (todo != null)
            {
                _db.Todos.Remove(todo); 
                await _db.SaveChangesAsync();
                
            }
           
        }

        // for updating todo
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
