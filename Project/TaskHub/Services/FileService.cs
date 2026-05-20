using System.Text.Json;
using System.Text.Json.Serialization;
using TaskHub.Models;

namespace TaskHub.Services;

// IDisposable requirement: FileService owns disposable stream resources during async file work.
public class FileService : IDisposable
{
    private bool _disposed;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    // async/await requirement: data is saved asynchronously.
    public async Task SaveTasksAsync(string filePath, IEnumerable<TaskItem> tasks)
    {
        ThrowIfDisposed();

        await using FileStream stream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(stream, tasks, _jsonOptions);
    }

    // async/await requirement: data is loaded asynchronously.
    public async Task<List<TaskItem>> LoadTasksAsync(string filePath)
    {
        ThrowIfDisposed();

        if (!File.Exists(filePath))
        {
            return new List<TaskItem>();
        }

        await using FileStream stream = File.OpenRead(filePath);
        return await JsonSerializer.DeserializeAsync<List<TaskItem>>(stream, _jsonOptions) ?? new List<TaskItem>();
    }

    public void Dispose()
    {
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(FileService));
        }
    }
}
