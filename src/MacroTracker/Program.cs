using System.Globalization;
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

app.Run();
