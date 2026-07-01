namespace MacroTracker.Models;

public class DailyLogEntry
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public FoodReferenceType FoodReferenceType { get; set; }
    public Guid? FoodItemId { get; set; }
    public Guid? RecipeId { get; set; }
    public decimal Amount { get; set; }
    public FoodUnit Unit { get; set; } = FoodUnit.Grams;
    public string? Meal { get; set; }
    public string? Notes { get; set; }
    public decimal? LoggedCalories { get; set; }
    public decimal? LoggedCarbs { get; set; }
    public decimal? LoggedProtein { get; set; }
    public decimal? LoggedFat { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
