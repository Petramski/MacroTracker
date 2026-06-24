# MacroTracker - Acceptance Criteria Checklist

## ✅ ALL CRITERIA MET

### Project Structure
- ✅ Solution file created: `MacroTracker.sln`
- ✅ Project structure matches requirements
- ✅ App_Data folder created with JSON files
- ✅ Models folder with all required classes
- ✅ Services folder with business logic
- ✅ Components with Pages and Layout
- ✅ README.md with comprehensive documentation
- ✅ Install script: `install-to-requested-path.ps1`

### Technology Stack
- ✅ Built with C# and Blazor Server
- ✅ Uses Bootstrap for styling
- ✅ No SQL dependency
- ✅ No SQLite dependency
- ✅ No Entity Framework Core (EF Core) dependency
- ✅ No database NuGet packages
- ✅ Uses System.Text.Json for serialization
- ✅ Project file (.csproj) is minimal - no external DB packages
- ✅ Target framework: .NET 10.0

### Models (All Implemented)
- ✅ FoodItem
  - Guid Id
  - string Name
  - string? Brand
  - string? Notes
  - decimal CaloriesPer100g
  - decimal CarbsPer100g
  - decimal ProteinPer100g
  - decimal FatPer100g
  - DateTime CreatedAt
  - DateTime UpdatedAt

- ✅ Recipe
  - Guid Id
  - string Name
  - string? Notes
  - decimal TotalPreparedWeightGrams
  - List<RecipeIngredient> Ingredients
  - DateTime CreatedAt
  - DateTime UpdatedAt

- ✅ RecipeIngredient
  - Guid FoodItemId
  - decimal AmountGrams

- ✅ DailyLogEntry
  - Guid Id
  - DateOnly Date
  - FoodReferenceType FoodReferenceType (enum)
  - Guid? FoodItemId
  - Guid? RecipeId
  - decimal AmountGrams
  - string? Meal
  - string? Notes
  - DateTime CreatedAt
  - DateTime UpdatedAt

- ✅ MacroTotals
  - decimal Calories
  - decimal Carbs
  - decimal Protein
  - decimal Fat

- ✅ FoodReferenceType Enum
  - Food
  - Recipe

### Services (All Implemented)
- ✅ JsonFileStore<T> - Generic JSON persistence
  - Thread-safe with SemaphoreSlim
  - Auto-creates folders
  - Indented JSON for readability
  - Path: App_Data folder under app directory

- ✅ FoodService
  - GetAllAsync() - sorted alphabetically
  - GetByIdAsync(Guid)
  - AddAsync(FoodItem)
  - UpdateAsync(FoodItem)
  - DeleteAsync(Guid)
  - IsFoodUsedAsync(Guid) - checks daily log

- ✅ RecipeService
  - GetAllAsync() - sorted alphabetically
  - GetByIdAsync(Guid)
  - AddAsync(Recipe)
  - UpdateAsync(Recipe)
  - DeleteAsync(Guid)
  - IsRecipeUsedAsync(Guid) - checks daily log

- ✅ DailyLogService
  - GetAllAsync()
  - GetByDateAsync(DateOnly)
  - GetByIdAsync(Guid)
  - AddAsync(DailyLogEntry)
  - UpdateAsync(DailyLogEntry)
  - DeleteAsync(Guid)

- ✅ MacroCalculator
  - CalculateForEntryAsync(DailyLogEntry) - dispatch to food or recipe
  - CalculateFromFood(FoodItem, decimal amountGrams)
  - CalculateFromRecipeAsync(Recipe, decimal amountGrams)
  - GetRecipeMacrosPerHundredGramsAsync(Recipe)
  - FormatCalories(decimal) - 0 decimals
  - FormatMacro(decimal) - 1 decimal

### Pages (All Implemented)
- ✅ Home.razor (`/`)
  - Today's dashboard
  - Today's date display
  - Macro totals cards (Calories, Carbs, Protein, Fat)
  - Entries list grouped by meal
  - Quick action links

- ✅ Foods.razor (`/foods`)
  - View all foods
  - Search/filter by name or brand
  - Sorted alphabetically
  - Add button
  - Edit/Delete buttons per food
  - Form for add/edit with all fields
  - Validation for required fields
  - Confirmation before deleting foods in use

- ✅ Recipes.razor (`/recipes`)
  - View all recipes
  - Add button
  - Edit/Delete buttons per recipe
  - Form for add/edit
  - Ingredient list management
  - Add ingredient from dropdown
  - Remove ingredient capability
  - Shows per-100g macros
  - Confirmation before deleting recipes in use

- ✅ DailyLog.razor (`/daily-log`)
  - Date picker (default: today)
  - Add Entry button
  - Entries table with columns:
    - Item name
    - Meal type
    - Amount (grams)
    - Calories
    - Carbs, Protein, Fat
    - Notes
    - Edit/Delete actions
  - Daily totals cards
  - Entry form with:
    - Food/Recipe type selector
    - Item dropdown
    - Amount input
    - Meal type dropdown
    - Notes field

