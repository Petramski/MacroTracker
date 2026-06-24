#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Install MacroTracker to the requested path.

.DESCRIPTION
    Copies or verifies the MacroTracker project in C:\Users\bepema01\Repos\Private\MacroTracker

.EXAMPLE
    .\install-to-requested-path.ps1
#>

$TargetPath = "C:\Users\bepema01\Repos\Private\MacroTracker"

# Check if we're already in the target directory
$CurrentPath = Get-Location | Convert-Path
if ($CurrentPath -eq $TargetPath) {
    Write-Host "✓ Already in the requested location: $TargetPath" -ForegroundColor Green
    Write-Host "`nMacroTracker is ready to run. Use:" -ForegroundColor Cyan
    Write-Host "  dotnet restore" -ForegroundColor Yellow
    Write-Host "  dotnet run --project .\src\MacroTracker\MacroTracker.csproj" -ForegroundColor Yellow
    exit 0
}

# Otherwise, copy to the target location
Write-Host "Installing MacroTracker to: $TargetPath" -ForegroundColor Cyan

# Create target directory if it doesn't exist
if (-not (Test-Path $TargetPath)) {
    New-Item -ItemType Directory -Path $TargetPath -Force | Out-Null
    Write-Host "✓ Created directory: $TargetPath" -ForegroundColor Green
}

# Copy all files from current directory to target
try {
    $FilesToCopy = Get-ChildItem -Path $CurrentPath -Recurse -Exclude ".git", ".gitignore", "bin", "obj"
    $FilesToCopy | Copy-Item -Destination { 
        if ($_.PSIsContainer) {
            Join-Path $TargetPath $_.FullName.Substring($CurrentPath.Length)
        } else {
            $Dir = Split-Path -Path ($_.FullName.Substring($CurrentPath.Length)) -Parent
            if ($Dir) {
                $TargetDir = Join-Path $TargetPath $Dir
                if (-not (Test-Path $TargetDir)) {
                    New-Item -ItemType Directory -Path $TargetDir -Force | Out-Null
                }
            }
            Join-Path $TargetPath $_.FullName.Substring($CurrentPath.Length)
        }
    } -Force -ErrorAction Stop

    Write-Host "✓ Installation complete!" -ForegroundColor Green
    Write-Host "`nTo run MacroTracker:" -ForegroundColor Cyan
    Write-Host "  cd $TargetPath" -ForegroundColor Yellow
    Write-Host "  dotnet restore" -ForegroundColor Yellow
    Write-Host "  dotnet run --project .\src\MacroTracker\MacroTracker.csproj" -ForegroundColor Yellow
}
catch {
    Write-Host "✗ Installation failed: $_" -ForegroundColor Red
    exit 1
}
