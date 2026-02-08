# Attribute Configuration Reference

This reference lists all constructor arguments and configurable properties for attributes in Thinktecture.Runtime.Extensions.

**Notes**:

- Defaults shown reflect runtime defaults from constructors or property getters
- Some properties only take effect under certain conditions (see descriptions)
- All attributes are in the `Thinktecture` namespace

## Base Attributes (Common Configuration)

### ValueObjectAttributeBase

Base class for `ValueObjectAttribute<TKey>` and `ComplexValueObjectAttribute`.

**Properties**:

| Property                      | Type                      | Default       | Description                                                                                                                                                              |
|-------------------------------|---------------------------|---------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `SkipFactoryMethods`          | `bool`                    | `false`       | Do not generate `Create`, `TryCreate`, `Validate` methods                                                                                                                |
| `ConstructorAccessModifier`   | `AccessModifier`          | `Private`     | Access modifier for generated constructor                                                                                                                                |
| `CreateFactoryMethodName`     | `string`                  | `"Create"`    | Name for the Create factory method (whitespace resets to default)                                                                                                        |
| `TryCreateFactoryMethodName`  | `string`                  | `"TryCreate"` | Name for the TryCreate factory method (whitespace resets to default)                                                                                                     |
| `SkipToString`                | `bool`                    | `false`       | Skip `ToString()` generation                                                                                                                                             |
| `AllowDefaultStructs`         | `bool`                    | `false`       | Allow `default(struct)` instances (structs only)                                                                                                                         |
| `DefaultInstancePropertyName` | `string`                  | `"Empty"`     | Name of default instance property (structs only, when AllowDefaultStructs = true)                                                                                        |
| `SerializationFrameworks`     | `SerializationFrameworks` | `All`         | Which serialization frameworks to generate integration code for                                                                                                          |
| `SkipEqualityComparison`      | `bool`                    | `false`       | Skip generation of equality members (`Equals`, `GetHashCode`, `==`, `!=`, `IEquatable<T>`) - also sets `ComparisonOperators` and `EqualityComparisonOperators` to `None` |

### UnionAttributeBase

Base class for `AdHocUnionAttribute` and generic `UnionAttribute<T1, T2, ...>`.

**Properties**:

| Property                      | Type                             | Default             | Description                                                                           |
|-------------------------------|----------------------------------|---------------------|---------------------------------------------------------------------------------------|
| `DefaultStringComparison`     | `StringComparison`               | `OrdinalIgnoreCase` | Default string comparison for union operations                                        |
| `SkipToString`                | `bool`                           | `false`             | Skip `ToString()` generation                                                          |
| `ConstructorAccessModifier`   | `UnionConstructorAccessModifier` | `Public`            | Access modifier for generated constructor                                             |
| `ConversionFromValue`         | `ConversionOperatorsGeneration`  | `Implicit`          | Generate implicit conversion operators from member types to union                     |
| `ConversionToValue`           | `ConversionOperatorsGeneration`  | `Explicit`          | Generate explicit conversion operators from union to member types                     |
| `SwitchMethods`               | `SwitchMapMethodsGeneration`     | (varies)            | Configure Switch method generation                                                    |
| `MapMethods`                  | `SwitchMapMethodsGeneration`     | (varies)            | Configure Map method generation                                                       |
| `SwitchMapStateParameterName` | `string?`                        | `"state"`           | Name of state parameter in Switch/Map methods                                         |
| `UseSingleBackingField`       | `bool`                           | `false`             | Use single backing field for all union values (reduces memory, increases type checks) |
| `SkipEqualityComparison`      | `bool`                           | `false`             | Skip generation of equality members (ad-hoc unions only)                              |

## Smart Enum Attributes

### SmartEnumAttribute (Keyless)

For type-safe enums without underlying values.

**Targets**: `class`

**Constructor**:

```csharp
SmartEnumAttribute()
```

**Properties**:

