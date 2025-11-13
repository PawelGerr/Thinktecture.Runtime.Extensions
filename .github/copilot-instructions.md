This file provides guidance to the AI assistant when working with code in this repository.

## Common Commands

### Build and Test

- **Build solution**: `dotnet build`
- **Restore packages**: `dotnet restore`
- **Run all tests**: `dotnet test`

### Development Requirements

- .NET 10.0 SDK (as specified in global.json)
- C# 11+ for generated code
- Multiple .NET versions (7.0, 8.0, 9.0, 10.0) for framework compatibility testing

## Architecture Overview

This is a .NET library providing **Smart Enums**, **Value Objects**, and **Discriminated Unions** through Roslyn Source Generators. The core architecture consists of:

### Core Components

- **`src/Thinktecture.Runtime.Extensions`**: Core library with base interfaces, attributes, and runtime helpers
    - Attributes: `SmartEnumAttribute<TKey>`, `SmartEnumAttribute`, `ValueObjectAttribute<TKey>`, `ComplexValueObjectAttribute`, `UnionAttribute<T1,T2,...>` (up to 5 type parameters), `AdHocUnionAttribute` (non-generic alternative to UnionAttribute), `UnionAttribute`, `ObjectFactoryAttribute<T>`
    - Additional attributes: `KeyMemberEqualityComparerAttribute`, `KeyMemberComparerAttribute`, `MemberEqualityComparerAttribute`, `IgnoreMemberAttribute`, `ValidationErrorAttribute`, `UseDelegateFromConstructorAttribute`, `UnionSwitchMapOverloadAttribute`
    - Key interfaces: `ISmartEnum<TKey>`, `IEnum`, `IKeyedObject<TKey>`, `IKeyedValueObject<TKey>`, `IComplexValueObject`, `IValidationError`, `IObjectFactory<T>`
    - Utility types: `ValidationError`, `ComparerAccessors`, collection helpers (EmptyDictionary, EmptyLookup, SingleItemReadOnlyDictionary, etc.)
    - Enums: `AccessModifier`, `MemberKind`, `OperatorsGeneration`, `ConversionOperatorsGeneration`, `SwitchMapMethodsGeneration`, `SerializationFrameworks`, `UnionConstructorAccessModifier`
- **`src/Thinktecture.Runtime.Extensions.SourceGenerator`**: Roslyn Source Generators and Analyzers that create boilerplate code and validate usage for partial classes/structs
    - Contains 6 source generators, 2 analyzers, and 1 code fix provider
    - Organized into subdirectories: SmartEnums, ValueObjects, AdHocUnions, RegularUnions, ObjectFactories, Annotations, Diagnostics, CodeFixes
- **Framework Integration Projects**: Separate projects for JSON, MessagePack, Newtonsoft.Json, ProtoBuf, Entity Framework Core (7/8/9), ASP.NET Core, and Swashbuckle
    - **EF Core**: Shared sources in `EntityFrameworkCore.Sources`, version-specific projects for EF Core 7/8/9
    - **Serialization**: Converters/formatters for System.Text.Json, MessagePack, Newtonsoft.Json, and ProtoBuf

### Source Generators and Analyzers

The `Thinktecture.Runtime.Extensions.SourceGenerator` project contains multiple incremental source generators and analyzers:

#### Source Generators

1. **`SmartEnumSourceGenerator`**: Generates code for types annotated with `[SmartEnum<T>]` or `[SmartEnum]`
    - Creates factory methods (`Create`, `TryCreate`, `Get`, `Parse`, `TryParse`)
    - Generates equality, comparison, and conversion operators
    - Implements `IParsable<T>`, `IComparable<T>`, `IFormattable` interfaces
    - Generates exhaustive `Switch`/`Map` methods for pattern matching
    - Integrates with serializers (JSON, MessagePack, Newtonsoft.Json)

2. **`ValueObjectSourceGenerator`**: Generates code for types annotated with `[ValueObject<T>]` or `[ComplexValueObject]`
    - Handles both keyed (simple) and complex value objects
    - Creates factory methods (`Create`, `TryCreate`, `Validate`)
    - Generates equality members (`Equals`, `GetHashCode`, operators)
    - Implements `IParsable<T>`, `IComparable<T>`, `IFormattable` interfaces
    - Supports arithmetic operators (`+`, `-`, `*`, `/`) when configured
    - Integrates with serializers (JSON, MessagePack, Newtonsoft.Json)

