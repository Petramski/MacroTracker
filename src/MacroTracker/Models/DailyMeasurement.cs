namespace MacroTracker.Models;

public class DailyMeasurement
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public decimal WeightKg { get; set; }
    public decimal WaistCm { get; set; }
    public decimal GlucoseMmolL { get; set; }
    public decimal KetonesMmolL { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
