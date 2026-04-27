public class Product : IEntity
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }

    public override string ToString()
    {
        return $"Product(Id={Id}, Name={Name}, Price={Price})";
    }
}

public class User : IEntity
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;

    public override string ToString()
    {
        return $"User(Id={Id}, Name={Name}, Email={Email})";
    }
}
