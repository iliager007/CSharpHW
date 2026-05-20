namespace TaskHub.Models;

// OOP: TaskItem is a class that stores all data for one task.
public class TaskItem : IIdentifiable
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Priority Priority { get; set; }
    public DateTime Deadline { get; set; }
    public TaskStatus Status { get; set; }
}
