using TodoApi.Models;

namespace WebToDoList.DTOs;

public class TodoItemDTO
{
    public string? Name { get; set; }
    
    public TaskStatusNow Status { get; set; }
    
}
