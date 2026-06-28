namespace MacroTracker.Models;

public class RecipeIngredient
{
    public FoodReferenceType ReferenceType { get; set; } = FoodReferenceType.Food;
    public Guid FoodItemId { get; set; } = Guid.Empty;
    public Guid? RecipeId { get; set; }
    public decimal AmountGrams { get; set; }
}
