using TaskHub.Models;
using TaskHub.Services;
using TaskHub.Utilities;

namespace TaskHub;

public class Program
{
    private const string DefaultFilePath = "tasks.json";

    public static async Task Main()
    {
        TaskManager taskManager = new();
        using FileService fileService = new();
        using CancellationTokenSource cancellationTokenSource = new();

        // Multithreading/background Task requirement with CancellationToken.
        Task deadlineChecker = StartDeadlineCheckerAsync(taskManager, cancellationTokenSource.Token);

        bool exit = false;

        while (!exit)
        {
            ShowMenu();
            string? choice = Console.ReadLine();
            Console.WriteLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        CreateTask(taskManager);
                        break;
                    case "2":
                        ViewTasksMenu(taskManager);
                        break;
                    case "3":
                        EditTask(taskManager);
                        break;
                    case "4":
                        DeleteTask(taskManager);
                        break;
                    case "5":
                        SearchTasks(taskManager);
                        break;
                    case "6":
                        ShowStatistics(taskManager);
                        break;
                    case "7":
                        await SaveTasksAsync(taskManager, fileService);
                        break;
                    case "8":
                        await LoadTasksAsync(taskManager, fileService);
                        break;
                    case "0":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Unknown menu option.");
                        break;
                }
            }
            catch (Exception ex)
            {
                // Exception handling requirement: errors are caught and shown without crashing the app.
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine();
        }

