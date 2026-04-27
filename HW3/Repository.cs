using System;
using System.Collections.Generic;

public class Repository<T> where T : IEntity
{
    private readonly Dictionary<int, T> _items = new();

    public int Count => _items.Count;

    public void Add(T item)
    {
        if (_items.ContainsKey(item.Id))
        {
            throw new InvalidOperationException($"Item with Id={item.Id} already exists.");
        }

        _items[item.Id] = item;
    }

    public bool Remove(int id)
    {
        return _items.Remove(id);
    }

    public T? GetById(int id)
    {
        return _items.TryGetValue(id, out T? item) ? item : default;
    }

    public IReadOnlyList<T> GetAll()
    {
        List<T> result = new();
        foreach (T item in _items.Values)
        {
            result.Add(item);
        }

        return result.AsReadOnly();
    }

    public IReadOnlyList<T> Find(Predicate<T> predicate)
    {
        List<T> result = new();
        foreach (T item in _items.Values)
        {
            if (predicate(item))
            {
                result.Add(item);
            }
        }

        return result.AsReadOnly();
    }
}
