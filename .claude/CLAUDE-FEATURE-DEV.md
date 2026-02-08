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

- **`KeyedJsonCodeGenerator`**: Generates JSON converter attributes on the type. On NET9+, when `UseSpanBasedJsonConverter` is true, it emits `#if NET9_0_OR_GREATER` blocks that register `ThinktectureSpanParsableJsonConverterFactory<T, TValidationError>` for zero-allocation deserialization, with a fallback to the regular `ThinktectureJsonConverterFactory<T, TValidationError>` on older target frameworks.

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

### Zero-Allocation JSON via ObjectFactory (NET9+)

When `[ObjectFactory<ReadOnlySpan<char>>]` is present with `UseForSerialization = SerializationFrameworks.SystemTextJson`, the source generator sets `UseSpanBasedJsonConverter = true` for the type. This causes the generated JSON converter attribute to use `ThinktectureSpanParsableJsonConverterFactory` on NET9+, enabling zero-allocation deserialization by transcoding UTF-8 JSON bytes directly to `ReadOnlySpan<char>` without allocating a `string`. This is the opt-in mechanism for Value Objects (unlike Smart Enums with string keys, where it is automatic).

## Span-Based Zero-Allocation JSON Deserialization (NET9+)

This feature enables zero-allocation JSON deserialization for string-keyed Smart Enums and Value Objects with `ReadOnlySpan<char>` object factories, by converting UTF-8 JSON bytes directly to `ReadOnlySpan<char>` instead of allocating an intermediate `string`.

### Architecture

The feature spans three layers:

1. **`Utf8JsonReaderHelper`** (internal, `Thinktecture.Internal` namespace, NET9+ only): The low-level engine that converts UTF-8 JSON bytes to `ReadOnlySpan<char>` without string allocation.
   - **Fast path**: When the JSON value is contiguous in the buffer and unescaped, it transcodes the raw UTF-8 bytes directly via `Encoding.UTF8.GetChars(ReadOnlySpan<byte>, Span<char>)`.
   - **Slow path**: When the value is escaped or fragmented across buffer segments, it uses `Utf8JsonReader.CopyString(Span<byte>)` to get unescaped UTF-8 bytes, then transcodes.
   - **Memory strategy**: Uses `stackalloc` for values up to 128 chars; rents from `ArrayPool<char>.Shared` for larger values and returns the buffer after use.

2. **`ThinktectureSpanParsableJsonConverter<T, TValidationError>`** (NET9+ only): A `System.Text.Json` converter that uses `Utf8JsonReaderHelper` to obtain a `ReadOnlySpan<char>` from the JSON reader, then calls `IConvertible<ReadOnlySpan<char>>.ToValue()` on the target type to perform zero-allocation conversion.
   - **Type constraints**: `T : IObjectFactory<T, ReadOnlySpan<char>, TValidationError>, IConvertible<ReadOnlySpan<char>>`
   - Paired with `ThinktectureSpanParsableJsonConverterFactory<T, TValidationError>` for registration.

3. **`ThinktectureJsonConverterFactory`** (runtime detection): Updated with a NET9+-only constructor accepting `Func<Type, bool>? skipSpanBasedDeserialization`. At runtime, it inspects metadata via its internal `CanUseSpanParsableConverter()` method to decide whether to return the span-based converter or the regular converter for a given type.

### Runtime Converter Selection (`CanUseSpanParsableConverter`)

The `ThinktectureJsonConverterFactory` checks two paths to determine if a type supports span-based deserialization:

- **String-keyed Smart Enums**: Checks `Metadata.Keyed.SmartEnum.DisableSpanBasedJsonConversion`. If `false` (the default), the span-based converter is used.
- **Other types (Value Objects, etc.)**: Checks the type's `ObjectFactories` metadata for an entry whose `ValueType` is `ReadOnlySpan<char>` and whose `SerializationFrameworks` includes `SystemTextJson`.

The optional `skipSpanBasedDeserialization` delegate (passed via the constructor) provides an additional external override for consumers who need to disable span-based deserialization for specific types at the application level.

### How It Works for Smart Enums

**Automatic for string-keyed enums**: When a Smart Enum has a `string` key type, the source generator automatically enables span-based JSON deserialization (no user action needed).

**Opt-out**: Set `DisableSpanBasedJsonConversion = true` on the `[SmartEnum<string>]` attribute:

```csharp
[SmartEnum<string>(DisableSpanBasedJsonConversion = true)]
public partial class MyEnum
{
    public static readonly MyEnum Item1 = new("value1");
}
```

**Non-string keys**: Not applicable. The feature is only effective for `string`-keyed Smart Enums because `IConvertible<ReadOnlySpan<char>>` maps naturally to string-based key lookup.

### How It Works for Value Objects

**Opt-in via ObjectFactory**: Value Objects must explicitly declare `[ObjectFactory<ReadOnlySpan<char>>]` with `UseForSerialization = SerializationFrameworks.SystemTextJson`:

