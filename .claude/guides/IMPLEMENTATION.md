# Feature Implementation Guide

This guide covers both the architecture of the source generator system and step-by-step recipes for common development tasks. The architecture sections provide the reference knowledge; the recipes at the end provide actionable workflows.

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
    - Use `StringBuilder` for output
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
- **Newtonsoft.Json**: Similar pattern

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

Smart enums, keyed value objects, regular unions, and complex value objects can have their own generic type parameters (in addition to any key type from the attribute). For example:

```csharp
[SmartEnum<int>]
public partial class PriorityLevel<TContext> where TContext : notnull
{
    // TContext is the type's own generic parameter
    // int is the key type from the attribute
}
```

**Impact on source generators**: When a type has generic parameters, the generator must:

- Track `TypeArgumentsCount` in `ITypeInformation` to emit correct type signatures in generated code
- Handle fully qualified names that include generic arity (e.g., `PriorityLevel<TContext>` vs `PriorityLevel`)

Note: Generated partial declarations do not need to repeat generic constraints -- C# inherits them from the original declaration.

**Important**: Ad-hoc unions cannot be generic (the analyzer enforces this via a diagnostic).

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

Switch/Map methods are generated for Smart Enums, Regular Unions, and Ad-hoc Unions. The generation logic lives in dedicated code generators per type category.

### Generation process

1. **Item/variant discovery**: The generator collects items (Smart Enum fields), derived types (Regular Unions), or member types (Ad-hoc Unions) from the state object.
2. **Parameter name derivation**: Each item/variant name is converted to a camelCase parameter name. For Regular Unions with nested types, the `NestedUnionParameterNames` setting controls whether intermediate type names are included (e.g., `failureNotFound` vs `notFound`).
3. **Overload generation**: The generator emits multiple overloads per method -- `Switch` has void (`Action`) and returning (`Func<TResult>`) variants, plus state variants with a `TState` parameter. `Map` takes direct `TResult` values instead of lambdas. When `DefaultWithPartialNameMatching` is configured, `SwitchPartially`/`MapPartially` overloads are also generated with a `@default` fallback parameter.
4. **Dispatch logic**: The generated method body differs by type category:
    - **Smart Enums**: Identity comparison (`ReferenceEquals`) against the static item fields
    - **Regular Unions**: Type checks against derived types
    - **Ad-hoc Unions**: Switch on the internal discriminator index

### Configuration settings

- **`SwitchMapMethodsGeneration`**: Controls which overloads to generate (`Default`, `DefaultWithPartialNameMatching`, `None`)
- **`SwitchMapStateParameterName`**: Name of the state parameter in state overloads (default: `"state"`)

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

The `[ObjectFactory<T>]` attribute declares that a type supports creation from a value of type `T`. This drives the `ObjectFactorySourceGenerator`.

### What the attribute requires from the user

When `[ObjectFactory<T>]` is applied, the user must implement:

- **`Validate(T value, IFormatProvider? provider, out MyType? item)`**: Factory method that creates an instance from the given value type, returning a `ValidationError?`.
- **`ToValue()`** (when `UseForSerialization` is set): Conversion method returning `T`, used by serializers to convert back from the type to its representation.

### What the generator produces

The `ObjectFactorySourceGenerator` detects the attribute and generates:

- Adds `IObjectFactory<MyType, T, TValidationError>` to the type's interface list (the user's `Validate` method satisfies the interface contract)
- `IParsable<T>` implementation (when `T` is `string`) that delegates to `Validate`
- Serializer integration code (JSON converter attributes, etc.) based on `UseForSerialization`
- Adds `IConvertible<T>` to the type's interface list (the user's `ToValue` method satisfies the interface contract)

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

