using TaskHub.Models;

namespace TaskHub.Services;

// Delegate example: the manager can accept reusable task filtering logic.
public delegate bool TaskFilter(TaskItem task);