3. **`AdHocUnionSourceGenerator`**: Generates code for ad-hoc unions annotated with `[Union<T1, T2, ...>]` or `[AdHocUnion(typeof(T1), typeof(T2), ...)]` (up to 5 types)
    - Supports both generic and non-generic attribute syntaxes (identical functionality)
    - Creates implicit conversion operators for each union member type
    - Generates exhaustive `Switch`/`Map` methods for pattern matching
    - Provides type-safe value access with `IsT1`, `AsT1` properties and methods

4. **`RegularUnionSourceGenerator`**: Generates code for inheritance-based unions annotated with `[Union]`
    - Creates static factory methods for each derived type
    - Generates exhaustive `Switch`/`Map` methods for all derived types
    - Supports complex domain hierarchies with multiple derived types

5. **`ObjectFactorySourceGenerator`**: Generates code for types with custom factories annotated with `[ObjectFactory<T>]`
    - Creates custom serialization/deserialization logic
    - Implements `IParsable<T>` with custom parsing logic
    - Integrates with serializers using custom converter factories

6. **`AnnotationsSourceGenerator`**: Generates JetBrains ReSharper/Rider annotations if not already present
    - Adds attributes like `[InstantHandle]`
    - Improves IDE static analysis and code completion
    - Only generates if JetBrains.Annotations package is not referenced

#### Analyzers

1. **`ThinktectureRuntimeExtensionsAnalyzer`**: Main diagnostic analyzer with 40+ diagnostic rules that validates correct usage of library features
    - **Type structure**: Ensures types with `[SmartEnum]`, `[ValueObject]`, `[Union]`, or `[AdHocUnion]` are `partial`, are class/struct, not generic (except regular unions and complex value objects), not nested in generic types
    - **Constructor rules**: Validates constructors are private, no primary constructors allowed
    - **Member validation**:
        - Fields must be readonly, properties must be readonly (no set) or have private init
        - Smart enum items must be public static readonly/const
        - Key members should not be nullable, custom key member implementations must match types
        - Members disallowing default values must be required
    - **Comparer validation**: Validates KeyMemberEqualityComparer/KeyMemberComparer usage, warns about string-based value objects without explicit equality comparer
    - **Smart enum specific**: Key member names must be allowed, enums without derived types must be sealed, must have at least one item, derived types must be properly accessible
    - **Union specific**: Must be sealed or have private constructors only, records must be sealed, derived types must not be generic, accessibility rules for non-abstract derived types
    - **Object factory**: Validates corresponding constructor exists, smart enums cannot use object factory constructors
    - **Switch/Map usage**: Index-based switch/map must use named parameters, validates exhaustiveness
    - **UseDelegateFromConstructor**: Methods must be partial and not have generics
    - **Struct defaults**: Validates AllowDefaultStructs settings consistency

2. **`ThinktectureRuntimeExtensionsInternalUsageAnalyzer`**: Prevents usage of internal library APIs outside Thinktecture modules
    - Detects usage of types/members in `Thinktecture.Internal` namespace
    - Validates that types with `[ModuleInternalAttribute]` are only used within the library
    - Prevents accidental dependencies on internal implementation details

#### Code Fix Provider

- **`ThinktectureRuntimeExtensionsCodeFixProvider`**: Provides automatic fixes for common diagnostic issues
    - Can automatically add `partial` keyword to types
    - Fixes member accessibility issues
    - Other code fixes for analyzer diagnostics

### Key Concepts

#### 1. Smart Enums

**Keyless**: `[SmartEnum]` - Type-safe enums without underlying values

- Items are defined as public static readonly fields
- Cannot use comparison operators (no key to compare)
- Supports `Switch`/`Map` methods, equality operators, serialization
- Must be sealed if no derived types

**Keyed**: `[SmartEnum<TKey>]` - Type-safe enums with underlying key values

- Key can be any non-nullable type (int, string, Guid, etc.)
- Configurable key member via `KeyMemberName`, `KeyMemberAccessModifier`, `KeyMemberKind`
- Supports comparison operators if key is comparable
- Supports `IParsable<T>`, `IComparable<T>`, `IFormattable` (depending on key type)
- Configurable conversion operators: `ConversionToKeyMemberType`, `ConversionFromKeyMemberType`
- Can skip interfaces: `SkipIComparable`, `SkipIParsable`, `SkipIFormattable`, `SkipToString`
- Can control operator generation: `ComparisonOperators`, `EqualityComparisonOperators`
- Can control `Switch`/`Map` generation: `SwitchMethods`, `MapMethods`

