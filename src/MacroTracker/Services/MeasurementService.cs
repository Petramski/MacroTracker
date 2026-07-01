using MacroTracker.Models;

namespace MacroTracker.Services;

public class MeasurementService
{
    private readonly JsonFileStore<DailyMeasurement> _store;

    public MeasurementService()
    {
        _store = new JsonFileStore<DailyMeasurement>("measurements.json");
    }

    public async Task<List<DailyMeasurement>> GetAllAsync()
    {
        var measurements = await _store.LoadAsync();
        return measurements.OrderBy(m => m.Date).ThenBy(m => m.CreatedAt).ToList();
    }

    public async Task<DailyMeasurement?> GetByDateAsync(DateOnly date)
    {
        var measurements = await _store.LoadAsync();
        return measurements.FirstOrDefault(m => m.Date == date);
    }

    public async Task UpsertByDateAsync(DailyMeasurement measurement)
    {
        var measurements = await _store.LoadAsync();
        var existing = measurements.FirstOrDefault(m => m.Date == measurement.Date);

        if (existing == null)
        {
            measurement.Id = Guid.NewGuid();
            measurement.CreatedAt = DateTime.UtcNow;
            measurement.UpdatedAt = DateTime.UtcNow;
            measurements.Add(measurement);
        }
        else
        {
            existing.WeightKg = measurement.WeightKg;
            existing.WaistCm = measurement.WaistCm;
            existing.GlucoseMmolL = measurement.GlucoseMmolL;
            existing.KetonesMmolL = measurement.KetonesMmolL;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        await _store.SaveAsync(measurements);
    }

    public async Task DeleteByDateAsync(DateOnly date)
    {
        var measurements = await _store.LoadAsync();
        measurements.RemoveAll(m => m.Date == date);
        await _store.SaveAsync(measurements);
    }
}
