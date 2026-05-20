using TaskHub.Models;

namespace TaskHub.Utilities;

// Static class requirement: helper methods for console input and output.
public static class ConsoleHelper
{
    public static string ReadRequiredString(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(input))
            {
                return input.Trim();
            }

            Console.WriteLine("Value cannot be empty.");
        }
    }

    public static string? ReadOptionalString(string prompt)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();
        return string.IsNullOrWhiteSpace(input) ? null : input.Trim();
    }

    public static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int value))
            {
                return value;
            }

            Console.WriteLine("Please enter a valid number.");
        }
    }

    public static DateTime ReadDateTime(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (DateTime.TryParse(Console.ReadLine(), out DateTime value))
            {
                return value;
            }

            Console.WriteLine("Please enter a valid date and time. Example: 2026-05-20 18:30");
        }
    }

    public static TEnum ReadEnum<TEnum>(string prompt) where TEnum : struct, Enum
    {
        while (true)
        {
            Console.WriteLine(prompt);
            foreach (TEnum value in Enum.GetValues<TEnum>())
            {
                Console.WriteLine($"{Convert.ToInt32(value)}. {value}");
            }

            Console.Write("Choose: ");
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int number) && Enum.IsDefined(typeof(TEnum), number))
            {
                return (TEnum)Enum.ToObject(typeof(TEnum), number);
            }

            if (Enum.TryParse(input, ignoreCase: true, out TEnum enumValue))
            {
                return enumValue;
            }

            Console.WriteLine("Invalid choice.");
        }
    }

    public static TEnum? ReadOptionalEnum<TEnum>(string prompt) where TEnum : struct, Enum
    {
        Console.WriteLine(prompt);
        Console.WriteLine("Press Enter to keep current value.");
        foreach (TEnum value in Enum.GetValues<TEnum>())
        {
            Console.WriteLine($"{Convert.ToInt32(value)}. {value}");
        }

        Console.Write("Choose: ");
        string? input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        if (int.TryParse(input, out int number) && Enum.IsDefined(typeof(TEnum), number))
        {
            return (TEnum)Enum.ToObject(typeof(TEnum), number);
        }

        if (Enum.TryParse(input, ignoreCase: true, out TEnum enumValue))
        {
            return enumValue;
        }

        Console.WriteLine("Invalid choice. Current value will be kept.");
        return null;
    }

    public static void DisplayTasks(IEnumerable<TaskItem> tasks)
    {
        List<TaskItem> list = tasks.ToList();
        if (list.Count == 0)
        {
            Console.WriteLine("No tasks found.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("No. | ID | Title | Priority | Status | Deadline");
        Console.WriteLine(new string('-', 78));

        for (int i = 0; i < list.Count; i++)
        {
            TaskItem task = list[i];
            Console.WriteLine($"{i + 1,3} | {task.Id,2} | {task.Title} | {task.Priority} | {task.Status} | {task.Deadline:g}");
            Console.WriteLine($"      Description: {task.Description}");
        }
    }
}
