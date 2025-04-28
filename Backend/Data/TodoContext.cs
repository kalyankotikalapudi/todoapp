
using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options) : base(options)
        {
           
            Todos = Set<Todo>();  
        }

        public DbSet<Todo> Todos { get; set; }
    }
}
