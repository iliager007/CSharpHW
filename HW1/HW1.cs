using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("Простой калькулятор. Введите 'q' вместо числа, чтобы выйти.");

        while (true)
        {
            Console.Write("Введите первое число (или 'q' для выхода): ");
            string input1 = Console.ReadLine();

            if (input1 == "q")
                break;

            if (!double.TryParse(input1, out double a))
            {
                Console.WriteLine("Ошибка: нужно ввести число.");
                continue;
            }

            Console.Write("Введите второе число: ");
            string input2 = Console.ReadLine();

            if (!double.TryParse(input2, out double b))
            {
                Console.WriteLine("Ошибка: нужно ввести число.");
                continue;
            }

            Console.Write("Выберите операцию (+, -, *, /): ");
            string op = Console.ReadLine();

            double result;

            switch (op)
            {
                case "+":
                    result = a + b;
                    Console.WriteLine($"Результат: {result}");
                    break;

                case "-":
                    result = a - b;
                    Console.WriteLine($"Результат: {result}");
                    break;

                case "*":
                    result = a * b;
                    Console.WriteLine($"Результат: {result}");
                    break;

                case "/":
                    if (b == 0)
                    {
                        Console.WriteLine("Ошибка: деление на ноль.");
                    }
                    else
                    {
                        result = a / b;
                        Console.WriteLine($"Результат: {result}");
                    }
                    break;

                default:
                    Console.WriteLine("Неизвестная операция. Используйте +, -, *, /.");
                    break;
            }

            Console.WriteLine(); // пустая строка для удобства
        }

        Console.WriteLine("Программа завершена.");
    }
}
