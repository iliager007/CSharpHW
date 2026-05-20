namespace TaskHub.Models;

public class TaskStatistics
{
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int OverdueTasks { get; set; }
    public Dictionary<Priority, int> PriorityCounts { get; set; } = new();
}
