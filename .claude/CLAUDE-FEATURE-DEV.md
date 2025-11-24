# Feature Development Guide

This guide provides detailed information for implementing new features in Thinktecture.Runtime.Extensions. Consult this when working on source generators, analyzers, or core library features.

## Source Generator Architecture

The source generators follow a consistent incremental generation pattern optimized for performance and IDE responsiveness.

### Incremental Generator Pattern

All generators implement `IIncrementalGenerator` with this architecture:

1. **Syntax Providers**: Filter for attributed types early in the pipeline
   - Use `ForAttributeWithMetadataName` for efficient filtering
   - Example: `context.SyntaxProvider.ForAttributeWithMetadataName("Thinktecture.SmartEnumAttribute")`

2. **Transform Providers**: Extract semantic information into lightweight state objects
   - Transform syntax nodes to state objects containing only necessary information
   - Avoid carrying heavy `ISymbol` references through the pipeline
   - Example: Transform `INamedTypeSymbol` to `SmartEnumSourceGeneratorState`

3. **Code Generators**: Produce source code from state
   - Receive state objects and generate C# code
   - Use `StringBuilder` or `CodeGenerationStringBuilder` for output
   - Register generated source with `context.AddSource(hintName, sourceText)`

### Pipeline Architecture

```
SyntaxProvider (filter)
  → Transform (semantic analysis)
  → State Object (lightweight)
  → Code Generator (output)
```

**Key principle**: Move expensive operations (semantic analysis) as late as possible, and cache results in state objects.

## State Objects

Each generator has corresponding state classes that capture all information needed for code generation:

### Smart Enum State

- **`SmartEnumSourceGeneratorState`**: Complete state for smart enum generation
  - Contains type information, key member settings, items, derived types
  - Includes configuration (operators, interfaces, serialization)

### Value Object State

- **`KeyedValueObjectSourceGeneratorState`**: State for simple value objects with single key member
- **`ComplexValueObjectSourceGeneratorState`**: State for complex value objects with multiple members
  - Contains member information, equality comparers, validation settings

### Union State

- **`AdHocUnionSourceGeneratorState`**: State for ad-hoc unions (Union<T1, T2, ...>)
- **`RegularUnionSourceGeneratorState`**: State for inheritance-based unions

### Common State Interfaces

- **`ITypeInformation`**: Common type metadata (name, namespace, accessibility, containing types, generic parameters)
- **`ITypedMemberState`**: Member information with type details
- **`IMemberState`**: Basic member information
- **`IKeyMemberSettings`**: Configuration for key members (name, accessibility, kind)

## Code Generators

Code generation is separated into specialized generators for maintainability:

### Interface Implementations

- **`ParsableCodeGenerator`**: Generates `IParsable<T>` implementation
  - `Parse(string, IFormatProvider?)` and `TryParse(string, IFormatProvider?, out T?)`
  - Uses key member parsing or custom object factory

- **`SpanParsableCodeGenerator`**: Generates `ISpanParsable<T>` implementation (NET9+)
  - `Parse(ReadOnlySpan<char>, IFormatProvider?)` and `TryParse(ReadOnlySpan<char>, IFormatProvider?, out T?)`
  - Zero-allocation parsing via `StaticAbstractInvoker.ParseValue<TKey>`

- **`ComparableCodeGenerator`**: Generates `IComparable<T>` implementation
  - `CompareTo(T?)` method using key member or custom comparer

- **`FormattableCodeGenerator`**: Generates `IFormattable` implementation
  - `ToString(string?, IFormatProvider?)` using key member formatting

- **`InterfaceCodeGeneratorFactory`**: Factory for creating interface code generators

### Operator Generators