| Property                      | Type                         | Default   | Description                                   |
|-------------------------------|------------------------------|-----------|-----------------------------------------------|
| `EqualityComparisonOperators` | `OperatorsGeneration`        | `Default` | Generation of equality operators (`==`, `!=`) |
| `SwitchMethods`               | `SwitchMapMethodsGeneration` | `Default` | Switch method generation configuration        |
| `MapMethods`                  | `SwitchMapMethodsGeneration` | `Default` | Map method generation configuration           |
| `SwitchMapStateParameterName` | `string?`                    | `"state"` | Name of state parameter in Switch/Map methods |

### SmartEnumAttribute&lt;TKey&gt;

For type-safe enums with underlying key values (where `TKey : notnull`).

**Targets**: `class`

**Constructor**:

```csharp
SmartEnumAttribute()
```

**Properties**:

| Property                         | Type                            | Default                                         | Description                                                                                                                    |
|----------------------------------|---------------------------------|-------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------|
| `KeyMemberType`                  | `Type`                          | `typeof(TKey)`                                  | Type of the key member (readonly)                                                                                              |
| `KeyMemberAccessModifier`        | `AccessModifier`                | `Public`                                        | Access modifier for key member                                                                                                 |
| `KeyMemberKind`                  | `MemberKind`                    | `Property`                                      | Whether key member is property or field                                                                                        |
| `KeyMemberName`                  | `string`                        | `"_key"` (private field) or `"Key"` (otherwise) | Name of the key member                                                                                                         |
| `SkipIComparable`                | `bool`                          | `false`                                         | Skip `IComparable<T>` implementation (when key not comparable)                                                                 |
| `SkipIParsable`                  | `bool`                          | `false`                                         | Skip `IParsable<T>` implementation (independent from SkipISpanParsable)                                                        |
| `SkipISpanParsable`              | `bool`                          | `false`                                         | Skip `ISpanParsable<T>` implementation (independent from SkipIParsable; NET9+)                                                 |
| `ComparisonOperators`            | `OperatorsGeneration`           | `Default`                                       | Comparison operators (`<`, `<=`, `>`, `>=`) - depends on EqualityComparisonOperators                                           |
| `EqualityComparisonOperators`    | `OperatorsGeneration`           | `Default`                                       | Equality operators (`==`, `!=`) - coerced to at least ComparisonOperators                                                      |
| `SkipIFormattable`               | `bool`                          | `false`                                         | Skip `IFormattable` implementation (when key not IFormattable)                                                                 |
| `SkipToString`                   | `bool`                          | `false`                                         | Skip `ToString()` generation                                                                                                   |
| `SwitchMethods`                  | `SwitchMapMethodsGeneration`    | `Default`                                       | Switch method generation configuration                                                                                         |
| `MapMethods`                     | `SwitchMapMethodsGeneration`    | `Default`                                       | Map method generation configuration                                                                                            |
| `ConversionToKeyMemberType`      | `ConversionOperatorsGeneration` | `Implicit`                                      | Generate conversion operator from enum to key type                                                                             |
| `ConversionFromKeyMemberType`    | `ConversionOperatorsGeneration` | `Explicit`                                      | Generate conversion operator from key type to enum                                                                             |
| `SerializationFrameworks`        | `SerializationFrameworks`       | `All`                                           | Which serialization frameworks to generate integration code for                                                                |
| `DisableSpanBasedJsonConversion` | `bool`                          | `false`                                         | Disables ReadOnlySpan-based zero-allocation JSON conversion, falling back to string-based conversion (string keys only; NET9+) |
| `SwitchMapStateParameterName`    | `string?`                       | `"state"`                                       | Name of state parameter in Switch/Map methods                                                                                  |

**Note**: `ISpanParsable<T>` inherits from `IParsable<T>`, so setting `SkipISpanParsable = false` will automatically set `SkipIParsable = false` if needed.

## Value Object Attributes

### ValueObjectAttribute&lt;TKey&gt;

For single-value immutable types (where `TKey : notnull`).

**Targets**: `class` or `struct`

**Inherits**: `ValueObjectAttributeBase`

**Constructor**:

```csharp
ValueObjectAttribute()
```

**Properties**:

