namespace MacroTracker.Models;

public class PersonalData
{
    public Guid Id { get; set; }
    public decimal HeightCm { get; set; }
    public DateOnly? BirthDate { get; set; }
    public decimal GoalWeightKg { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
