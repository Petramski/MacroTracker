namespace MacroTracker.Models;

public class FoodItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public string? Notes { get; set; }
    public FoodUnit Unit { get; set; } = FoodUnit.Grams;
    public decimal UnitWeightGrams { get; set; } = 100;
    public decimal CaloriesPer100g { get; set; }
    public decimal CarbsPer100g { get; set; }
    public decimal ProteinPer100g { get; set; }
    public decimal FatPer100g { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
