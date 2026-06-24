using MacroTracker.Models;

namespace MacroTracker.Services;

public class RecipeService
{
    private readonly JsonFileStore<Recipe> _store;

    public RecipeService()
    {
        _store = new JsonFileStore<Recipe>("recipes.json");
    }

    public async Task<List<Recipe>> GetAllAsync()
    {
        var recipes = await _store.LoadAsync();
        return recipes.OrderBy(r => r.Name).ToList();
    }

    public async Task<Recipe?> GetByIdAsync(Guid id)
    {
        var recipes = await _store.LoadAsync();
        return recipes.FirstOrDefault(r => r.Id == id);
    }

    public async Task AddAsync(Recipe recipe)
    {
        recipe.Id = Guid.NewGuid();
        recipe.CreatedAt = DateTime.UtcNow;
        recipe.UpdatedAt = DateTime.UtcNow;

        var recipes = await _store.LoadAsync();
        recipes.Add(recipe);
        await _store.SaveAsync(recipes);
    }

    public async Task UpdateAsync(Recipe recipe)
    {
        recipe.UpdatedAt = DateTime.UtcNow;
        var recipes = await _store.LoadAsync();
        var index = recipes.FindIndex(r => r.Id == recipe.Id);
        if (index >= 0)
        {
            recipes[index] = recipe;
            await _store.SaveAsync(recipes);
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var recipes = await _store.LoadAsync();
        recipes.RemoveAll(r => r.Id == id);
        await _store.SaveAsync(recipes);
    }

    public async Task<bool> IsRecipeUsedAsync(Guid recipeId)
    {
        var dailyLog = await new DailyLogService().GetAllAsync();
        return dailyLog.Any(e => e.RecipeId == recipeId);
    }
}
