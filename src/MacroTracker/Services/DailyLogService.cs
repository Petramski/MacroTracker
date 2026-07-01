using MacroTracker.Models;

namespace MacroTracker.Services;

public class DailyLogService
{
    private readonly JsonFileStore<DailyLogEntry> _store;

    public DailyLogService()
    {
        _store = new JsonFileStore<DailyLogEntry>("daily-log.json");
    }

    public async Task<List<DailyLogEntry>> GetAllAsync()
    {
        var entries = await _store.LoadAsync();
        return entries.OrderBy(e => e.Date).ThenBy(e => e.CreatedAt).ToList();
    }

    public async Task<List<DailyLogEntry>> GetByDateAsync(DateOnly date)
    {
        var entries = await _store.LoadAsync();
        return entries.Where(e => e.Date == date)
            .OrderBy(e => e.CreatedAt)
            .ToList();
    }

    public async Task<DailyLogEntry?> GetByIdAsync(Guid id)
    {
        var entries = await _store.LoadAsync();
        return entries.FirstOrDefault(e => e.Id == id);
    }

    public async Task AddAsync(DailyLogEntry entry)
    {
        entry.Id = Guid.NewGuid();
        entry.CreatedAt = DateTime.UtcNow;
        entry.UpdatedAt = DateTime.UtcNow;

        var entries = await _store.LoadAsync();
        entries.Add(entry);
        await _store.SaveAsync(entries);
    }

    public async Task UpdateAsync(DailyLogEntry entry)
    {
        entry.UpdatedAt = DateTime.UtcNow;
        var entries = await _store.LoadAsync();
        var index = entries.FindIndex(e => e.Id == entry.Id);
        if (index >= 0)
        {
            entries[index] = entry;
            await _store.SaveAsync(entries);
        }
    }

    public async Task DeleteByDateAsync(DateOnly date)
    {
        var entries = await _store.LoadAsync();
        entries.RemoveAll(e => e.Date == date);
        await _store.SaveAsync(entries);
    }

    public async Task DeleteAsync(Guid id)
    {
        var entries = await _store.LoadAsync();
        entries.RemoveAll(e => e.Id == id);
        await _store.SaveAsync(entries);
    }
}
