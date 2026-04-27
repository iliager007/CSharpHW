using System;
using System.Collections.Generic;

internal static class Program
{
    private static void Main()
    {
        Console.WriteLine("=== Task 1: Generic Repository ===");
        RunRepositoryDemo();

        Console.WriteLine();
        Console.WriteLine("=== Task 2: Generic CollectionUtils Methods ===");
        RunCollectionUtilsDemo();
    }

    private static void RunRepositoryDemo()
    {
        Repository<Product> productRepository = new();
        productRepository.Add(new Product { Id = 1, Name = "Laptop", Price = 2500m });
        productRepository.Add(new Product { Id = 2, Name = "Mouse", Price = 80m });
        productRepository.Add(new Product { Id = 3, Name = "Phone", Price = 1200m });

        Console.WriteLine($"Products count: {productRepository.Count}");
        Console.WriteLine($"GetById(2): {productRepository.GetById(2)}");

        IReadOnlyList<Product> expensiveProducts = productRepository.Find(p => p.Price > 1000m);
        Console.WriteLine("Products with price > 1000:");
        PrintCollection(expensiveProducts);

        try
        {
            productRepository.Add(new Product { Id = 1, Name = "Duplicate Laptop", Price = 2600m });
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Duplicate add handled: {ex.Message}");
        }

        Repository<User> userRepository = new();
        userRepository.Add(new User { Id = 1, Name = "Alice", Email = "alice@example.com" });
        userRepository.Add(new User { Id = 2, Name = "Bob", Email = "bob@example.com" });

        Console.WriteLine();
        Console.WriteLine($"Users count: {userRepository.Count}");
        Console.WriteLine($"GetById(1): {userRepository.GetById(1)}");

        bool removed = userRepository.Remove(2);
        Console.WriteLine($"Remove user with Id=2: {removed}");
        Console.WriteLine("All users:");
        PrintCollection(userRepository.GetAll());
    }

    private static void RunCollectionUtilsDemo()
    {
        List<int> intValues = new() { 1, 2, 2, 3, 1, 4, 4, 5 };
        List<string> stringValues = new() { "apple", "banana", "apple", "orange", "banana", "kiwi" };

        Console.WriteLine("Distinct ints:");
        PrintCollection(CollectionUtils.Distinct(intValues));

        Console.WriteLine("Distinct strings:");
        PrintCollection(CollectionUtils.Distinct(stringValues));

        List<string> words = new() { "cat", "car", "house", "tree", "sun", "book" };
        Dictionary<int, List<string>> groupedByLength = CollectionUtils.GroupBy(words, word => word.Length);
        Console.WriteLine("Group words by length:");
        foreach (KeyValuePair<int, List<string>> group in groupedByLength)
        {
            Console.WriteLine($"Length {group.Key}: [{string.Join(", ", group.Value)}]");
        }

        Dictionary<string, int> firstCounter = new()
        {
            ["apple"] = 3,
            ["banana"] = 2,
            ["orange"] = 1
        };

        Dictionary<string, int> secondCounter = new()
        {
            ["banana"] = 4,
            ["orange"] = 2,
            ["kiwi"] = 5
        };

        Dictionary<string, int> merged = CollectionUtils.Merge(
            firstCounter,
            secondCounter,
            (left, right) => left + right);

        Console.WriteLine("Merged counters:");
        foreach (KeyValuePair<string, int> pair in merged)
        {
            Console.WriteLine($"{pair.Key}: {pair.Value}");
        }

        List<Product> products = new()
        {
            new Product { Id = 10, Name = "Monitor", Price = 900m },
            new Product { Id = 11, Name = "Gaming PC", Price = 3200m },
            new Product { Id = 12, Name = "Keyboard", Price = 150m }
        };

        Product mostExpensive = CollectionUtils.MaxBy(products, product => product.Price);
        Console.WriteLine($"Most expensive product: {mostExpensive}");
    }

    private static void PrintCollection<T>(IEnumerable<T> items)
    {
        Console.WriteLine($"[{string.Join(", ", items)}]");
    }
}
