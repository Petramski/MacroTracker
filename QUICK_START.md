# MacroTracker Quick Start Guide

## Running the Application

### 1. Open PowerShell
```powershell
cd C:\Users\bepema01\Repos\Private\MacroTracker
```

### 2. Build & Run
```powershell
dotnet restore
dotnet run --project .\src\MacroTracker\MacroTracker.csproj
```

### 3. Open in Browser
- Navigate to: **https://localhost:5001**
- (Check console for exact port if 5001 is in use)

## Main Features Overview

| Page | URL | Purpose |
|------|-----|---------|
| Today | `/` | Dashboard with today's totals and entries |
| Daily Log | `/daily-log` | Add/edit/delete food and recipe entries |
| Foods | `/foods` | Manage food database (search, add, edit, delete) |
| Recipes | `/recipes` | Build recipes from foods |
| History | `/history` | Analyze trends over date ranges |
| Data Files | `/data-files` | View storage location and file info |

## Using the App

### Add a Food
1. Go to **Foods** page
2. Click **Add Food**
3. Enter name, optional brand/notes
4. Enter macro values per 100g
5. Click **Save**

### Create a Recipe
1. Go to **Recipes** page
2. Click **Add Recipe**
3. Enter recipe name and total prepared weight in grams
4. Add ingredients by selecting foods and amounts
5. Click **Save**

### Log Your Intake
1. Go to **Daily Log** page
2. Select date (defaults to today)
3. Click **Add Entry**
4. Choose food or recipe
5. Enter amount in grams
6. Optional: add meal type and notes
7. Click **Save**
8. View daily totals at top of page

### Check Your History
1. Go to **History** page
2. Select date range (default: last 30 days)
3. View daily totals table
4. See averages and period totals below

## Data Storage

All data stored in plain JSON files at:
```
C:\Users\bepema01\Repos\Private\MacroTracker\src\MacroTracker\App_Data\
```

Files:
- **foods.json** - Your food database
- **recipes.json** - Your recipes
- **daily-log.json** - Your logged entries

You can:
- Edit files directly in any text editor
- Back them up to cloud storage or git
- Share them with others
- No database server needed!

## Macro Calculations

The app automatically calculates:

**For 100g of food:**
- Stored directly in foods.json

**For recipes:**
- Sum all ingredient macros
- Divide by total prepared weight
- Results shown as per-100g

**For logged amounts:**
- amount_grams ÷ 100 × per100g_value

**Display:**
- Calories: no decimals (e.g., 245)
- Macros (carbs/protein/fat): 1 decimal (e.g., 12.5g)

## Keyboard & Navigation

- **Navigation Menu** on left sidebar
- **Mobile Responsive** - works on phones too
- **Search** on Foods page filters by name/brand
- **Date Picker** on Daily Log and History pages

## Troubleshooting

### Port Already in Use
If you get "address already in use" error:
```powershell
# Kill existing dotnet processes
Get-Process dotnet -ErrorAction SilentlyContinue | 
  ForEach-Object { Stop-Process -Id $_.Id -Force }

# Wait a moment, then try again
Start-Sleep -Seconds 3
dotnet run --project .\src\MacroTracker\MacroTracker.csproj
```

### Build Errors
```powershell
# Clean and rebuild
dotnet clean
dotnet build
```

### Missing Seed Data
If foods.json is empty, copy seed data from initial git commit or rebuild from App_Data backup.

## Customization Tips

### Add More Seed Foods
Edit `src/MacroTracker/App_Data/foods.json` directly and add new items with this structure:
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440099",
  "name": "Your Food Name",
  "brand": "Optional Brand",
  "notes": "Optional notes",
  "caloriesPer100g": 100,
  "carbsPer100g": 10,
  "proteinPer100g": 20,
  "fatPer100g": 5,
  "createdAt": "2026-06-22T00:00:00Z",
  "updatedAt": "2026-06-22T00:00:00Z"
}
```

### Change UI Styling
Edit `src/MacroTracker/wwwroot/app.css` for custom CSS.

### Add More Meal Types
Edit the `<select>` in `DailyLog.razor` (line ~136):
```html
<option value="Breakfast">Breakfast</option>
<option value="Lunch">Lunch</option>
<option value="Dinner">Dinner</option>
<option value="Snack">Snack</option>
<!-- Add your own here -->
<option value="Pre-Workout">Pre-Workout</option>
```

## Project Structure

```
src/MacroTracker/
├── Models/           - Data classes
├── Services/         - Business logic & persistence
├── Components/Pages/ - UI pages (Razor)
├── Components/Layout/- Navigation & layout
├── App_Data/         - JSON data files
├── wwwroot/          - CSS, images, static files
└── Program.cs        - Startup configuration
```

## No Database Dependency

This app intentionally has:
- ❌ No SQL Server
- ❌ No SQLite
- ❌ No Entity Framework Core
- ❌ No database package dependencies
- ✅ Plain C# classes
- ✅ System.Text.Json serialization
- ✅ File-based storage only

Perfect for:
- Learning Blazor
- Privacy-first usage (all data stays local)
- Offline work
- Easy backup & portability

## Performance Notes

- Fast: JSON files load into memory (files are small for personal use)
- Thread-safe: SemaphoreSlim prevents concurrent writes
- Responsive: Blazor Server handles UI updates efficiently
- Scalable for single user: handles years of daily logs without slowdown

## Development Tips

### Enable Debug Mode
Add to `appsettings.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Warning"
    }
  }
}
```

### Hot Reload During Development
The app supports Blazor hot reload for component changes.

### Browser DevTools
Press F12 to access:
- Network monitoring
- JavaScript console
- Component inspection

## Support & Learning

- **Blazor Docs**: https://learn.microsoft.com/en-us/aspnet/core/blazor
- **System.Text.Json**: https://learn.microsoft.com/en-us/dotnet/api/system.text.json
- **Bootstrap**: https://getbootstrap.com/docs

Enjoy tracking your macros! 🎯📊