        cancellationTokenSource.Cancel();
        await deadlineChecker;
        Console.WriteLine("TaskHub closed.");
    }

    private static void ShowMenu()
    {
        Console.WriteLine("==== TaskHub ====");
        Console.WriteLine("1. Create task");
        Console.WriteLine("2. View tasks");
        Console.WriteLine("3. Edit task");
        Console.WriteLine("4. Delete task");
        Console.WriteLine("5. Search tasks");
        Console.WriteLine("6. Statistics");
        Console.WriteLine("7. Save tasks to file");
        Console.WriteLine("8. Load tasks from file");
        Console.WriteLine("0. Exit");
        Console.Write("Choose an option: ");
    }

    private static void CreateTask(TaskManager taskManager)
    {
        string title = ConsoleHelper.ReadRequiredString("Title: ");
        string description = ConsoleHelper.ReadRequiredString("Description: ");
        Priority priority = ConsoleHelper.ReadEnum<Priority>("Priority:");
        DateTime deadline = ConsoleHelper.ReadDateTime("Deadline (example 2026-05-20 18:30): ");

        TaskItem task = taskManager.CreateTask(title, description, priority, deadline);
        Console.WriteLine($"Task created with ID {task.Id}.");
    }

    private static void ViewTasksMenu(TaskManager taskManager)
    {
        Console.WriteLine("1. All tasks");
        Console.WriteLine("2. Completed tasks");
        Console.WriteLine("3. Incomplete tasks");
        Console.WriteLine("4. High priority tasks");
        Console.Write("Choose: ");

        string? choice = Console.ReadLine();
        List<TaskItem> tasks = choice switch
        {
            "1" => taskManager.GetAllTasks(),
            "2" => taskManager.GetTasks(task => task.Status == Models.TaskStatus.Done),
            "3" => taskManager.GetTasks(task => task.Status != Models.TaskStatus.Done),
            "4" => taskManager.GetTasks(task => task.Priority == Priority.High),
            _ => new List<TaskItem>()
        };

        ConsoleHelper.DisplayTasks(tasks);
    }

    private static void EditTask(TaskManager taskManager)
    {
        ConsoleHelper.DisplayTasks(taskManager.GetAllTasks());
        int id = ConsoleHelper.ReadInt("Enter task ID to edit: ");

        TaskItem? existingTask = taskManager.GetTaskById(id);
        if (existingTask is null)
        {
            Console.WriteLine("Task not found.");
            return;
        }

        Console.WriteLine("Press Enter to keep the current value.");
        string? title = ConsoleHelper.ReadOptionalString($"Title ({existingTask.Title}): ");
        string? description = ConsoleHelper.ReadOptionalString($"Description ({existingTask.Description}): ");
        Priority? priority = ConsoleHelper.ReadOptionalEnum<Priority>($"Priority ({existingTask.Priority}):");
        Models.TaskStatus? status = ConsoleHelper.ReadOptionalEnum<Models.TaskStatus>($"Status ({existingTask.Status}):");

        bool updated = taskManager.EditTask(id, title, description, priority, status);
        Console.WriteLine(updated ? "Task updated." : "Task not found.");
    }

    private static void DeleteTask(TaskManager taskManager)
    {
        List<TaskItem> tasks = taskManager.GetAllTasks();
        ConsoleHelper.DisplayTasks(tasks);

        int input = ConsoleHelper.ReadInt("Enter task ID or displayed number to delete: ");
        int id = input;

        if (tasks.Any(task => task.Id == input) is false && input >= 1 && input <= tasks.Count)
        {
            id = tasks[input - 1].Id;
        }

        bool deleted = taskManager.DeleteTask(id);
        Console.WriteLine(deleted ? "Task deleted." : "Task not found.");
    }

    private static void SearchTasks(TaskManager taskManager)
    {
        Console.WriteLine("1. Search by title");
        Console.WriteLine("2. Search by status");
        Console.WriteLine("3. Search by priority");
        Console.Write("Choose: ");

        string? choice = Console.ReadLine();
        List<TaskItem> results = choice switch
        {
            "1" => taskManager.SearchByTitle(ConsoleHelper.ReadRequiredString("Title search text: ")),
            "2" => taskManager.SearchByStatus(ConsoleHelper.ReadEnum<Models.TaskStatus>("Status:")),
            "3" => taskManager.SearchByPriority(ConsoleHelper.ReadEnum<Priority>("Priority:")),
            _ => new List<TaskItem>()
        };

        ConsoleHelper.DisplayTasks(results);
    }

    private static void ShowStatistics(TaskManager taskManager)
    {
        TaskStatistics statistics = taskManager.GetStatistics();

        Console.WriteLine($"Total tasks: {statistics.TotalTasks}");
        Console.WriteLine($"Completed tasks: {statistics.CompletedTasks}");
        Console.WriteLine($"Overdue tasks: {statistics.OverdueTasks}");
        Console.WriteLine("Tasks by priority:");

        foreach (KeyValuePair<Priority, int> pair in statistics.PriorityCounts)
        {
            Console.WriteLine($"- {pair.Key}: {pair.Value}");
        }
    }

    private static async Task SaveTasksAsync(TaskManager taskManager, FileService fileService)
    {
        string? filePath = ConsoleHelper.ReadOptionalString($"File path ({DefaultFilePath}): ");
        filePath ??= DefaultFilePath;

        await fileService.SaveTasksAsync(filePath, taskManager.GetAllTasks());
        Console.WriteLine($"Tasks saved to {filePath}.");
    }

    private static async Task LoadTasksAsync(TaskManager taskManager, FileService fileService)
    {
        string? filePath = ConsoleHelper.ReadOptionalString($"File path ({DefaultFilePath}): ");
        filePath ??= DefaultFilePath;

        List<TaskItem> tasks = await fileService.LoadTasksAsync(filePath);
        taskManager.ReplaceTasks(tasks);
        Console.WriteLine($"Loaded {tasks.Count} task(s) from {filePath}.");
    }

    private static async Task StartDeadlineCheckerAsync(TaskManager taskManager, CancellationToken cancellationToken)
    {
        HashSet<int> notifiedTaskIds = new();

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                DateTime now = DateTime.Now;
                List<TaskItem> overdueTasks = taskManager.GetTasks(task => task.Status != Models.TaskStatus.Done && task.Deadline < now);

                foreach (TaskItem task in overdueTasks)
                {
                    if (notifiedTaskIds.Add(task.Id))
                    {
                        Console.WriteLine();
                        Console.WriteLine($"[Deadline Alert] Task #{task.Id} \"{task.Title}\" is overdue. Deadline was {task.Deadline:g}.");
                        Console.Write("Choose an option: ");
                    }
                }

                notifiedTaskIds.RemoveWhere(id => overdueTasks.All(task => task.Id != id));
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when the application exits.
        }
    }
}