| Property                                | Type                            | Default                                             | Description                                                                                                                          |
|-----------------------------------------|---------------------------------|-----------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------|
| `KeyMemberType`                         | `Type`                          | `typeof(TKey)`                                      | Type of the key member (readonly)                                                                                                    |
| `KeyMemberAccessModifier`               | `AccessModifier`                | `Private`                                           | Access modifier for key member                                                                                                       |
| `KeyMemberKind`                         | `MemberKind`                    | `Field`                                             | Whether key member is property or field                                                                                              |
| `KeyMemberName`                         | `string`                        | `"_value"` (private field) or `"Value"` (otherwise) | Name of the key member                                                                                                               |
| `SkipKeyMember`                         | `bool`                          | `false`                                             | Do not generate key member; implement manually (use KeyMemberName to specify name)                                                   |
| `NullInFactoryMethodsYieldsNull`        | `bool`                          | `false`                                             | `Create`/`TryCreate`/`Validate` return null on null input (class + factories only)                                                   |
| `EmptyStringInFactoryMethodsYieldsNull` | `bool`                          | `false`                                             | String-key empty/whitespace yields null (class + factories only; implies NullInFactoryMethodsYieldsNull = true)                      |
| `SkipIComparable`                       | `bool`                          | `false`                                             | Skip `IComparable<T>` implementation (if key not comparable and no custom comparer)                                                  |
| `SkipIParsable`                         | `bool`                          | `false`                                             | Skip `IParsable<T>` implementation (if factories skipped or key not string/IParsable; independent from SkipISpanParsable)            |
| `SkipISpanParsable`                     | `bool`                          | `false`                                             | Skip `ISpanParsable<T>` implementation (if factories skipped or key not string/ISpanParsable; independent from SkipIParsable; NET9+) |
| `AdditionOperators`                     | `OperatorsGeneration`           | `None`                                              | Generate addition operators (`+`) - requires key supports these ops                                                                  |
| `SubtractionOperators`                  | `OperatorsGeneration`           | `None`                                              | Generate subtraction operators (`-`) - requires key supports these ops                                                               |
| `MultiplyOperators`                     | `OperatorsGeneration`           | `None`                                              | Generate multiplication operators (`*`) - requires key supports these ops                                                            |
| `DivisionOperators`                     | `OperatorsGeneration`           | `None`                                              | Generate division operators (`/`) - requires key supports these ops                                                                  |
| `ComparisonOperators`                   | `OperatorsGeneration`           | `Default`                                           | Comparison operators (`<`, `<=`, `>`, `>=`) - depends on EqualityComparisonOperators                                                 |
| `EqualityComparisonOperators`           | `OperatorsGeneration`           | `Default`                                           | Equality operators (`==`, `!=`) - coerced to at least ComparisonOperators                                                            |
| `SkipIFormattable`                      | `bool`                          | `false`                                             | Skip `IFormattable` implementation (if key not IFormattable)                                                                         |
| `ConversionToKeyMemberType`             | `ConversionOperatorsGeneration` | `Implicit`                                          | Generate implicit conversion operator from value object to key type                                                                  |
| `UnsafeConversionToKeyMemberType`       | `ConversionOperatorsGeneration` | `Explicit`                                          | Generate explicit conversion operator from value object to key type (may throw if validation fails)                                  |
| `ConversionFromKeyMemberType`           | `ConversionOperatorsGeneration` | `Explicit`                                          | Generate explicit conversion operator from key type to value object                                                                  |

**Note**: `ISpanParsable<T>` inherits from `IParsable<T>`, so setting `SkipISpanParsable = false` will automatically set `SkipIParsable = false` if needed.

**Inherits all properties from `ValueObjectAttributeBase`** (see above).

### ComplexValueObjectAttribute

For multi-property immutable types.

**Targets**: `class` or `struct`

**Inherits**: `ValueObjectAttributeBase`

**Properties**:

| Property                  | Type               | Default             | Description                                                            |
|---------------------------|--------------------|---------------------|------------------------------------------------------------------------|
| `DefaultStringComparison` | `StringComparison` | `OrdinalIgnoreCase` | Default string comparison for string members without explicit comparer |

