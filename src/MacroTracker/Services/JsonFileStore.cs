using System.Text.Json;
using System.Text.Json.Serialization;

namespace MacroTracker.Services;

public class JsonFileStore<T>
{
    private readonly string _folderPath;
    private readonly string _fileName;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public JsonFileStore(string fileName)
    {
        _fileName = fileName;
        _folderPath = Path.Combine(AppContext.BaseDirectory, "App_Data");
    }

    public string GetFilePath() => Path.Combine(_folderPath, _fileName);

    private void EnsureFolderExists()
    {
        if (!Directory.Exists(_folderPath))
        {
            Directory.CreateDirectory(_folderPath);
        }
    }

    public async Task<List<T>> LoadAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            EnsureFolderExists();
            var filePath = GetFilePath();

            if (!File.Exists(filePath))
            {
                return new List<T>();
            }

            var json = await File.ReadAllTextAsync(filePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<T>();
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            };

            return JsonSerializer.Deserialize<List<T>>(json, options) ?? new List<T>();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task SaveAsync(List<T> items)
    {
        await _semaphore.WaitAsync();
        try
        {
            EnsureFolderExists();
            var filePath = GetFilePath();

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            };

            var json = JsonSerializer.Serialize(items, options);
            await File.WriteAllTextAsync(filePath, json);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