```csharp
[ValueObject<string>]
[ObjectFactory<ReadOnlySpan<char>>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
public partial class ProductName
{
    // The generated IObjectFactory<ProductName, ReadOnlySpan<char>, ValidationError>
    // implementation enables zero-allocation JSON deserialization
}
```

The `ObjectFactorySourceGenerator` detects the `ReadOnlySpan<char>` object factory and sets `UseSpanBasedJsonConverter = true` in the serializer generator state.

### Source Generator Behavior

**State tracking**: `KeyedSerializerGeneratorState` has a `UseSpanBasedJsonConverter` boolean property that is included in equality comparison and hash code computation (ensuring incremental generation correctness).

**Per-generator logic for setting `UseSpanBasedJsonConverter`**:

- **`SmartEnumSourceGenerator`**: Sets to `true` when `!state.Settings.DisableSpanBasedJsonConversion && state.KeyMember?.SpecialType == SpecialType.System_String`
- **`ValueObjectSourceGenerator`**: Always passes `false` (Value Objects require an explicit `[ObjectFactory<ReadOnlySpan<char>>]`)
- **`ObjectFactorySourceGenerator`**: Sets to `true` when `state.AttributeInfo.ObjectFactories.Any(f => f.IsReadOnlySpanOfChar)`

**Generated code pattern** (`KeyedJsonCodeGenerator`):

```csharp
// When UseSpanBasedJsonConverter = true
#if NET9_0_OR_GREATER
[System.Text.Json.Serialization.JsonConverter(typeof(
    global::Thinktecture.Text.Json.Serialization.ThinktectureSpanParsableJsonConverterFactory<MyType, ValidationError>))]
#else
[System.Text.Json.Serialization.JsonConverter(typeof(
    global::Thinktecture.Text.Json.Serialization.ThinktectureJsonConverterFactory<MyType, string, ValidationError>))]
#endif
partial class MyType { }
```

**Settings and constants**:

- `SmartEnumAttribute<TKey>.DisableSpanBasedJsonConversion`: New attribute property (default `false`)
- `AllEnumSettings` and `SmartEnumSettings`: Track the `DisableSpanBasedJsonConversion` value
- `Constants.DISABLE_SPAN_BASED_JSON_CONVERSION`: String constant for attribute property lookup
- `AttributeDataExtensions.FindDisableSpanBasedJsonConversion()`: Extension method to extract the setting from attribute data

**`IConvertible<T>` update**: On NET9+, the interface uses `allows ref struct` constraint to support `ReadOnlySpan<char>` as the type parameter.

## Runtime Metadata System

The runtime metadata system provides the bridge between compile-time source generation and runtime framework integration. This system is critical for serialization, model binding, type conversion, and other runtime operations.

### MetadataLookup Class

**Location**: `src/Thinktecture.Runtime.Extensions/Internal/MetadataLookup.cs`

The `MetadataLookup` class is the central runtime component that discovers and caches metadata for Smart Enums, Value Objects, and Unions.

**Key methods**:

```csharp
// Find metadata for a type
public static Metadata? Find(Type? type)

// Find conversion metadata with filtering (used by serializers)
public static ConversionMetadata? FindMetadataForConversion(
    Type? type,
    Func<ObjectFactoryMetadata, bool> objectFactoryFilter,
    Func<Metadata.Keyed, bool> metadataFilter)
```

### IMetadataOwner Pattern

All generated types implement the `IMetadataOwner` interface, which requires a static non-public property:

```csharp
static Metadata global::Thinktecture.Internal.IMetadataOwner.Metadata { get; }
```

**Source generators** create this property containing metadata about:
- The type itself
- Key member information (for keyed types)
- Object factories
- Validation error types
- Conversion expressions (lambda expressions for converting between types)

**Example generated property**:

```csharp
[SmartEnum<int>]
public partial class MyEnum
{
    // Generated by source generator
    static Metadata global::Thinktecture.Internal.IMetadataOwner.Metadata { get; } =
        new SmartEnumMetadata(...);
}
```

### Metadata Class Hierarchy

**`Metadata` (base class)**: Common metadata for all generated types
- `Type`: The type this metadata belongs to
- `ObjectFactories`: List of `ObjectFactoryMetadata` for custom factories

**`Metadata.Keyed` (abstract)**: Metadata for types with key members
- `KeyType`: The type of the key (int, string, Guid, custom types)
- `ValidationErrorType`: Type returned by validation methods
- `ConvertFromKeyExpression`: Lambda expression for converting from key to type

**Concrete implementations**:
- `SmartEnumMetadata`: For Smart Enums
  - Includes `Metadata.Keyed.SmartEnum.DisableSpanBasedJsonConversion` (`bool`, `init`): When `true`, prevents the runtime `ThinktectureJsonConverterFactory` from selecting the span-based JSON converter for this Smart Enum. Default `false`. Only meaningful for string-keyed enums. Set via `SmartEnumAttribute<TKey>.DisableSpanBasedJsonConversion` and emitted by `SmartEnumCodeGenerator` in metadata initialization.