2. **`ThinktectureSpanParsableJsonConverter<T, TValidationError>`** (NET9+ only): A `System.Text.Json` converter that uses `Utf8JsonReaderHelper` to obtain a `ReadOnlySpan<char>` from the JSON reader. For deserialization, it calls `IObjectFactory<T, ReadOnlySpan<char>, TValidationError>.Validate()` to create the instance. For serialization, it calls `IConvertible<ReadOnlySpan<char>>.ToValue()` to get the value back.
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
2. **Indent Properly**: Use manual tracking
3. **Generate Readable Code**: Include comments and proper formatting
4. **Handle Nullability**: Emit nullable annotations (`?`) where appropriate
5. **XML Documentation**: Generate XML docs for public members
6. **Conditional Compilation**: Use `#if NET9_0_OR_GREATER` for version-specific code
7. **Attribute Lists**: Combine multiple attributes when possible
8. **Fully Qualified Names**: Use `global::` prefix for type references (e.g., `global::System.ArgumentException`) to avoid namespace conflicts -- do not generate `using` directives. Exception: C# language keywords (`string`, `int`, `bool`, etc.) are used as-is without `global::`
9. **File-Scoped Namespaces**: Use file-scoped namespaces for generated code (C# 10+)

## Performance Considerations

- **Incremental Generation**: Only regenerate when inputs change
- **Lightweight State**: Keep state objects small (no heavy `ISymbol` references)
- **Early Filtering**: Filter syntax nodes as early as possible
- **Avoid Allocations**: Reuse builders, avoid LINQ in hot paths
- **Equality Comparers**: Implement `IEquatable<T>` on state objects for proper caching

---

## Step-by-Step Recipes

Common repetitive development tasks in this repository, presented as step-by-step recipes.

### Recipe 1: Adding a New Property to an Existing Attribute

Example: how `DisableSpanBasedJsonConversion` was added to `SmartEnumAttribute<TKey>`.

**Step 1 -- Add the property to the attribute class**
File: `src/Thinktecture.Runtime.Extensions/SmartEnumAttribute.cs`

- Add a public property with appropriate default value and XML documentation.

**Step 2 -- Add a string constant for attribute property lookup**
File: `src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/Constants.cs`

- Add a `public const string` to `Constants.Attributes.Properties`. Value must exactly match the property name.

**Step 3 -- Add extraction method in AttributeDataExtensions**
File: `src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/AttributeDataExtensions.cs`

- Create a `FindXxx()` extension method on `AttributeData` (e.g. `FindDisableSpanBasedJsonConversion()`).
- Use the constant from step 2 to look up the named argument. Return a typed value with a default.

**Step 4 -- Update the settings class**
Files (depending on affected type):

- `src/.../CodeAnalysis/SmartEnums/SmartEnumSettings.cs` -- per-enum settings
- `src/.../CodeAnalysis/SmartEnums/AllEnumSettings.cs` -- merged settings for generation
- `src/.../CodeAnalysis/ValueObjects/ValueObjectSettings.cs` -- value object settings

Add the property. **Critical**: include it in `Equals()` and `GetHashCode()` -- the incremental generator cache depends on correct equality to avoid stale output.

**Step 5 -- Update the state object**
File: `src/.../CodeAnalysis/SmartEnums/SmartEnumSourceGeneratorState.cs` (example)

- Add a property, populate from settings, include in equality comparison and hash code.

**Step 6 -- Update the code generator**
Files: `SmartEnumCodeGenerator.cs`, `SmartEnumCodeGeneratorFactory.cs`, or serialization-specific factories (`JsonSmartEnumCodeGeneratorFactory.cs`, `NewtonsoftJsonSmartEnumCodeGeneratorFactory.cs`, `MessagePackSmartEnumCodeGeneratorFactory.cs`).

- Read the new setting from the state object; conditionally generate or omit code.

**Step 7 -- Add compilation test types**
Directory: `test/Thinktecture.Runtime.Extensions.Tests.Shared/TestEnums/` (or `TestValueObjects/`)

- Create types exercising the new property with different values.

**Step 8 -- Add snapshot tests**
File: `test/Thinktecture.Runtime.Extensions.SourceGenerator.Tests/SourceGeneratorTests/SmartEnumSourceGeneratorTests.cs`

- Add test methods verifying generated code changes. Uses the Verify framework (`.verified.cs` snapshots).

**Step 9 -- Update documentation**

- `.claude/reference/ATTRIBUTES.md` -- add property to attribute table
- `docs/*.md` -- user-facing documentation

---

### Recipe 2: Adding a New Analyzer Diagnostic

**Step 1 -- Define the DiagnosticDescriptor**
File: `src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/DiagnosticsDescriptors.cs`

- Choose next available ID (e.g. `TTRESG042`). IDs below 100 are errors; 100+ are warnings/info.
- Define `static readonly DiagnosticDescriptor` with title, message format, category `"Thinktecture.Runtime.Extensions"`, and severity.

**Step 2 -- Register and implement**
File: `src/.../CodeAnalysis/Diagnostics/ThinktectureRuntimeExtensionsAnalyzer.cs`

- Add to `SupportedDiagnostics`.
- Register with appropriate action (`RegisterSymbolAction`, `RegisterSyntaxNodeAction`, etc.).
- Call `context.ReportDiagnostic()` with precise `Location`.

**Step 3 -- Implement a code fix (if applicable)**
Directory: `src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/CodeFixes/`

- Create `CodeFixProvider`, register for the diagnostic ID, provide clear fix title.

**Step 4 -- Add tests**
Directory: `test/Thinktecture.Runtime.Extensions.SourceGenerator.Tests/AnalyzerAndCodeFixTests/`

- Name file after ID: `TTRESG042_Description.cs`.
- Test positive (triggers) and negative (no false positive) cases.
- If code fix exists, test before/after. Follow pattern of `TTRESG001_FieldMustBeReadOnly.cs`.

**Step 5 -- Update documentation**

- Add diagnostic to troubleshooting sections if it relates to a common user mistake.

---

### Recipe 3: Adding a New Source Generator Feature (End-to-End)

**Step 1 -- Design**
Decide what triggers generation, what gets generated, and configuration options.

**Step 2 -- Attributes/interfaces**
Directory: `src/Thinktecture.Runtime.Extensions/`

- Add or modify attribute classes and interfaces.

**Step 3 -- Syntax provider**
File: The generator's `Initialize` method (e.g. `SmartEnumSourceGenerator.cs`)

- Filter via `ForAttributeWithMetadataName` on the `IncrementalGeneratorInitializationContext.SyntaxProvider`.

**Step 4 -- Transform**
Extract semantic info from `GeneratorAttributeSyntaxContext`. Convert Roslyn symbols into plain data objects (state objects) to avoid holding compilation references.

**Step 5 -- State object**
Directory: `src/.../CodeAnalysis/{Feature}/`

- Must implement `IEquatable<T>` with correct `Equals`/`GetHashCode` (critical for incremental caching).
- Include every piece of data the code generator needs.

**Step 6 -- Code generator**
Directory: `src/.../CodeAnalysis/{Feature}/`

- Build C# source with `StringBuilder`. Follow existing generators.

**Step 7 -- Code generator factory**

- Implement `ICodeGeneratorFactory` (or `IKeyedSerializerCodeGeneratorFactory` / `IComplexSerializerCodeGeneratorFactory` for serialization).

**Step 8 -- Analyzer rules**
Add diagnostics for invalid configurations (see Recipe 2).

**Step 9 -- Test**

1. **Compilation test types** in `test/Thinktecture.Runtime.Extensions.Tests.Shared/`
2. **Snapshot tests** in `test/.../SourceGenerator.Tests/SourceGeneratorTests/`
3. **Behavior tests** in `test/Thinktecture.Runtime.Extensions.Tests/`
4. **Integration tests** in framework-specific test projects under `test/`

**Step 10 -- Document**

- `.claude/reference/ATTRIBUTES.md`, `docs/*.md`

---

### Recipe 4: Adding a New Framework Integration Package

**Step 1 -- Create the project**
Directory: `src/Thinktecture.Runtime.Extensions.{Framework}/`

- Follow existing naming: `.Json`, `.MessagePack`, `.EntityFrameworkCore10`, etc.
- Use an existing integration `.csproj` as a template.
- Add to `Thinktecture.Runtime.Extensions.slnx`.
- Configure framework targeting in `Directory.Build.props`.
- Add NuGet package versions to `Directory.Packages.props` (centralized version management).

**Step 2 -- Implement converters/formatters**

- Use `MetadataLookup.FindMetadataForConversion()` (`src/Thinktecture.Runtime.Extensions/Internal/MetadataLookup.cs`) for runtime type discovery.
- Object factory priority: factories override key-based metadata for conversion.
- Support both keyed types (single key member) and complex types (multiple members).
- If source-generated converters are needed, add a code generator factory (Recipe 3, steps 6-7).

**Step 3 -- Create test project**
Directory: `test/Thinktecture.Runtime.Extensions.{Framework}.Tests/`

- Round-trip serialization tests (serialize then deserialize, verify equality).
- Null handling, default values, invalid input, edge cases, custom configurations.
- Reference `Thinktecture.Runtime.Extensions.Tests.Shared` for shared test types.

**Step 4 -- Update documentation**

- Framework integration sections in user-facing docs.
- NuGet badge and usage instructions in `docs/Home.md`.
- If the integration requires source generation, document the attribute configuration.
