# Code Style and Conventions

## General Guidelines

### Naming Conventions
- **Classes/Interfaces/Enums/Properties**: PascalCase
- **Methods**: PascalCase  
- **Fields**: camelCase with underscore prefix for private fields (`_fieldName`)
- **Parameters/Local Variables**: camelCase
- **Constants**: PascalCase or ALL_CAPS (depending on context)

### Language Features
- **Nullable Reference Types**: Enabled (`<Nullable>enable</Nullable>`)
- **Implicit Usings**: Enabled (`<ImplicitUsings>enable</ImplicitUsings>`)
- **Language Version**: C# 13.0
- **Target Framework**: .NET 7.0 baseline

### Documentation
- **XML Documentation**: Required for all publicly visible types and members
- Exception: Source generators, test projects, and sample projects are exempt
- Use clear, concise descriptions that explain purpose and usage

### File Organization
- **Root Namespace**: `Thinktecture` for all projects
- **Partial Classes**: Common for types using source generators (Smart Enums, Value Objects, Unions)
- **Test Structure**: Mirror the source directory structure in test projects

## Source Generator Patterns

### Value Objects
```csharp
[ValueObject<string>]  // Simple value object
public partial struct CustomerId 
{
    // Validation via static partial method
    static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            validationError = new ValidationError("Customer ID cannot be empty");
    }
}

[ComplexValueObject]  // Multiple properties
public partial class DateRange
{
    public DateOnly Start { get; }
    public DateOnly End { get; }
    
    static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref DateOnly start, ref DateOnly end)
    {
        if (end < start)
            validationError = new ValidationError("End date cannot be before start date");
    }
}
```

### Smart Enums
```csharp
[SmartEnum<string>]
public partial class ShippingMethod
{
    public static readonly ShippingMethod Standard = new("STANDARD", basePrice: 5.99m);
    public static readonly ShippingMethod Express = new("EXPRESS", basePrice: 15.99m);
    
    private readonly decimal _basePrice;
    
    public decimal CalculatePrice(decimal weight) => _basePrice + (weight * 0.5m);
}
```

### Discriminated Unions
```csharp
// Ad-hoc unions
[Union<string, int>]
public partial class TextOrNumber;

// Regular unions
[Union]
public partial record Result<T>
{
    public record Success(T Value) : Result<T>;
    public record Failure(string Error) : Result<T>;
}
```

## EditorConfig Compliance
- Follow the settings defined in `.editorconfig` and `src/.editorconfig`
- Use `dotnet format` to ensure compliance
- Key settings:
  - Warning level for CA1852 (sealed classes)
  - UTF-8 encoding for verified test files
  - LF line endings for verification files

## Testing Conventions
- Use **xUnit** framework
- Test file naming: `[ClassName]Tests.cs`
- Test method naming: `[MethodUnderTest]_Should[ExpectedBehavior]_When[Condition]`
- Use **AwesomeAssertions** for readable assertions: `result.Should().Be(expected)`
- Use **Verify.Xunit** for snapshot testing of generated code
- Mock dependencies with **NSubstitute**

## Performance Considerations
- Source generators run at compile-time, not runtime
- Generated code is optimized for performance (reflection-free enumeration, fast lookups)
- Use static methods where possible to avoid closures in Switch/Map operations
- Prefer structs for simple value objects to reduce heap allocations