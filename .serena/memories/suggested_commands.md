# Suggested Commands for Thinktecture.Runtime.Extensions

## Essential Development Commands

### Building
```powershell
# Restore packages
dotnet restore

# Build entire solution
dotnet build

# Build specific project
dotnet build src/Thinktecture.Runtime.Extensions

# Build with specific configuration
dotnet build --configuration Release
```

### Testing
```powershell
# Run all tests
dotnet test

# Run tests for specific project
dotnet test test/Thinktecture.Runtime.Extensions.Json.Tests

# Run tests with detailed output
dotnet test --verbosity normal

# Run tests and generate coverage (if configured)
dotnet test --collect:"XPlat Code Coverage"
```

### Code Quality
```powershell
# Format code according to .editorconfig
dotnet format

# Format only specific project
dotnet format src/Thinktecture.Runtime.Extensions

# Analyze code (built-in analyzers)
dotnet build --verbosity normal  # Will show analyzer warnings
```

### Package Management
```powershell
# Add package reference (update Directory.Packages.props for version)
dotnet add package PackageName

# Update packages
dotnet restore --force-evaluate
```

### Common Development Workflow
```powershell
# After making changes
dotnet build
dotnet test
dotnet format

# Before committing
dotnet build --configuration Release
dotnet test --verbosity normal
```

## Windows-Specific Utilities
```powershell
# Directory navigation
Get-ChildItem -Path . -Recurse -Name "*.cs" | Select-String "pattern"

# Find files
Get-ChildItem -Path . -Recurse -Filter "*.csproj"

# Git operations
git status
git add .
git commit -m "message"
git push

# Process management
Get-Process dotnet
Stop-Process -Name dotnet
```

## Solution Management
```powershell
# List projects in solution
dotnet sln list

# Add new project to solution
dotnet sln add path/to/new.csproj

# Remove project from solution  
dotnet sln remove path/to/project.csproj
```