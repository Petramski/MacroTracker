# MacroTracker

A local-first daily food intake tracker built with C# and Blazor Server. Track calories, carbohydrates, protein, and fat—all stored locally in JSON files.

## Features

- **Food Repository**: Maintain a text-based catalog of foods and products with macro values per 100g
- **Recipe Management**: Create recipes from known food items with ingredient tracking
- **Daily Log**: Log food and recipe intake for any date with meal categorization
- **Body Measurements**: Track daily weight (kg) and waist circumference (cm)
- **Personal Data**: Save body profile settings like height (cm) for metric calculations
- **Dashboard**: View today's summary with quick stats and entry breakdown
- **History**: Analyze trends with date range views and daily/average totals
- **No Database**: All data is stored in human-readable JSON files under `App_Data/`

## Technology Stack

- **Language**: C#
- **Framework**: Blazor Server
- **Styling**: Bootstrap
- **Serialization**: System.Text.Json
- **Persistence**: Plain JSON text files
- **No dependencies**: No SQL, SQLite, EF Core, or external database packages

## Installation & Running

### Prerequisites
- .NET 10.0 (or .NET 8.0+ with SDK 10.0 support)

### Clone or Extract

Extract this project to your desired location, or if already in `C:\Users\bepema01\Repos\Private\MacroTracker`, you're ready to proceed.

### Restore and Run

```powershell
dotnet restore
dotnet run --project .\src\MacroTracker\MacroTracker.csproj
```

The application will start on `https://localhost:5001` (or similar). Open your browser and navigate there.

## Data Files

All application data is stored in JSON files located at:

```
src/MacroTracker/App_Data/
├── foods.json       (food items and nutritional data)
├── recipes.json     (recipes and their ingredients)
├── daily-log.json   (daily food intake entries)
├── measurements.json (daily body measurements)
└── personal-data.json (single-user profile data)
```

These files are:
- **Human-readable**: Formatted JSON, suitable for manual inspection or editing
- **Version-control friendly**: Can be committed to git for backup
- **Safe**: Thread-safe operations via `SemaphoreSlim` to prevent concurrent write conflicts

## Usage

1. **Foods Page**: Add foods/products with name, brand, and macro values per 100g
2. **Recipes Page**: Create recipes by selecting foods and entering ingredient amounts
3. **Daily Log**: Log entries by selecting food/recipe, amount in grams, meal type, and notes
4. **Body Measurements**: Enter daily weight and waist circumference from the Daily Log page
5. **Personal Data**: Set your height once on the Personal Data page
6. **Today**: Quick dashboard showing today's totals, BMI, and height/waist ratio
7. **History**: Select a date range to view daily summaries, measurements, and period averages

## Building the Solution

```powershell
dotnet build .\MacroTracker.sln
```

## No Database Confirmation

- ✅ **No SQL**: No SQL Server, PostgreSQL, MySQL, or similar
- ✅ **No SQLite**: No embedded SQLite databases
- ✅ **No EF Core**: No Entity Framework Core or ORM dependencies
- ✅ **No NuGet Database Packages**: All persistence is custom JSON file handling

## Architecture

### Models
- `FoodItem`: Individual food/product with macros
- `Recipe`: Recipe with ingredient list and total prepared weight
- `DailyLogEntry`: Log entry with food/recipe reference and amount
- `DailyMeasurement`: Daily weight and waist circumference entry
- `PersonalData`: Single-user body profile (height)
- `MacroTotals`: Aggregated nutritional values

### Services
- `JsonFileStore<T>`: Generic file-based persistence layer
- `FoodService`: Food/product CRUD
- `RecipeService`: Recipe CRUD
- `DailyLogService`: Daily log entry CRUD
- `MeasurementService`: Daily body measurement upsert/query
- `PersonalDataService`: Single-user personal data read/write
- `MacroCalculator`: Macro computation from foods and recipes

### Pages
- `Home.razor`: Today's dashboard
- `Foods.razor`: Food management
- `Recipes.razor`: Recipe management
- `DailyLog.razor`: Log entries with date selection
- `History.razor`: Date range analysis
- `PersonalProfile.razor`: Single-user profile settings
- `DataFiles.razor`: File location and app info

## Macro Calculations

### For Foods
```
amount_grams / 100 * per100g_value
```

### For Recipes
1. Sum all ingredient macros
2. Divide by total prepared weight in grams
3. Multiply by 100 to get per-100g values
4. Apply the same formula to logged amounts

## Styling

The application uses Bootstrap for responsive, clean UI with:
- Cards for data display
- Tables for listings
- Forms for input
- Badges for meal types
- Responsive grid layout

## Contributions

This is a personal project. Feel free to fork or use as a template for your own macro tracking needs.

## License

Open source. Use freely.