**Inherits all properties from `ValueObjectAttributeBase`** (see above).

## Union Attributes

### UnionAttribute&lt;T1, T2&gt; through UnionAttribute&lt;T1, T2, T3, T4, T5&gt;

Ad-hoc unions for 2-5 types (generic syntax).

**Targets**: `class` or `struct`

**Inherits**: `UnionAttributeBase`

**Properties** (per generic parameter):

| Property                                                      | Type      | Default   | Description                                                                                                                                                                                                                                                                                                                   |
|---------------------------------------------------------------|-----------|-----------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `T1Name`, `T2Name`, ...                                       | `string?` | Type name | Override member name for T1, T2, etc.                                                                                                                                                                                                                                                                                         |
| `T1IsNullableReferenceType`, `T2IsNullableReferenceType`, ... | `bool`    | `false`   | Mark T1, T2, etc. as nullable reference type (no effect for value types). Automatically set to `true` for reference types when `TXIsStateless = true`                                                                                                                                                                         |
| `T1IsStateless`, `T2IsStateless`, ...                         | `bool`    | `false`   | Mark T1, T2, etc. as a stateless type that carries no instance data. Reduces memory by storing only discriminator index. Accessors return `default(T)`. Automatically sets `TXIsNullableReferenceType = true` for reference types. **Recommended: Use struct types for stateless members to avoid null-handling complexity.** |

**Inherits all properties from `UnionAttributeBase`** (see above).

### AdHocUnionAttribute

Ad-hoc unions for 2-5 types (non-generic syntax, alternative to `UnionAttribute<T1, T2, ...>`).

**Targets**: `class` or `struct`

**Inherits**: `UnionAttributeBase`

**Constructor**:

```csharp
AdHocUnionAttribute(Type t1, Type t2, Type? t3 = null, Type? t4 = null, Type? t5 = null)
```

**Properties**:

| Property                                                                                   | Type      | Default    | Description                                                                                                                                                                                                                                                                                                                   |
|--------------------------------------------------------------------------------------------|-----------|------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `T1`, `T2`                                                                                 | `Type`    | (required) | Required member types                                                                                                                                                                                                                                                                                                         |
| `T3`, `T4`, `T5`                                                                           | `Type?`   | `null`     | Optional member types                                                                                                                                                                                                                                                                                                         |
| `T1Name`, `T2Name`, ..., `T5Name`                                                          | `string?` | Type name  | Override member name for T1, T2, etc.                                                                                                                                                                                                                                                                                         |
| `T1IsNullableReferenceType`, `T2IsNullableReferenceType`, ..., `T5IsNullableReferenceType` | `bool`    | `false`    | Mark T1, T2, etc. as nullable reference type (no effect for value types). Automatically set to `true` for reference types when `TXIsStateless = true`                                                                                                                                                                         |
| `T1IsStateless`, `T2IsStateless`, ..., `T5IsStateless`                                     | `bool`    | `false`    | Mark T1, T2, etc. as a stateless type that carries no instance data. Reduces memory by storing only discriminator index. Accessors return `default(T)`. Automatically sets `TXIsNullableReferenceType = true` for reference types. **Recommended: Use struct types for stateless members to avoid null-handling complexity.** |

**Inherits all properties from `UnionAttributeBase`** (see above).

### UnionAttribute (Regular, Inheritance-Based)

For inheritance-based unions with derived types.

**Targets**: `class`

**Constructor**:

```csharp
UnionAttribute()
```

**Properties**:

| Property                      | Type                                 | Default    | Description                                                                                |
|-------------------------------|--------------------------------------|------------|--------------------------------------------------------------------------------------------|
| `SwitchMethods`               | `SwitchMapMethodsGeneration`         | `Default`  | Switch method generation configuration                                                     |
| `MapMethods`                  | `SwitchMapMethodsGeneration`         | `Default`  | Map method generation configuration                                                        |
| `ConversionFromValue`         | `ConversionOperatorsGeneration`      | `Implicit` | Generate implicit conversion operators from derived types to union                         |
| `SwitchMapStateParameterName` | `string?`                            | `"state"`  | Name of state parameter in Switch/Map methods                                              |
| `NestedUnionParameterNames`   | `NestedUnionParameterNameGeneration` | `Default`  | Controls how parameter names are generated for nested Regular Unions in Switch/Map methods |

