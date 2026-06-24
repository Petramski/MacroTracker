using MacroTracker.Models;

namespace MacroTracker.Services;

public class PersonalDataService
{
    private readonly JsonFileStore<PersonalData> _store;

    public PersonalDataService()
    {
        _store = new JsonFileStore<PersonalData>("personal-data.json");
    }

    public async Task<PersonalData> GetAsync()
    {
        var items = await _store.LoadAsync();
        return items.FirstOrDefault() ?? new PersonalData();
    }

    public async Task SaveAsync(PersonalData personalData)
    {
        var items = await _store.LoadAsync();
        var existing = items.FirstOrDefault();

        if (existing == null)
        {
            personalData.Id = Guid.NewGuid();
            personalData.CreatedAt = DateTime.UtcNow;
            personalData.UpdatedAt = DateTime.UtcNow;
            items = new List<PersonalData> { personalData };
        }
        else
        {
            existing.HeightCm = personalData.HeightCm;
            existing.UpdatedAt = DateTime.UtcNow;
            items = new List<PersonalData> { existing };
        }

        await _store.SaveAsync(items);
    }
}
