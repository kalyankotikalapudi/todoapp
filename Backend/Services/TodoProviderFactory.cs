using Backend.Models;
using Backend.Services;
using Backend.Data;

namespace Backend.Services{
public static class TodoProviderFactory{      
  public static ITodoProvider GetTodoProvider(string providerType, TodoContext db){
            return providerType switch{
                "A" => new ProviderA(db),  
                "B" => new ProviderB(db),  
                _ => throw new ArgumentException("Invalid provider type")
            };
    }
}
}