**Custom Behavior**: Use `[UseDelegateFromConstructor]` on partial methods to inject delegate parameters into constructor

#### 2. Value Objects

**Simple (Keyed)**: `[ValueObject<TKey>]` - Single-value immutable types

- Key member is the single value (e.g., `Amount`, `ProductId`)
- Default key member name: `_value` (private field) or `Value` (public)
- Configurable via `KeyMemberName`, `KeyMemberAccessModifier`, `KeyMemberKind`
- **String keys**: MUST specify `[KeyMemberEqualityComparer<MyType, string, ...>]` for proper string comparison (Ordinal, OrdinalIgnoreCase, etc.)
- Null handling: `NullInFactoryMethodsYieldsNull`, `EmptyStringInFactoryMethodsYieldsNull`
- Supports arithmetic operators: `AdditionOperators`, `SubtractionOperators`, `MultiplyOperators`, `DivisionOperators`
- Comparison/equality operators: `ComparisonOperators`, `EqualityComparisonOperators`
- Conversion operators: `ConversionToKeyMemberType`, `UnsafeConversionToKeyMemberType`, `ConversionFromKeyMemberType`
- Can skip key member generation: `SkipKeyMember` (you implement it manually)
- **Cannot be generic**: Keyed value objects must not have type parameters

**Complex**: `[ComplexValueObject]` - Multi-property immutable types

- Multiple properties define the value (e.g., `DateRange` with StartDate and EndDate)
- Properties are auto-discovered (non-static, non-ignored)
- Use `[IgnoreMember]` to exclude properties from equality/comparisons
- Use `[MemberEqualityComparer<MyType, TMember, ...>]` for custom per-member equality
- **String members**: Analyzer warns if no explicit equality comparer is specified
- All members participate in `Equals`, `GetHashCode` unless ignored
- **Can be generic**: Complex value objects can have type parameters (e.g., `Range<T> where T : IComparable<T>`)

**Common settings**:

- `SkipFactoryMethods`: Skip generation of `Create`, `TryCreate`, `Validate`
- `SkipIParsable`, `SkipIComparable`, `SkipIFormattable`: Skip interface implementations
- `SkipEqualityComparison`: Skip generation of equality members (`Equals`, `GetHashCode`, `==`, `!=`, `IEquatable<T>`) - also sets `ComparisonOperators` and `EqualityComparisonOperators` to `None`
- Validation: Implement `ValidateFactoryArguments` (preferred) or `ValidateConstructorArguments`

#### 3. Discriminated Unions

**Ad-hoc Unions**: `[Union<T1, T2>]` through `[Union<T1, T2, T3, T4, T5>]`, or non generic `[AdHocUnion(Type t1, Type t2, Type? t3, Type? t4, Type? t5)]`

