using Backend.Models;
using Backend.Services;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// code for database conection
builder.Services.AddDbContext<TodoContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("MySqlConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySqlConnection"))
    )
);




builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cors setup to contact frontend
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:8081")  
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors(); 
app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();




//all API calls were inculded here instead of seperate file

// To get all todos for a provider
app.MapGet("/todo/{provider}/all", async (string provider, TodoContext db) =>
{
    var todos = await db.Todos
        .Where(t => t.Provider == provider)
        .ToListAsync();

    return Results.Ok(todos);
});


// To get the todo detials when searched
app.MapGet("/todo/{provider}/search/{name}", async (string provider, string name, TodoContext db) =>
{
    
    var todo = await db.Todos
        .Where(t => t.Provider == provider && t.Name.ToLower() == name.ToLower())
        .FirstOrDefaultAsync();

    if (todo == null)
    {
        return Results.NotFound(new { message = "Todo not found" }); 
    }

    return Results.Ok(todo); 
});



// to add the todo
app.MapPost("/todo/{provider}", async (string provider, [FromBody] Todo todo, TodoContext db) =>
{
   var todoProvider = TodoProviderFactory.GetTodoProvider(provider, db); 
   await todoProvider.AddTodoAsync(todo);
   return Results.Ok(todo);
});

// to update todo
app.MapPut("/todo/{provider}/{id}", async (string provider, int id, [FromBody] Todo todo, TodoContext db) =>
{
   var todoProvider = TodoProviderFactory.GetTodoProvider(provider, db);
   await todoProvider.UpdateTodoAsync(id, todo);
   return Results.Ok(todo);
});

// To delete todo
app.MapDelete("/todo/{provider}/{id}", async (string provider, int id, TodoContext db) =>
{
    var todoProvider = TodoProviderFactory.GetTodoProvider(provider, db);

    await todoProvider.DeleteTodoAsync(id);
    return Results.NoContent();
});



app.Run();
