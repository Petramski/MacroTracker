using System.Globalization;
using System.IO.Compression;
using MacroTracker.Components;
using MacroTracker.Services;

var builder = WebApplication.CreateBuilder(args);
var dutchCulture = new CultureInfo("nl-NL");

CultureInfo.DefaultThreadCurrentCulture = dutchCulture;
CultureInfo.DefaultThreadCurrentUICulture = dutchCulture;

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<FoodService>();
builder.Services.AddScoped<RecipeService>();
builder.Services.AddScoped<DailyLogService>();
builder.Services.AddScoped<MeasurementService>();
builder.Services.AddScoped<PersonalDataService>();
builder.Services.AddScoped<MacroCalculator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapGet("/api/data-files/export", () =>
{
    var appDataPath = Path.Combine(AppContext.BaseDirectory, "App_Data");
    Directory.CreateDirectory(appDataPath);

    using var memoryStream = new MemoryStream();
    using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true))
    {
        foreach (var filePath in Directory.GetFiles(appDataPath, "*.json", SearchOption.TopDirectoryOnly))
        {
            var entry = archive.CreateEntry(Path.GetFileName(filePath), CompressionLevel.Fastest);
            using var entryStream = entry.Open();
            using var fileStream = File.OpenRead(filePath);
            fileStream.CopyTo(entryStream);
        }
    }

    memoryStream.Position = 0;
    var fileName = $"macrotracker-data-{DateTime.Now:yyyyMMdd-HHmmss}.zip";
    return Results.File(memoryStream.ToArray(), "application/zip", fileName);
});

app.MapPost("/api/data-files/import", async (IFormFile? zipFile) =>
{
    if (zipFile is null || zipFile.Length == 0)
    {
        return Results.BadRequest("Upload een geldig zip-bestand.");
    }

    var appDataPath = Path.Combine(AppContext.BaseDirectory, "App_Data");
    Directory.CreateDirectory(appDataPath);

    using var uploadedStream = zipFile.OpenReadStream();
    using var archive = new ZipArchive(uploadedStream, ZipArchiveMode.Read);

    var importedCount = 0;
    foreach (var entry in archive.Entries)
    {
        if (string.IsNullOrWhiteSpace(entry.Name) || !entry.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            continue;
        }

        var safeFileName = Path.GetFileName(entry.Name);
        if (string.IsNullOrWhiteSpace(safeFileName))
        {
            continue;
        }

        var destinationPath = Path.Combine(appDataPath, safeFileName);
        await using var entryStream = entry.Open();
        await using var destinationStream = File.Create(destinationPath);
        await entryStream.CopyToAsync(destinationStream);
        importedCount++;
    }

    if (importedCount == 0)
    {
        return Results.BadRequest("Het zip-bestand bevat geen JSON-bestanden.");
    }

    return Results.Redirect("/data-files");
}).DisableAntiforgery();

app.Run();
