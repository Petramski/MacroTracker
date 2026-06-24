using MacroTracker.Models;

namespace MacroTracker.Services;

public class FoodService
{
    private readonly JsonFileStore<FoodItem> _store;

    public FoodService()
    {
        _store = new JsonFileStore<FoodItem>("foods.json");
    }

    public async Task<List<FoodItem>> GetAllAsync()
    {
        var foods = await _store.LoadAsync();
        return foods.OrderBy(f => f.Name).ToList();
    }

    public async Task<FoodItem?> GetByIdAsync(Guid id)
    {
        var foods = await _store.LoadAsync();
        return foods.FirstOrDefault(f => f.Id == id);
    }

    public async Task AddAsync(FoodItem food)
    {
        food.Id = Guid.NewGuid();
        food.CreatedAt = DateTime.UtcNow;
        food.UpdatedAt = DateTime.UtcNow;

        var foods = await _store.LoadAsync();
        foods.Add(food);
        await _store.SaveAsync(foods);
    }

    public async Task UpdateAsync(FoodItem food)
    {
        food.UpdatedAt = DateTime.UtcNow;
        var foods = await _store.LoadAsync();
        var index = foods.FindIndex(f => f.Id == food.Id);
        if (index >= 0)
        {
            foods[index] = food;
            await _store.SaveAsync(foods);
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var foods = await _store.LoadAsync();
        foods.RemoveAll(f => f.Id == id);
        await _store.SaveAsync(foods);
    }

    public async Task<bool> IsFoodUsedAsync(Guid foodId)
    {
        var dailyLog = await new DailyLogService().GetAllAsync();
        return dailyLog.Any(e => e.FoodItemId == foodId);
    }
}