**NestedUnionParameterNames Values**:

- **`Default` (0)**: Includes intermediate type names in parameter names (e.g., `failureNotFound` for `Failure.NotFound`). Ensures uniqueness but results in longer names. **This is backward compatible and the current behavior.**
- **`Simple` (1)**: Uses only the final type name (e.g., `notFound` for `Failure.NotFound`). Shorter and more readable, but **can cause compilation errors if multiple nested unions have types with the same name**.

**Example**:

```csharp
// Default mode (backward compatible)
[Union]
public partial class ApiResponse
{
   public sealed class Success : ApiResponse;

   public abstract partial class Failure : ApiResponse
   {
      private Failure() { }
      public sealed class NotFound : Failure;
   }
}
// Generated parameter names: success, failureNotFound

// Simple mode (shorter names)
[Union(NestedUnionParameterNames = NestedUnionParameterNameGeneration.Simple)]
public partial class ApiResponse
{
   public sealed class Success : ApiResponse;

   public abstract partial class Failure : ApiResponse
   {
      private Failure() { }
      public sealed class NotFound : Failure;
   }
}
// Generated parameter names: success, notFound
```

**⚠️ Warning**: Using `Simple` mode can cause compilation errors if multiple nested unions contain types with the same name. The C# compiler will report duplicate parameter errors. In such cases, use `Default` mode or rename the conflicting types.

## Object Factory Attribute

### ObjectFactoryAttribute&lt;T&gt;

For types with custom factories for parsing/serialization. When `T` is `ReadOnlySpan<char>` and `UseForSerialization = SerializationFrameworks.SystemTextJson`, enables zero-allocation JSON deserialization on NET9+ by transcoding UTF-8 JSON bytes directly to `ReadOnlySpan<char>` instead of allocating a `string`.

**Targets**: `class` or `struct`

**AllowMultiple**: `true` (can apply multiple times with different types)

**Constructor**:

```csharp
ObjectFactoryAttribute()
```

**Properties**:

| Property                      | Type                      | Default     | Description                                                         |
|-------------------------------|---------------------------|-------------|---------------------------------------------------------------------|
| `Type`                        | `Type`                    | `typeof(T)` | Value type the factory accepts (readonly)                           |
| `UseForSerialization`         | `SerializationFrameworks` | `All`       | Which serialization frameworks should use this factory/type         |
| `UseWithEntityFramework`      | `bool`                    | `false`     | Enable EF Core integration for this factory                         |
| `UseForModelBinding`          | `bool` (init-only)        | `false`     | Enable ASP.NET Core model binding                                   |
| `HasCorrespondingConstructor` | `bool` (init-only)        | `false`     | Indicates presence of a single-parameter constructor of type `Type` |

## Additional Attributes

### KeyMemberEqualityComparerAttribute&lt;TType, TMember, TEqualityComparer&gt;

Specify custom equality comparer for key member.

**Usage**:

```csharp
[ValueObject<string>]
[KeyMemberEqualityComparer<ProductId, string, StringComparer>(typeof(StringComparer), nameof(StringComparer.Ordinal))]
public partial class ProductId { }
```

### KeyMemberComparerAttribute&lt;TType, TMember, TComparer&gt;

Specify custom comparer for key member (for ordering).

### MemberEqualityComparerAttribute&lt;TType, TMember, TEqualityComparer&gt;

Specify custom equality comparer for a specific member in complex value objects.

**Usage**:

```csharp
[ComplexValueObject]
public partial class Person
{
    [MemberEqualityComparer<Person, string, StringComparer>(typeof(StringComparer), nameof(StringComparer.OrdinalIgnoreCase))]
    public string Name { get; }
}
```

### IgnoreMemberAttribute

Exclude member from equality/comparison in complex value objects.

