# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Common Commands

### Build and Test
- **Build solution**: `dotnet build --configuration Release`
- **Restore packages**: `dotnet restore`
- **Run all tests**: `dotnet test --configuration Release`
- **Pack NuGet packages**: `dotnet pack --configuration Release --output out`
- **Format code**: `dotnet format`

### Development Requirements
- .NET 9.0 SDK (as specified in global.json)
- C# 11+ for generated code
- Multiple .NET versions (7.0, 8.0, 9.0) for framework compatibility testing

## Architecture Overview

This is a .NET library providing **Smart Enums**, **Value Objects**, and **Discriminated Unions** through Roslyn Source Generators. The core architecture consists of:

### Core Components
- **`src/Thinktecture.Runtime.Extensions`**: Core library with base interfaces and attributes
- **`src/Thinktecture.Runtime.Extensions.SourceGenerator`**: Roslyn Source Generator that creates boilerplate code for partial classes/structs
- **Framework Integration Projects**: Separate projects for JSON, MessagePack, Entity Framework Core (7/8/9), ASP.NET Core, and Swashbuckle

### Key Concepts
1. **Smart Enums** (`[SmartEnum<T>]` or `[SmartEnum]`): Type-safe enums with rich behavior, properties, and methods
   - Can be keyed (with underlying type) or keyless
   - Support custom behavior via inheritance or `[UseDelegateFromConstructor]`
   - Provide exhaustive pattern matching with `Switch`/`Map` methods
2. **Value Objects** (`[ValueObject<T>]` or `[ComplexValueObject]`): Immutable types with validation and proper equality
   - Simple (keyed) vs. Complex (multiple properties) value objects
   - Use `[KeyMemberEqualityComparer]` for custom equality (especially strings)
   - Use `[ObjectFactory<T>]` for custom serialization/parsing behavior
3. **Discriminated Unions** (`[Union<T1,T2>]` for ad-hoc or `[Union]` for inheritance-based): Type-safe alternative types
   - Ad-hoc unions: simple combination of types (up to 5)
   - Regular unions: inheritance-based for complex domain hierarchies

### Source Generation Pattern
Most types in this library are `partial` classes/structs. The Source Generator automatically creates:
- Constructors and factory methods (`Create`, `TryCreate`, `Validate`)
- Equality members (`Equals`, `GetHashCode`, operators)
- Conversion operators and `IParsable<T>` implementations
- Pattern matching methods (`Switch`, `Map`)
- Integration with serializers and frameworks

## Development Guidelines

### Validation Implementation
- **Always prefer `ValidateFactoryArguments`** over `ValidateConstructorArguments`
  - `ValidateFactoryArguments` returns `ValidationError` for better framework integration
  - `ValidateConstructorArguments` can only throw exceptions, integrates poorly with frameworks
- Use `ref` parameters to normalize values during validation
- Validation is used by factory methods (`Create`, `TryCreate`, `Validate`)

### Framework Integration
- **JSON/MessagePack Serialization**: Two approaches
  1. **Project Reference** (preferred): Reference integration packages in the project where types are defined
  2. **Manual Registration**: Register converter factories in `Startup.cs` or `Program.cs`
- **Entity Framework Core**: 
  - Use `.UseThinktectureValueConverters()` on `DbContextOptionsBuilder` for automatic value converters
  - Regular Discriminated Unions may need manual discriminator configuration
- **ASP.NET Core Model Binding**: 
  - Types rely on `IParsable<T>` interface (auto-generated)
  - Use `[ObjectFactory<string>]` for custom parsing logic from strings

### Pattern Matching
- Use generated `Switch`/`Map` methods for exhaustive, type-safe pattern matching
- Overloads exist to prevent closures for performance-critical scenarios

## Project Structure

### Source Projects (`src/`)
- Core library and framework-specific integrations
- Each integration has corresponding test project in `test/`
- `samples/` contains demonstration projects

### Key Files
- `Thinktecture.Runtime.Extensions.slnx`: Main solution file (.slnx format)
- `Directory.Build.props`: Global MSBuild properties (version, framework targets)
- `Directory.Packages.props`: Centralized NuGet package version management - **manage all package versions here**
- `global.json`: .NET SDK version specification (currently 9.0.0)
- `.github/copilot-instructions.md`: Detailed contributor guidance
- `.editorconfig`: Code style configuration (especially in `src/`)

## Testing Strategy
- xUnit for unit testing
- AwesomeAssertions for readable assertions
- Verify.Xunit for snapshot testing
- Comprehensive tests for generated code, serialization, and framework integration

## Code Style
- Follow `.editorconfig` settings (especially in `src/.editorconfig`)
- **XML documentation required** for all publicly visible types and members (except source generator, test, and sample projects)
- Multi-target framework support (net7.0 base, with EF Core version-specific projects)
- Use `dotnet format` to format the entire solution

## Common Patterns and Use Cases

### Value Objects
- **ISBN**: String-based with validation and normalization
- **Amount**: Decimal-based ensuring always positive and rounded
- **DateRange**: Complex object with start/end dates and validation

### Smart Enums
- **ShippingMethod**: Rich enum with pricing, delivery estimates, and calculation methods
- **ProductGroup**: Enum with custom properties referencing other enums
- **JSON Discriminator**: Using Smart Enum as type discriminator in JSON converters

### Discriminated Unions
- **Result<T>**: Generic union for success/failure without exceptions (`Success(T Value)`, `Failure(string Error)`)
- **PartiallyKnownDate**: Regular union for dates with varying precision (year only, year/month, full date)
- **Jurisdiction**: Complex domain modeling combining unions and value objects