- **`ComparisonOperatorsCodeGenerator`**: `<`, `<=`, `>`, `>=` operators
- **`EqualityComparisonOperatorsCodeGenerator`**: `==`, `!=` operators
- **`AdditionOperatorsCodeGenerator`**: `+` operator (value objects only)
- **`SubtractionOperatorsCodeGenerator`**: `-` operator (value objects only)
- **`MultiplyOperatorsCodeGenerator`**: `*` operator (value objects only)
- **`DivisionOperatorsCodeGenerator`**: `/` operator (value objects only)

### Serialization Integration

Separate code generator factories for each serialization framework:

- **`JsonSmartEnumCodeGeneratorFactory`**: System.Text.Json integration for smart enums
- **`JsonValueObjectCodeGeneratorFactory`**: System.Text.Json integration for value objects
- **`MessagePackSmartEnumCodeGeneratorFactory`**: MessagePack integration for smart enums
- **`MessagePackValueObjectCodeGeneratorFactory`**: MessagePack integration for value objects
- **Newtonsoft.Json and ProtoBuf**: Similar pattern

**Pattern**: Each factory checks if the corresponding serialization package is referenced, then generates appropriate converter/formatter registration code.

## Type Information System

Rich type metadata is captured and passed through the generation pipeline:

### ITypeInformation Interface

```csharp
- TypeFullyQualified: Fully qualified type name (with namespace)
- TypeMinimallyQualified: Type name with containing types (no namespace)
- Name: Simple type name
- Namespace: Namespace or null
- IsReferenceType: True if class, false if struct
- IsSealed: Sealed modifier present
- Accessibility: Public, Internal, etc.
- ContainingTypes: List of containing types (for nested types)
- TypeArgumentsCount: Number of generic type parameters
- HasStructLayoutAttribute: StructLayout attribute present
```

### Containing Types

**`ContainingTypeState`**: Represents a containing type for nested types
- Name, Accessibility, TypeArgumentsCount
- Properly handles nested generic types

### Generic Type Support

Smart enums, keyed value objects, regular unions, and complex value objects can be generic:

```csharp
public partial class SmartEnum_Generic<T> where T : IEquatable<T>
{
    // T is the enum's generic parameter
}

[SmartEnum<int>] // int is the key type
public partial class SmartEnum_Generic<T> where T : IEquatable<T>
{
    // Both T (enum's parameter) and int (key type) are handled
}
```

**Important**: Ad-hoc unions cannot be generic (analyzer enforces this).

## Nullability Awareness

Full support for nullable reference types and value types:

### NullableAnnotation

Captured from Roslyn's `ITypeSymbol.NullableAnnotation`:
- `None`: No nullability information
- `NotAnnotated`: Non-nullable reference type or value type
- `Annotated`: Nullable reference type (`string?`) or nullable value type (`int?`)

### Special Types

- **`SpecialType`**: Enum representing built-in types (String, Int32, Boolean, etc.)
- Used for optimization (direct type name generation without symbol lookup)

## Pattern Matching Code Generation

Switch/Map methods provide exhaustive pattern matching:

### Smart Enums and Regular Unions

```csharp
// Switch (void return)
myEnum.Switch(
    item1: () => DoSomething(),
    item2: () => DoSomethingElse()
);

// Map (TResult return)
var result = myEnum.Map(
    item1: () => "Value1",
    item2: () => "Value2"
);

// With state parameter (avoids closures)
myEnum.Switch(
    state: myState,
    item1: (state) => DoSomething(state),
    item2: (state) => DoSomethingElse(state)
);
```

### Configuration

- **`SwitchMapMethodsGeneration`**: Enum controlling which overloads to generate
  - `Default`: Standard overloads
  - `DefaultWithPartialNameMatching`: Allow partial name matching
  - `None`: Skip generation

- **`SwitchMapStateParameterName`**: Customize state parameter name (default: "state")

## StaticAbstractInvoker Pattern (NET9+)

Used for generic parsing with `ISpanParsable<T>`:

```csharp
internal static class StaticAbstractInvoker
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TValue ParseValue<TValue>(
        ReadOnlySpan<char> s,
        IFormatProvider? provider)
        where TValue : ISpanParsable<TValue>, allows ref struct
    {
        return TValue.Parse(s, provider);
    }
}
```