**Usage**:

```csharp
[ComplexValueObject]
public partial class DateRange
{
    public DateTime Start { get; }
    public DateTime End { get; }

    [IgnoreMember]
    public int DurationInDays => (End - Start).Days;  // Not part of equality
}
```

### ValidationErrorAttribute

Mark a type as a validation error (implements `IValidationError`).

### UseDelegateFromConstructorAttribute

Inject delegate parameters into constructor from partial methods.

**Usage**:

```csharp
[SmartEnum<int>]
public partial class Status
{
    [UseDelegateFromConstructor]
    private static partial Func<Status, string> GetDisplayText();

    // Constructor will receive Func<Status, string> parameter
}
```

### UnionSwitchMapOverloadAttribute

Customize generated Switch/Map overloads for regular unions.

## Enums

### AccessModifier

- `Public`
- `Internal`
- `Private`
- `Protected`
- `ProtectedInternal`
- `PrivateProtected`

### MemberKind

- `Property`
- `Field`

### OperatorsGeneration

- `Default`: Generate default set of operators
- `DefaultWithKeyTypeOverloads`: Generate default operators + overloads taking key type
- `None`: Skip operator generation

### ConversionOperatorsGeneration

- `Implicit`: Generate implicit conversion operator
- `Explicit`: Generate explicit conversion operator
- `None`: Skip conversion operator generation

### SwitchMapMethodsGeneration

- `Default`: Generate default Switch/Map methods
- `DefaultWithPartialNameMatching`: Generate default + partial name matching overloads
- `None`: Skip Switch/Map generation

### SerializationFrameworks

Flags enum:

- `None`
- `SystemTextJson`
- `NewtonsoftJson`
- `MessagePack`
- `All` (default)

### UnionConstructorAccessModifier

- `Public`
- `Internal`
- `Private`

### NestedUnionParameterNameGeneration

Controls how parameter names are generated in Switch/Map methods for nested Regular Union types.

- `Default` (0): Includes intermediate type names in parameter names (e.g., `failureNotFound` for `Failure.NotFound`). Ensures uniqueness, backward compatible.
- `Simple` (1): Uses only the final type name (e.g., `notFound` for `Failure.NotFound`). Shorter names but can cause conflicts if multiple nested unions have same-named types.

## ISpanParsable Implementation Details (NET9+)

When a key type implements `ISpanParsable<TKey>`, the smart enum or value object automatically implements `ISpanParsable<T>` for zero-allocation parsing.

### Key Features

- **Zero-allocation parsing**: No string allocation when parsing from `ReadOnlySpan<char>`
- **Automatic generation**: Enabled when key type implements `ISpanParsable<TKey>`
- **Culture support**: Full `IFormatProvider` parameter threading
- **Custom types**: Works with custom structs/classes implementing `ISpanParsable<T>`
- **Object factory override**: Can be overridden with `[ObjectFactory<ReadOnlySpan<char>>]`

### Generated Methods

```csharp
#if NET9_0_OR_GREATER
public static MyEnum Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
{
    // Parses key using StaticAbstractInvoker.ParseValue<TKey>(s, provider)
    // Validates key exists in enum
    // Returns enum instance or throws FormatException
}

public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out MyEnum? result)
{
    // Same logic but returns false instead of throwing
}
#endif
```

### Supported Key Types

- Numeric: `int`, `long`, `decimal`, `byte`, `short`, `uint`, `ulong`, `ushort`, `double`, `float`, `sbyte`
- Date/Time: `DateTime`, `DateTimeOffset`, `TimeSpan`
- Other: `Guid`, `Version`, `string`
- Custom types implementing `ISpanParsable<T>`

### Known Limitations

1. **String-keyed value objects**: Missing optimized `Validate(ReadOnlySpan<char>, ...)` overload
    - Impact: String-keyed value objects allocate string during ISpanParsable parsing
    - Smart enums have the optimized overload

2. **Error messages for generic types**: Show base type name only, not full generic signature
    - Example: `"Unable to parse MyType"` instead of `"Unable to parse MyType<T>"`
