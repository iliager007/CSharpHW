using TaskHub.Models;

namespace TaskHub.Data;

// Generics and List<T>: this repository can manage any IIdentifiable type.
public class TaskRepository<T> where T : IIdentifiable
{
    private readonly List<T> _items = new();
    private int _nextId = 1;

    public IReadOnlyList<T> Items => _items.AsReadOnly();

    public void Add(T item)
    {
        item.Id = _nextId++;
        _items.Add(item);
    }

    public bool RemoveById(int id)
    {
        T? item = GetById(id);
        if (item is null)
        {
            return false;
        }

        _items.Remove(item);
        return true;
    }

    public T? GetById(int id)
    {
        return _items.FirstOrDefault(item => item.Id == id);
    }

    public void ReplaceAll(IEnumerable<T> items)
    {
        _items.Clear();
        _items.AddRange(items);
        _nextId = _items.Count == 0 ? 1 : _items.Max(item => item.Id) + 1;
    }
}
