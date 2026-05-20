using TaskHub.Data;
using TaskHub.Models;

namespace TaskHub.Services;

// Business logic for creating, editing, searching, statistics, and task access.
public class TaskManager
{
    private readonly TaskRepository<TaskItem> _repository = new();
    private readonly object _lock = new();

    public TaskItem CreateTask(string title, string description, Priority priority, DateTime deadline)
    {
        TaskItem task = new()
        {
            Title = title,
            Description = description,
            Priority = priority,
            Deadline = deadline,
            Status = Models.TaskStatus.New
        };

        lock (_lock)
        {
            _repository.Add(task);
        }

        return task;
    }

    public List<TaskItem> GetAllTasks()
    {
        lock (_lock)
        {
            return _repository.Items.Select(CloneTask).ToList();
        }
    }

    public List<TaskItem> GetTasks(TaskFilter filter)
    {
        lock (_lock)
        {
            return _repository.Items.Where(task => filter(task)).Select(CloneTask).ToList();
        }
    }

    public TaskItem? GetTaskById(int id)
    {
        lock (_lock)
        {
            TaskItem? task = _repository.GetById(id);
            return task is null ? null : CloneTask(task);
        }
    }

    public bool EditTask(int id, string? title, string? description, Priority? priority, Models.TaskStatus? status)
    {
        lock (_lock)
        {
            TaskItem? task = _repository.GetById(id);
            if (task is null)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(title))
            {
                task.Title = title;
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                task.Description = description;
            }

            if (priority.HasValue)
            {
                task.Priority = priority.Value;
            }

            if (status.HasValue)
            {
                task.Status = status.Value;
            }

            return true;
        }
    }

    public bool DeleteTask(int id)
    {
        lock (_lock)
        {
            return _repository.RemoveById(id);
        }
    }

    public List<TaskItem> SearchByTitle(string title)
    {
        return GetTasks(task => task.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
    }

    public List<TaskItem> SearchByStatus(Models.TaskStatus status)
    {
        return GetTasks(task => task.Status == status);
    }

    public List<TaskItem> SearchByPriority(Priority priority)
    {
        return GetTasks(task => task.Priority == priority);
    }

    public Dictionary<Priority, int> GetPriorityCounts()
    {
        lock (_lock)
        {
            // Dictionary<TKey, TValue> requirement: stores count of tasks by priority.
            Dictionary<Priority, int> counts = Enum.GetValues<Priority>().ToDictionary(priority => priority, _ => 0);

            foreach (TaskItem task in _repository.Items)
            {
                counts[task.Priority]++;
            }

            return counts;
        }
    }

    public TaskStatistics GetStatistics()
    {
        lock (_lock)
        {
            DateTime now = DateTime.Now;
            return new TaskStatistics
            {
                TotalTasks = _repository.Items.Count,
                CompletedTasks = _repository.Items.Count(task => task.Status == Models.TaskStatus.Done),
                OverdueTasks = _repository.Items.Count(task => task.Status != Models.TaskStatus.Done && task.Deadline < now),
                PriorityCounts = GetPriorityCounts()
            };
        }
    }

    public void ReplaceTasks(IEnumerable<TaskItem> tasks)
    {
        lock (_lock)
        {
            _repository.ReplaceAll(tasks);
        }
    }

    private static TaskItem CloneTask(TaskItem task)
    {
        return new TaskItem
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Priority = task.Priority,
            Deadline = task.Deadline,
            Status = task.Status
        };
    }
}