- `KeyedValueObjectMetadata`: For simple Value Objects with single key
- `ComplexValueObjectMetadata`: For complex Value Objects with multiple members

**`ObjectFactoryMetadata`**: Metadata for `[ObjectFactory<T>]` attributes
- `ValueType`: The type used by the factory (e.g., `string` for `[ObjectFactory<string>]`)
- `ValidationErrorType`: Type returned by validation
- `ConvertFromKeyExpressionViaConstructor`: Lambda for creating instance from value

### ConversionMetadata

Used by serialization frameworks to convert between types and their representations:

```csharp
public class ConversionMetadata
{
    public Type Type { get; }
    public Type KeyType { get; }
    public Type? ValidationErrorType { get; }
    public LambdaExpression? FromCtor { get; }  // Constructor-based conversion
    public LambdaExpression? FromFactory { get; }  // Factory method conversion
}
```

**Serializers use this to**:
- Determine how to serialize (use `KeyType` representation)
- Determine how to deserialize (use `FromCtor` or `FromFactory` expressions)
- Handle validation errors (check `ValidationErrorType`)

### Runtime Discovery Process

1. **Lookup**: Call `MetadataLookup.Find(type)` or `MetadataLookup.FindMetadataForConversion(...)`
2. **Cache check**: Check `_metadata` or `_objectFactories` dictionary
3. **Type validation**: Verify type is a candidate (not primitive, array, enum, or pointer)
4. **Nullable unwrapping**: Extract underlying type from `Nullable<T>` if needed
5. **Interface check**: Verify type implements `IMetadataOwner`
6. **Base type traversal**: Walk up inheritance chain using `SearchBaseTypesForMetadata`
7. **Reflection**: Get the static `Metadata` property via reflection
8. **Caching**: Store result for future lookups

**Performance**: Metadata is discovered once per type and cached. Base type traversal handles inheritance hierarchies efficiently.

### Object Factory Priority System

**CRITICAL**: When both object factories and key-based metadata exist, **object factories have priority**.

From `MetadataLookup.cs:71-72`:
```csharp
// Object factories have priority over metadata
var metadataFromFactory = GetConversionMetadata(type, keyedMetadata, metadata.ObjectFactories, objectFactoryFilter);
```

**Implications**:
- `[ObjectFactory<string>]` on a `[SmartEnum<int>]` means string-based serialization takes precedence
- Multiple object factories can exist; use `objectFactoryFilter` to select the appropriate one
- This enables framework-specific customization (e.g., ASP.NET Core uses `[ObjectFactory<string>]` for model binding)

### Filtering Mechanism

`FindMetadataForConversion` accepts two filters:

1. **`objectFactoryFilter`**: Select appropriate `ObjectFactoryMetadata`
   - Example: `f => f.ValueType == typeof(string)` for string-based factories
   - `LastOrDefault` is used, so later-declared factories have priority

2. **`metadataFilter`**: Select appropriate `Metadata.Keyed`
   - Example: Check if key type matches expected type
   - Used to ensure metadata compatibility with serializer requirements

**Why filtering is needed**:
- A type may have multiple `[ObjectFactory<T>]` attributes for different scenarios
- Serializers need to select factories matching their serialization format
- ASP.NET Core model binding needs string-based factories, while MessagePack might need byte-based

### Practical Implications for Feature Development

When implementing new features:

1. **Adding new metadata**: Extend `Metadata` hierarchy and update source generators to create instances
2. **New serialization framework**: Use `MetadataLookup.FindMetadataForConversion` with appropriate filters
3. **Custom conversion logic**: Implement via `[ObjectFactory<T>]` and ensure metadata includes conversion expressions
4. **Testing runtime behavior**: Mock `Metadata` instances or create test types with `IMetadataOwner` implementation
5. **Performance**: Leverage caching - metadata lookup is expensive on first call, cheap on subsequent calls
6. **Inheritance**: Remember that metadata is searched up the base type chain

**Example - New Serializer Integration**:

```csharp
// In your custom JsonConverter or MessagePackFormatter
var conversionMetadata = MetadataLookup.FindMetadataForConversion(
    type,
    objectFactoryFilter: f => f.ValueType == typeof(string),  // Need string-based factory
    metadataFilter: m => m.KeyType == typeof(string)  // Or string key type
);

if (conversionMetadata != null)
{
    // Use conversionMetadata.FromFactory or FromCtor to deserialize
    // Use conversionMetadata.KeyType to serialize
}
```

**Common Pitfalls**:
- Forgetting that object factories override key-based metadata
- Not handling null metadata (type might not have metadata)
- Ignoring `ValidationErrorType` (should check for validation failures)
- Assuming metadata exists without checking `IMetadataOwner` interface implementation

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
