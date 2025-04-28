using System;

namespace Backend.Models
{
    public class Todo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Provider { get; set; }

       
       public DateTime DueDate { get; set; }
       

       
        public Todo(string name, string description, string provider, DateTime dueDate)
        {
            Name = name;
            Description = description;
            Provider = provider;
            DueDate = dueDate;
        }
    }
}
