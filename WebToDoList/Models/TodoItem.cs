namespace TodoApi.Models;

public enum TaskStatusNow
{
    ToDo,
    InProgress,
    Done
}

public class TodoItem
{
    public long Id { get; set; }
    public string? Name { get; set; }
    
    public TaskStatusNow Status { get; set; } 
}