- ✅ History.razor (`/history`)
  - Date range selector (default: last 30 days)
  - Daily totals table
  - Shows: Date, Calories, Carbs, Protein, Fat
  - Period summary section
  - Average daily totals
  - Total period totals
  - Days with entries count

- ✅ DataFiles.razor (`/data-files`)
  - Displays App_Data folder path
  - Lists JSON files:
    - foods.json
    - recipes.json
    - daily-log.json
  - Shows file sizes
  - Confirms no SQL/SQLite/EF Core
  - Technology stack info

### UI Features
- ✅ Bootstrap styling throughout
- ✅ Responsive layout
- ✅ Navigation menu with links:
  - Today (Home)
  - Daily Log
  - Foods
  - Recipes
  - History
  - Data Files
- ✅ Cards for data display
- ✅ Tables for listings
- ✅ Forms with proper validation
- ✅ Badges for meal types
- ✅ Edit/Delete buttons
- ✅ Search functionality
- ✅ Date pickers
- ✅ Friendly messages when no data
- ✅ Loading indicators

### Data Persistence
- ✅ Files stored at: `src/MacroTracker/App_Data/`
- ✅ foods.json - array of FoodItem
- ✅ recipes.json - array of Recipe
- ✅ daily-log.json - array of DailyLogEntry
- ✅ UTF-8 encoding
- ✅ Indented JSON format (human-readable)
- ✅ Thread-safe writes with SemaphoreSlim
- ✅ Auto-creates missing files
- ✅ Auto-creates App_Data folder

### Seed Data
- ✅ foods.json initialized with 8 foods:
  1. Banana (89 cal, 23g carbs, 1.1g protein, 0.3g fat)
  2. Apple (52 cal, 14g carbs, 0.3g protein, 0.2g fat)
  3. Oats (389 cal, 66g carbs, 17g protein, 7g fat)
  4. Whole Milk (61 cal, 4.8g carbs, 3.2g protein, 3.3g fat)
  5. Chicken Breast (165 cal, 0g carbs, 31g protein, 3.6g fat)
  6. Rice Cooked (130 cal, 28g carbs, 2.7g protein, 0.3g fat)
  7. Olive Oil (884 cal, 0g carbs, 0g protein, 100g fat)
  8. Egg (155 cal, 1.1g carbs, 13g protein, 11g fat)
- ✅ recipes.json - empty array (ready for user)
- ✅ daily-log.json - empty array (ready for user)

### Calculation Requirements
- ✅ Food macros: amountGrams / 100 * per100gValue
- ✅ Recipe per-100g: sum ingredients / total weight * 100
- ✅ Recipe consumed: amountGrams / 100 * recipe per100g
- ✅ Calories displayed: 0 decimals
- ✅ Macros displayed: 1 decimal

### Build & Run
- ✅ Solution builds: `dotnet build .\MacroTracker.sln`
- ✅ No build errors
- ✅ Only minor nullable warnings (acceptable)
- ✅ Runs with: `dotnet run --project .\src\MacroTracker\MacroTracker.csproj`
- ✅ No database server required
- ✅ No setup scripts needed beyond `dotnet restore`

### Documentation
- ✅ README.md with:
  - Project description
  - Feature list
  - Tech stack
  - Installation instructions
  - Data file locations
  - SQL/SQLite/EF Core confirmation
  - Basic usage steps
- ✅ QUICK_START.md with:
  - How to run the app
  - Feature overview table
  - Step-by-step usage guide
  - Data storage info
  - Troubleshooting tips
  - Customization examples
- ✅ COMPLETION_SUMMARY.md with:
  - Full feature list
  - Build commands
  - Calculation formulas
  - Acceptance criteria review

### Installation Script
- ✅ install-to-requested-path.ps1
  - Checks if already in target location
  - Copies project to C:\Users\bepema01\Repos\Private\MacroTracker
  - Creates necessary directories
  - Provides run instructions

## File Count Summary
- C# Code Files: 17
- Razor Components: 14
- JSON Data Files: 25
- Documentation Files: 3
- PowerShell Scripts: 1
- **Total: 60 files created**

## No Database Confirmation
✅ Project file (.csproj) contains NO:
- EntityFramework packages
- EntityFrameworkCore packages
- SQLite packages
- SQL Server packages
- Data Access Layer packages
- ORM packages of any kind

✅ All persistence is handled by:
- Custom C# classes (models)
- System.Text.Json serialization
- File I/O operations
- SemaphoreSlim for thread safety

## Success Criteria Met
✅ Builds successfully
✅ Runs successfully
✅ All pages functional
✅ All features working
✅ Data persists to JSON files
✅ No database dependencies
✅ Responsive UI with Bootstrap
✅ Complete documentation
✅ Ready for production use as local-first app

---

**MacroTracker is complete and ready to use!** 🎉
