using MacroTracker.Models;

namespace MacroTracker.Services;

public class MacroCalculator
{
    private readonly FoodService _foodService;
    private readonly RecipeService _recipeService;

    public MacroCalculator()
    {
        _foodService = new FoodService();
        _recipeService = new RecipeService();
    }

    public async Task<MacroTotals> CalculateForEntryAsync(DailyLogEntry entry)
    {
        if (entry.FoodReferenceType == FoodReferenceType.Food && entry.FoodItemId.HasValue)
        {
            var food = await _foodService.GetByIdAsync(entry.FoodItemId.Value);
            if (food != null)
            {
                return CalculateFromFood(food, entry.Amount, entry.Unit);
            }
        }
        else if (entry.FoodReferenceType == FoodReferenceType.Recipe && entry.RecipeId.HasValue)
        {
            var recipe = await _recipeService.GetByIdAsync(entry.RecipeId.Value);
            if (recipe != null)
            {
                return await CalculateFromRecipeAsync(recipe, entry.Amount, entry.Unit);
            }
        }

        return new MacroTotals();
    }

    public MacroTotals CalculateFromFood(FoodItem food, decimal amount, FoodUnit unit = FoodUnit.Grams)
    {
        var amountGrams = ConvertToGrams(amount, unit, food.UnitWeightGrams);
        var factor = amountGrams / 100;
        return new MacroTotals
        {
            Calories = food.CaloriesPer100g * factor,
            Carbs = food.CarbsPer100g * factor,
            Protein = food.ProteinPer100g * factor,
            Fat = food.FatPer100g * factor
        };
    }

    public async Task<MacroTotals> CalculateFromRecipeAsync(Recipe recipe, decimal amount, FoodUnit unit = FoodUnit.Grams)
    {
        var amountGrams = ConvertToGrams(amount, unit, recipe.UnitWeightGrams);
        var recipeMacros = await GetRecipeMacrosPerHundredGramsAsync(recipe);
        var factor = amountGrams / 100;
        return new MacroTotals
        {
            Calories = recipeMacros.Calories * factor,
            Carbs = recipeMacros.Carbs * factor,
            Protein = recipeMacros.Protein * factor,
            Fat = recipeMacros.Fat * factor
        };
    }

    public async Task<MacroTotals> GetRecipeMacrosPerHundredGramsAsync(Recipe recipe)
    {
        var totalMacros = new MacroTotals();

        foreach (var ingredient in recipe.Ingredients)
        {
            var food = await _foodService.GetByIdAsync(ingredient.FoodItemId);
            if (food != null)
            {
                var ingredientMacros = CalculateFromFood(food, ingredient.AmountGrams, FoodUnit.Grams);
                totalMacros.Calories += ingredientMacros.Calories;
                totalMacros.Carbs += ingredientMacros.Carbs;
                totalMacros.Protein += ingredientMacros.Protein;
                totalMacros.Fat += ingredientMacros.Fat;
            }
        }

        if (recipe.TotalPreparedWeightGrams > 0)
        {
            var factor = 100 / recipe.TotalPreparedWeightGrams;
            totalMacros.Calories *= factor;
            totalMacros.Carbs *= factor;
            totalMacros.Protein *= factor;
            totalMacros.Fat *= factor;
        }

        return totalMacros;
    }

    public static decimal ConvertToGrams(decimal amount, FoodUnit unit, decimal unitWeightGrams)
    {
        return unit switch
        {
            FoodUnit.Grams => amount,
            FoodUnit.Piece => amount * unitWeightGrams,
            FoodUnit.Serving => amount * unitWeightGrams,
            FoodUnit.Cup => amount * unitWeightGrams,
            FoodUnit.Tablespoon => amount * unitWeightGrams,
            FoodUnit.Teaspoon => amount * unitWeightGrams,
            FoodUnit.Slice => amount * unitWeightGrams,
            FoodUnit.Handful => amount * unitWeightGrams,
            _ => amount
        };
    }

    public static string FormatCalories(decimal cal) => Math.Round(cal, 0).ToString("F0");
    public static string FormatMacro(decimal macro) => Math.Round(macro, 1).ToString("F1");

    public static string GetUnitDisplayName(FoodUnit unit) => unit switch
    {
        FoodUnit.Grams => "g",
        FoodUnit.Piece => "stuk",
        FoodUnit.Serving => "portie",
        FoodUnit.Cup => "kop",
        FoodUnit.Tablespoon => "el",
        FoodUnit.Teaspoon => "tl",
        FoodUnit.Slice => "plak",
        FoodUnit.Handful => "handje",
        _ => "eenheid"
    };
}
