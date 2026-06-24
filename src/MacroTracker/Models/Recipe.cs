namespace MacroTracker.Models;

public class Recipe
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public decimal TotalPreparedWeightGrams { get; set; }
    public FoodUnit Unit { get; set; } = FoodUnit.Grams;
    public decimal UnitWeightGrams { get; set; } = 100;
    public List<RecipeIngredient> Ingredients { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
