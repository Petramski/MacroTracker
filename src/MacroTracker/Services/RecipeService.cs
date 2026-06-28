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

    public async Task<bool> WouldCreateCircularReferenceAsync(Recipe candidate)
    {
        if (candidate.Id == Guid.Empty)
        {
            return false;
        }

        var recipes = await _store.LoadAsync();
        var recipeById = recipes.ToDictionary(r => r.Id, r => r);
        recipeById[candidate.Id] = candidate;

        foreach (var childRecipeId in GetChildRecipeIds(candidate))
        {
            if (childRecipeId == candidate.Id)
            {
                return true;
            }

            if (HasPathToTarget(childRecipeId, candidate.Id, new HashSet<Guid> { candidate.Id }, recipeById))
            {
                return true;
            }
        }

        return false;
    }

    private static IEnumerable<Guid> GetChildRecipeIds(Recipe recipe)
    {
        return recipe.Ingredients
            .Where(i => i.ReferenceType == FoodReferenceType.Recipe && i.RecipeId.HasValue)
            .Select(i => i.RecipeId!.Value);
    }

    private static bool HasPathToTarget(Guid currentId, Guid targetId, HashSet<Guid> visited, Dictionary<Guid, Recipe> recipeById)
    {
        if (!visited.Add(currentId))
        {
            return false;
        }

        if (!recipeById.TryGetValue(currentId, out var currentRecipe))
        {
            return false;
        }

        foreach (var nextId in GetChildRecipeIds(currentRecipe))
        {
            if (nextId == targetId)
            {
                return true;
            }

            if (HasPathToTarget(nextId, targetId, visited, recipeById))
            {
                return true;
            }
        }

        return false;
    }
}