**Key points**:
- Uses C# 11 static abstract interface members (SAIM)
- `allows ref struct` constraint (NET9+) supports `ReadOnlySpan<char>`
- Marked with `AggressiveInlining` for performance
- Enables zero-allocation parsing for any `ISpanParsable<T>` key type

## Custom Key Types

Users can define custom key types implementing `ISpanParsable<T>`:

```csharp
public readonly struct CustomKey : ISpanParsable<CustomKey>
{
    public static CustomKey Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        // Custom parsing logic
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out CustomKey result)
    {
        // Custom parsing logic
    }
}

[SmartEnum<CustomKey>]
public partial class MyEnum { }
```

The source generator automatically detects `ISpanParsable<T>` and generates appropriate code.

## Object Factory Pattern

Custom factories allow overriding default parsing/serialization behavior:

### Attribute

```csharp
[ObjectFactory<string>] // Parse from string
[ObjectFactory<ReadOnlySpan<char>>] // Parse from span (NET9+)
public partial class MyValueObject
{
    private MyValueObject(string value)
    {
        // Custom constructor
    }
}
```

### Generated Code

The generator creates:
- `IParsable<T>` implementation using the custom constructor
- Integration with serializers (JSON, MessagePack, etc.)
- Model binding for ASP.NET Core

## Diagnostic Implementation

When implementing analyzers:

### DiagnosticDescriptor

Define descriptics with:
- **Id**: Unique identifier (e.g., "TTRESG001")
- **Title**: Short description
- **MessageFormat**: Detailed message with placeholders
- **Category**: "Thinktecture.Runtime.Extensions"
- **DiagnosticSeverity**: Error, Warning, Info
- **IsEnabledByDefault**: Usually true

### Registration

```csharp
public override void Initialize(AnalysisContext context)
{
    context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
    context.EnableConcurrentExecution();

    context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
}
```

### Best Practices

- Report diagnostics with precise locations (use `GetLocation()` on syntax nodes)
- Provide clear, actionable error messages
- Include code fix providers when possible
- Test with edge cases and invalid input

## Code Generation Best Practices

1. **Use StringBuilder**: For efficient string concatenation
2. **Indent Properly**: Use `IndentedStringBuilder` or manual tracking
3. **Generate Readable Code**: Include comments and proper formatting
4. **Handle Nullability**: Emit nullable annotations (`?`) where appropriate
5. **XML Documentation**: Generate XML docs for public members
6. **Conditional Compilation**: Use `#if NET9_0_OR_GREATER` for version-specific code
7. **Attribute Lists**: Combine multiple attributes when possible
8. **Optimize Imports**: Only generate necessary using directives
9. **File-Scoped Namespaces**: Use file-scoped namespaces for generated code (C# 10+)
10. **Global Usings**: Consider global using directives for common types

## Performance Considerations

- **Incremental Generation**: Only regenerate when inputs change
- **Lightweight State**: Keep state objects small (no heavy `ISymbol` references)
- **Caching**: Use `CompilationProvider.Combine` to cache per-compilation data
- **Early Filtering**: Filter syntax nodes as early as possible
- **Avoid Allocations**: Reuse builders, avoid LINQ in hot paths
- **Equality Comparers**: Implement `IEquatable<T>` on state objects for proper caching

## Testing Source Generators

Use the `CompilationTestBase` for testing:

```csharp
public class MyGeneratorTests : CompilationTestBase
{
    [Fact]
    public async Task Should_generate_expected_code()
    {
        // Arrange
        var source = @"
            [SmartEnum<int>]
            public partial class MyEnum { }
        ";

        // Act
        var output = await GetGeneratedOutputAsync(source, typeof(SmartEnumSourceGenerator));

        // Assert
        await Verify(output);
    }
}
```

**Use Verify.Xunit** for snapshot testing of generated code.
