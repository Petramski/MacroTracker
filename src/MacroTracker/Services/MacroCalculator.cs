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
        return await GetRecipeMacrosPerHundredGramsAsync(recipe, new HashSet<Guid>());
    }

    private async Task<MacroTotals> GetRecipeMacrosPerHundredGramsAsync(Recipe recipe, HashSet<Guid> visitedRecipeIds)
    {
        if (recipe.Id != Guid.Empty && !visitedRecipeIds.Add(recipe.Id))
        {
            return new MacroTotals();
        }

        var totalMacros = new MacroTotals();

        foreach (var ingredient in recipe.Ingredients)
        {
            if (ingredient.ReferenceType == FoodReferenceType.Recipe && ingredient.RecipeId.HasValue)
            {
                var nestedRecipe = await _recipeService.GetByIdAsync(ingredient.RecipeId.Value);
                if (nestedRecipe != null)
                {
                    var nestedPerHundred = await GetRecipeMacrosPerHundredGramsAsync(nestedRecipe, visitedRecipeIds);
                    var nestedFactor = ingredient.AmountGrams / 100;
                    totalMacros.Calories += nestedPerHundred.Calories * nestedFactor;
                    totalMacros.Carbs += nestedPerHundred.Carbs * nestedFactor;
                    totalMacros.Protein += nestedPerHundred.Protein * nestedFactor;
                    totalMacros.Fat += nestedPerHundred.Fat * nestedFactor;
                }

                continue;
            }

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

        if (recipe.Id != Guid.Empty)
        {
            visitedRecipeIds.Remove(recipe.Id);
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

    public static decimal? CalculateBmi(decimal weightKg, decimal heightCm)
    {
        if (weightKg <= 0 || heightCm <= 0)
        {
            return null;
        }

        var heightMeters = heightCm / 100m;
        return Math.Round(weightKg / (heightMeters * heightMeters), 2);
    }

    public static string GetBmiTileClass(decimal? bmi) => bmi switch
    {
        null => "card bg-light",
        < 18.5m => "card bmi-tile bmi-tile-underweight text-white",
        < 25m => "card bmi-tile bmi-tile-healthy text-white",
        < 30m => "card bmi-tile bmi-tile-overweight-1 text-dark",
        < 40m => "card bmi-tile bmi-tile-overweight-2 text-white",
        _ => "card bmi-tile bmi-tile-overweight-3 text-white"
    };

    public static string GetBmiTableCellClass(decimal? bmi) => bmi switch
    {
        null => string.Empty,
        < 18.5m => "bmi-table-text-underweight",
        < 25m => "bmi-table-text-healthy",
        < 30m => "bmi-table-text-overweight-1",
        < 40m => "bmi-table-text-overweight-2",
        _ => "bmi-table-text-overweight-3"
    };

    public static decimal? CalculateWaistToHeightRatio(decimal waistCm, decimal heightCm)
    {
        if (waistCm <= 0 || heightCm <= 0)
        {
            return null;
        }

        return Math.Round(waistCm / heightCm, 2);
    }

    public static string GetWaistToHeightTileClass(decimal? ratio) => ratio switch
    {
        null => "card bg-light",
        < 0.4m => "card wthr-tile wthr-tile-low text-white",
        < 0.5m => "card wthr-tile wthr-tile-healthy text-white",
        < 0.6m => "card wthr-tile wthr-tile-elevated text-dark",
        _ => "card wthr-tile wthr-tile-high text-white"
    };

    public static string GetWaistToHeightTableCellClass(decimal? ratio) => ratio switch
    {
        null => string.Empty,
        < 0.4m => "wthr-table-text-low",
        < 0.5m => "wthr-table-text-healthy",
        < 0.6m => "wthr-table-text-elevated",
        _ => "wthr-table-text-high"
    };

    public static string GetGlucoseTileClass(decimal glucoseMmolL)
    {
        if (glucoseMmolL <= 0)
        {
            return "card bg-light";
        }

        return glucoseMmolL switch
        {
            < 4.0m => "card glucose-tile glucose-tile-hypo text-white",
            <= 7.0m => "card glucose-tile glucose-tile-normal text-white",
            _ => "card glucose-tile glucose-tile-hyper text-dark"
        };
    }

    public static string GetGkiTileClass(decimal? gki) => gki switch
    {
        null => "card bg-light",
        > 9.0m => "card bg-light",
        >= 6.0m => "card gki-tile gki-tile-light-ketosis text-dark",
        >= 3.0m => "card gki-tile gki-tile-moderate-ketosis text-white",
        _ => "card gki-tile gki-tile-deep-ketosis text-white"
    };

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
