# Project Structure and Architecture

## Repository Layout
```
Thinktecture.Runtime.Extensions/
├── docs/                          # Markdown documentation
├── samples/                       # Example projects demonstrating usage
├── src/                          # Source code
│   ├── Thinktecture.Runtime.Extensions/              # Core library
│   ├── Thinktecture.Runtime.Extensions.SourceGenerator/ # Source generators and analyzers
│   ├── Thinktecture.Runtime.Extensions.Json/         # System.Text.Json integration
│   ├── Thinktecture.Runtime.Extensions.Newtonsoft.Json/ # Newtonsoft.Json integration
│   ├── Thinktecture.Runtime.Extensions.MessagePack/  # MessagePack integration
│   ├── Thinktecture.Runtime.Extensions.ProtoBuf/     # ProtoBuf integration
│   ├── Thinktecture.Runtime.Extensions.EntityFrameworkCore7/ # EF Core 7 integration
│   ├── Thinktecture.Runtime.Extensions.EntityFrameworkCore8/ # EF Core 8 integration
│   ├── Thinktecture.Runtime.Extensions.EntityFrameworkCore9/ # EF Core 9 integration
│   ├── Thinktecture.Runtime.Extensions.AspNetCore/   # ASP.NET Core integration
│   └── Thinktecture.Runtime.Extensions.Swashbuckle/  # OpenAPI integration
├── test/                         # Unit tests (mirrors src structure)
└── test-results/                 # Test output files
```

## Core Components

### Core Library (`Thinktecture.Runtime.Extensions`)
- **Attributes**: `[ValueObject]`, `[SmartEnum]`, `[Union]` 
- **Base Interfaces**: `IValidationError`, `ISmartEnum`, `IComplexValueObject`
- **Utility Types**: `ValidationError`, `Empty` collections, `SingleItem`
- **Core Abstractions**: Foundation types for generated code

### Source Generator (`Thinktecture.Runtime.Extensions.SourceGenerator`)
- **Code Generation**: Creates boilerplate for Smart Enums, Value Objects, Unions
- **Analyzers**: Compile-time diagnostics and warnings
- **Code Fixes**: Automatic fixes for common issues
- **Performance**: Reflection-free generated code

### Integration Packages
Each integration package follows the same pattern:
- **Converters/Formatters**: Handle serialization/deserialization
- **Extension Methods**: Register with DI containers or configure options
- **Type Descriptors**: Enable framework-specific type conversion

## Key Design Patterns

### Source Generator Pattern
- Types are marked with attributes (`[ValueObject]`, `[SmartEnum]`, `[Union]`)
- Source generator creates `partial` implementations at compile-time
- Generated code includes constructors, factory methods, equality, comparison
- Developers implement validation and custom logic in user code

### Validation Pattern
```csharp
// Preferred validation approach
static partial void ValidateFactoryArguments(ref ValidationError? validationError, ref T value)
{
    // Return ValidationError for framework integration
    // Modify value by reference for normalization
}

// Legacy validation (avoid for new code)
static partial void ValidateConstructorArguments(T value)
{
    // Can only throw exceptions
}
```

### Factory Method Pattern
Generated types provide multiple creation methods:
- `Create(T value)`: Throws on validation failure
- `TryCreate(T value, out Type? result)`: Returns bool success
- `Validate(T value, IFormatProvider?, out Type? result)`: Returns ValidationError

### Pattern Matching Support
- `Switch()`: Execute actions based on type/value (void return)
- `Map<TResult>()`: Transform values based on type/value (typed return)
- Both methods are exhaustive by design

## Framework Integration Architecture

### Serialization Strategy
1. **Project Reference Approach** (Preferred):
   - Add integration package as project reference
   - Attributes automatically applied to types
   
2. **Manual Registration**:
   - Register converter factories in Startup/Program.cs
   - More flexible but requires manual setup

### Entity Framework Integration
- Use `.UseThinktectureValueConverters()` extension method
- Automatic value converter registration for all supported types
- Support for complex value objects with multiple properties

### ASP.NET Core Model Binding
- Relies on `IParsable<T>` interface (generated automatically)
- String-based types use `[ObjectFactory<string>]` for custom parsing
- Supports binding from route, query, and body parameters