- Simple combination of 2-5 types without inheritance
- Two syntaxes available:
    - **Generic** (recommended): `[Union<string, int>]` - simpler syntax for most cases
    - **Non-generic**: `[AdHocUnion(typeof(string), typeof(int))]` - use when generic attribute limitations are hit (e.g., nullable reference types in generic collections like `List<string?>`, or other C# generic attribute constraints)
- Both syntaxes generate identical functionality and support the same customization options
- Generates implicit conversion operators from each type to union (configurable via `ConversionFromValue`)
- Type checking: `IsT1`, `IsT2`, etc. properties
- Value access: `AsT1`, `AsT2`, etc. properties (throws if wrong type)
- Configurable member names: `T1Name`, `T2Name`, etc.
- Nullable reference types: `T1IsNullableReferenceType`, `T2IsNullableReferenceType`, etc.
- `SkipEqualityComparison`: Skip generation of equality members (`Equals`, `GetHashCode`, `==`, `!=`, `IEquatable<T>`) for ad-hoc unions only
- Supports `Switch`/`Map` methods for exhaustive pattern matching

**Regular (Inheritance-based) Unions**: `[Union]`

- Base class/record represents the union, derived types are the alternatives
- Must be sealed or have only private constructors
- Records must be sealed
- Generates static factory methods for each derived type
- Supports `Switch`/`Map` methods over all derived types
- Can control constructor accessibility: `UnionConstructorAccessModifier`
- Use `[UnionSwitchMapOverload]` to customize generated `Switch`/`Map` overloads

**Common settings**:

- `SwitchMethods`, `MapMethods`: Control generation of pattern matching methods
- `SwitchMapStateParameterName`: Customize state parameter name (default: "state")

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

#### Serialization

The library supports multiple serialization frameworks through dedicated integration packages:

1. **System.Text.Json** (`Thinktecture.Runtime.Extensions.Json`)
    - `ThinktectureJsonConverterFactory`: Auto-registers converters for Smart Enums, Value Objects, and Unions
    - Specialized converters for numeric key types (Int, Long, Byte, Short, UInt, ULong, UShort, Single, Double, Decimal, SByte)
    - **Integration**: Either reference the package in your project (source generator auto-generates integration code) OR manually register `ThinktectureJsonConverterFactory` in JsonSerializerOptions

2. **MessagePack** (`Thinktecture.Runtime.Extensions.MessagePack`)
    - `ThinktectureMessageFormatterResolver`: Resolves formatters for generated types
    - Separate formatters for classes (`ThinktectureMessagePackFormatter`) and structs (`ThinktectureStructMessagePackFormatter`)
    - **Integration**: Either reference the package OR manually register the resolver

3. **Newtonsoft.Json** (`Thinktecture.Runtime.Extensions.Newtonsoft.Json`)
    - Provides JsonConverter implementations for compatibility with Newtonsoft.Json
    - **Integration**: Either reference the package OR manually register converters

4. **ProtoBuf** (`Thinktecture.Runtime.Extensions.ProtoBuf`)
    - Support for Protocol Buffers serialization
    - **Integration**: Reference the package in your project

**General pattern**: The source generator detects which serialization packages are referenced and automatically generates appropriate integration code. You can also opt out using the `SerializationFrameworks` property on attributes.

#### Entity Framework Core

Three version-specific packages (`EntityFrameworkCore7`, `EntityFrameworkCore8`, `EntityFrameworkCore9`, `EntityFrameworkCore10`) with shared implementation in `EntityFrameworkCore.Sources`:

- **Value Converters**: Automatically converts Value Objects and Smart Enums to/from their underlying types
- **Setup**: Call `.UseThinktectureValueConverters()` on `DbContextOptionsBuilder` to register automatic value converters
- **Advanced**: Use `.UseValueObjectValueConverter<TValueObject, TProvider>()` for fine-grained control
- **Conventions**: Includes `ThinktectureConventionSetPlugin` for automatic EF Core configuration
- **Extensions**: Provides extension methods on `EntityTypeBuilder`, `PropertyBuilder`, `ComplexTypePropertyBuilder`, `PrimitiveCollectionBuilder`, and `ModelBuilder` for explicit configuration
- **Regular Discriminated Unions**: May require manual discriminator configuration using standard EF Core APIs

#### ASP.NET Core

Package: `Thinktecture.Runtime.Extensions.AspNetCore`

- **Model Binding**: Automatic model binding relies on `IParsable<T>` interface (auto-generated by source generators)
- **Custom Parsing**: Use `[ObjectFactory<string>]` for custom parsing logic from strings in query parameters, route values, etc.
- **Minimal APIs**: Full support for Smart Enums and Value Objects as parameters

#### Swashbuckle/OpenAPI

Package: `Thinktecture.Runtime.Extensions.Swashbuckle`

- Provides schema filters and operation filters for proper OpenAPI documentation of Smart Enums, Value Objects, and Unions
- Ensures generated OpenAPI specs correctly represent the underlying types for client generation

### Pattern Matching

- Use generated `Switch`/`Map` methods for exhaustive, type-safe pattern matching
- Overloads exist to prevent closures for performance-critical scenarios

### Source Generator Architecture

The source generators follow a consistent pattern:

1. **Incremental Generators**: All generators implement `IIncrementalGenerator` for optimal performance
2. **Pipeline Architecture**:
    - Syntax providers filter for attributed types
    - Transform providers extract semantic information into lightweight state objects
    - Code generators produce source code from state
3. **State Objects**: Each generator has corresponding state classes (e.g., `SmartEnumSourceGeneratorState`, `KeyedValueObjectSourceGeneratorState`, `ComplexValueObjectSourceGeneratorState`)
4. **Code Generators**: Separated into specialized generators:
    - **Interface implementations**: `ParsableCodeGenerator`, `ComparableCodeGenerator`, `FormattableCodeGenerator`, `InterfaceCodeGeneratorFactory`
    - **Operators**: `ComparisonOperatorsCodeGenerator`, `EqualityComparisonOperatorsCodeGenerator`, `AdditionOperatorsCodeGenerator`, `SubtractionOperatorsCodeGenerator`, `MultiplyOperatorsCodeGenerator`, `DivisionOperatorsCodeGenerator`
    - **Serialization**: Separate code generator factories for each serialization framework (e.g., `JsonSmartEnumCodeGeneratorFactory`, `MessagePackValueObjectCodeGeneratorFactory`)
5. **Type Information**: Rich type metadata captured in interfaces like `ITypeInformation`, `ITypedMemberState`, `IMemberState`, `IKeyMemberSettings`
6. **Nullability Awareness**: Full support for nullable reference types and value types
7. **Containing Types**: Handles nested types properly with `ContainingTypeState`
8. **Generics Support**: Limited support - regular unions and complex value objects can be generic, but smart enums, keyed value objects, and ad-hoc unions cannot

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
- Analyze whether `Theory` can be used for parameterized tests, before considering separate tests for each case
- Edge cases must be tested as well
- AwesomeAssertions for readable assertions (uses same api as FluentAssertions)
- Verify.Xunit for snapshot testing (useful for generated code verification)
- Comprehensive tests for generated code, serialization, and framework integration
- Follow Arrange-Act-Assert pattern for unit tests
- The tests should have same folder structure as the source code (e.g., with class `Ns1/Ns2/MyClass.cs` -> `Ns1/Ns2/MyCassTests/MyMethod.cs`)
- Create a separate test class/file for every public member (eg. for class `MyClass` with method `MyMethod` create `MyCassTests/MyMethod.cs`)
- If the tests requires a `CSharpCompilation` use base class `test/Thinktecture.Runtime.Extensions.SourceGenerator.Tests/CompilationTestBase.cs`
- When testing attributes, use real attributes, like `SmartEnumAttribute`, `ValueObjectAttribute`, `ComplexValueObjectAttribute`, `UnionAttribute`, `ObjectFactoryAttribute`, etc. Don't use fake attributes.

## Code Style

- Follow `.editorconfig` settings (especially in `src/.editorconfig`)
- **XML documentation required** for all publicly visible types and members (except source generator, analyzer, test, and sample projects)
- Multi-target framework support (net7.0 base, with EF Core version-specific projects)
- Don't use `#region`/`#endregion`

## Common Troubleshooting and Best Practices

### Best Practices

1. **String-based keys/members**: Always explicitly specify equality comparer using `[KeyMemberEqualityComparer]` or `[MemberEqualityComparer]` to avoid culture-sensitive comparisons
2. **Validation**: Prefer `ValidateFactoryArguments` over `ValidateConstructorArguments` for better framework integration and returning `ValidationError`
3. **Null handling**: For nullable key types, consider using `NullInFactoryMethodsYieldsNull` or `EmptyStringInFactoryMethodsYieldsNull` instead of throwing exceptions
4. **Immutability**: All members should be readonly (fields) or have no setter/private init (properties)
5. **Constructors**: Keep constructors private to enforce use of factory methods (`Create`, `TryCreate`)
6. **Smart Enum items**: Must be public static readonly fields - non-static fields or properties won't be recognized as items
7. **Serialization**: Reference integration packages in the same project as your types for automatic code generation, or manually register converter factories
8. **Partial keyword**: Types must be marked `partial` for source generators to work

### Common Issues

1. **"Type must be partial"**: Add `partial` keyword to your class/struct declaration
2. **"String-based value object needs equality comparer"**: Add `[KeyMemberEqualityComparer<MyType, string, StringComparer>]` attribute
3. **"Smart enum has no items"**: Ensure items are public static readonly fields of the enum type
4. **"Custom key member implementation not found"**: When using `SkipKeyMember = true`, ensure you've implemented the key member with the correct name and type
5. **"Members disallowing default values must be required"**: Properties with `IDisallowDefaultValue` constraint must use `required` keyword or be initialized
6. **Serialization not working**: Ensure integration package is referenced, or manually register converters/formatters
7. **EF Core not converting**: Call `.UseThinktectureValueConverters()` on DbContextOptionsBuilder
8. **Switch/Map not exhaustive**: Ensure all enum items or union types are handled; consider using generated overloads for compile-time exhaustiveness checking

## Common Patterns and Use Cases

### Value Objects

- **ISBN**: String-based with validation and normalization
- **Amount**: Decimal-based ensuring always positive and rounded
- **DateRange**: Complex object with start/end dates and validation
- **Range<T>**: Generic complex value object for ranges (e.g., `Range<DateTime>`, `Range<int>`)

### Smart Enums

- **ShippingMethod**: Rich enum with pricing, delivery estimates, and calculation methods
- **ProductGroup**: Enum with custom properties referencing other enums
- **JSON Discriminator**: Using Smart Enum as type discriminator in JSON converters

### Discriminated Unions

- **Result<T>**: Generic union for success/failure without exceptions (`Success(T Value)`, `Failure(string Error)`)
- **PartiallyKnownDate**: Regular union for dates with varying precision (year only, year/month, full date)
- **Jurisdiction**: Complex domain modeling combining unions and value objects

---

## Attribute Configuration Reference

This section lists constructor arguments and configurable properties for key attributes.

Notes:

- Defaults shown reflect runtime defaults from constructors or property getters.
- Some properties only take effect under certain conditions (see descriptions).

### Base Attributes (common configuration)

- ValueObjectAttributeBase (base for ValueObjectAttribute<TKey>, ComplexValueObjectAttribute)
    - Properties:
        - SkipFactoryMethods: bool — do not generate Create/TryCreate/Validate
        - ConstructorAccessModifier: AccessModifier — default Private
        - CreateFactoryMethodName: string — default "Create" (whitespace resets to default)
        - TryCreateFactoryMethodName: string — default "TryCreate" (whitespace resets to default)
        - SkipToString: bool — skip ToString() generation
        - AllowDefaultStructs: bool — allow default(struct) instances (structs only)
        - DefaultInstancePropertyName: string — default "Empty" (structs only)
        - SerializationFrameworks: SerializationFrameworks — default All

- UnionAttributeBase (base for AdHocUnionAttribute and generic UnionAttribute<...>)
    - Properties:
        - DefaultStringComparison: StringComparison — default OrdinalIgnoreCase
        - SkipToString: bool — skip ToString() generation
        - ConstructorAccessModifier: UnionConstructorAccessModifier — default Public
        - ConversionFromValue: ConversionOperatorsGeneration — default Implicit
        - ConversionToValue: ConversionOperatorsGeneration — default Explicit
        - SwitchMethods: SwitchMapMethodsGeneration — configure Switch generation
        - MapMethods: SwitchMapMethodsGeneration — configure Map generation
        - SwitchMapStateParameterName: string? — default "state"
        - UseSingleBackingField: bool — default false

### SmartEnumAttribute (keyless)

- Targets: class
- Constructors:
    - SmartEnumAttribute()
- Properties:
    - EqualityComparisonOperators: OperatorsGeneration — generation for equality operators
    - SwitchMethods: SwitchMapMethodsGeneration — Switch generation
    - MapMethods: SwitchMapMethodsGeneration — Map generation
    - SwitchMapStateParameterName: string? — default "state"

### SmartEnumAttribute<TKey>

- Targets: class; where TKey : notnull
- Constructors:
    - SmartEnumAttribute() — sets defaults
- Properties:
    - KeyMemberType: Type — default typeof(TKey)
    - KeyMemberAccessModifier: AccessModifier — default Public
    - KeyMemberKind: MemberKind — default Property
    - KeyMemberName: string — default "_key" if private field; otherwise "Key"
    - SkipIComparable: bool — skip IComparable<T> when key not comparable
    - SkipIParsable: bool — skip IParsable<T> when key not string/IParsable
    - ComparisonOperators: OperatorsGeneration — comparison operators (depends on EqualityComparisonOperators)
    - EqualityComparisonOperators: OperatorsGeneration — equality operators; coerced to at least ComparisonOperators
    - SkipIFormattable: bool — skip IFormattable when key not IFormattable
    - SkipToString: bool — skip ToString()
    - SwitchMethods: SwitchMapMethodsGeneration
    - MapMethods: SwitchMapMethodsGeneration
    - ConversionToKeyMemberType: ConversionOperatorsGeneration — default Implicit
    - ConversionFromKeyMemberType: ConversionOperatorsGeneration — default Explicit
    - SerializationFrameworks: SerializationFrameworks — default All
    - SwitchMapStateParameterName: string? — default "state"

### ValueObjectAttribute<TKey>

- Targets: class | struct; where TKey : notnull
- Inherits: ValueObjectAttributeBase
- Constructors:
    - ValueObjectAttribute() — sets defaults
- Properties:
    - KeyMemberType: Type — readonly, typeof(TKey)
    - KeyMemberAccessModifier: AccessModifier — default Private
    - KeyMemberKind: MemberKind — default Field
    - KeyMemberName: string — default "_value" if private field; otherwise "Value"
    - SkipKeyMember: bool — do not generate key member; implement manually (use KeyMemberName)
    - NullInFactoryMethodsYieldsNull: bool — Create/TryCreate/Validate return null on null input (class + factories only; implied true if EmptyStringInFactoryMethodsYieldsNull is true)
    - EmptyStringInFactoryMethodsYieldsNull: bool — string-key empty/whitespace yields null (class + factories only)
    - SkipIComparable: bool — skip if key not comparable and no custom comparer
    - SkipIParsable: bool — skip if factories skipped or key not string/IParsable
    - AdditionOperators: OperatorsGeneration — requires key supports these ops
    - SubtractionOperators: OperatorsGeneration — requires key supports these ops
    - MultiplyOperators: OperatorsGeneration — requires key supports these ops
    - DivisionOperators: OperatorsGeneration — requires key supports these ops
    - ComparisonOperators: OperatorsGeneration — depends on EqualityComparisonOperators
    - EqualityComparisonOperators: OperatorsGeneration — coerced to at least ComparisonOperators
    - SkipIFormattable: bool — skip if key not IFormattable
    - ConversionToKeyMemberType: ConversionOperatorsGeneration — default Implicit
    - UnsafeConversionToKeyMemberType: ConversionOperatorsGeneration — default Explicit (may throw)
    - ConversionFromKeyMemberType: ConversionOperatorsGeneration — default Explicit

### ComplexValueObjectAttribute

- Targets: class | struct
- Inherits: ValueObjectAttributeBase
- Properties:
    - DefaultStringComparison: StringComparison — default OrdinalIgnoreCase

### UnionAttribute<T1, T2>, ..., UnionAttribute<T1, T2, T3, T4, T5>

- Targets: class | struct
- Inherits: UnionAttributeBase
- Properties (per generic parameter):
    - T1Name, T2Name, ...: string? — override member name (default: type name)
    - T1IsNullableReferenceType, T2IsNullableReferenceType, ...: bool — mark as nullable reference type (no effect for structs)

### AdHocUnionAttribute (non-generic version of UnionAttribute<T1, T2, ...>)

- Targets: class | struct
- Inherits: UnionAttributeBase
- Constructors:
    - AdHocUnionAttribute(Type t1, Type t2, Type? t3 = null, Type? t4 = null, Type? t5 = null)
- Properties:
    - T1, T2: Type — required member types
    - T3, T4, T5: Type? — optional member types
    - T1Name..T5Name: string? — override member names (default: type name)
    - T1IsNullableReferenceType..T5IsNullableReferenceType: bool — mark as nullable reference type (no effect for structs)

### UnionAttribute (regular inheritance-based unions)

- Targets: class
- Constructors:
    - UnionAttribute() — sets ConversionFromValue = Implicit
- Properties:
    - SwitchMethods: SwitchMapMethodsGeneration
    - MapMethods: SwitchMapMethodsGeneration
    - ConversionFromValue: ConversionOperatorsGeneration — default Implicit
    - SwitchMapStateParameterName: string? — default "state"

### ObjectFactoryAttribute<T>

- Targets: class | struct; AllowMultiple = true
- Constructors:
    - ObjectFactoryAttribute() — configures factory for typeof(T)
- Properties:
    - Type: Type — value type the factory accepts (readonly; typeof(T))
    - UseForSerialization: SerializationFrameworks — frameworks that should use the factory/type
    - UseWithEntityFramework: bool — enable EF Core integration for this factory
    - UseForModelBinding: bool (init-only) — enable ASP.NET Core model binding
    - HasCorrespondingConstructor: bool (init-only) — indicates presence of a single-parameter ctor of type `Type`
- Obsolete alias:
    - ValueObjectFactoryAttribute<T> — use ObjectFactoryAttribute<T> instead
