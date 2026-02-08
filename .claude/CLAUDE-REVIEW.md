# Code Review Guide

This guide provides checklists and best practices for reviewing code changes in Thinktecture.Runtime.Extensions.

## Review Checklist

### General Code Quality

- [ ] Code follows `.editorconfig` settings (indentation, spacing, naming conventions)
- [ ] No `#region`/`#endregion` directives used
- [ ] XML documentation present for all public types and members (except test/sample projects)
- [ ] No compiler warnings introduced
- [ ] Code is clear and maintainable (no overly complex logic)
- [ ] No commented-out code blocks left in
- [ ] Proper error handling where appropriate

### Type Structure

For types with `[SmartEnum]`, `[ValueObject]`, `[ComplexValueObject]`, `[Union]`, or `[AdHocUnion]`:

- [ ] Type is marked `partial`
- [ ] Type is `class` or `struct` (not interface, delegate, enum)
- [ ] Type is not nested inside a generic type
- [ ] For Smart Enums: Can be generic itself
- [ ] For Keyed Value Objects: Can be generic itself
- [ ] For Complex Value Objects: Can be generic itself
- [ ] For Regular Unions: Can be generic itself
- [ ] For Ad-hoc Unions: Cannot be generic
- [ ] No primary constructors used

### Constructor and Member Rules

- [ ] Constructors are `private` (enforces factory method usage)
- [ ] Fields are `readonly`
- [ ] Properties have no setter, or `private init` only
- [ ] Smart enum items are `public static readonly` fields of the enum type
- [ ] No mutable state exposed

### Immutability Verification

Value objects and smart enums must be immutable:

- [ ] All fields are `readonly`
- [ ] Properties have no `set` accessor (use `init` or no setter)
- [ ] Collections exposed as `IReadOnlyCollection<T>` or similar
- [ ] No public methods that mutate state

### String-Based Types

For types with string keys or string members:

- [ ] Explicit `[KeyMemberEqualityComparer<MyType, string, StringComparer>]` attribute present
- [ ] Appropriate comparer specified (usually `StringComparer.Ordinal` or `StringComparer.OrdinalIgnoreCase`)
- [ ] For complex value objects with string members: `[MemberEqualityComparer<MyType, string, StringComparer>]` present

**Common issue**: Forgetting equality comparer leads to culture-sensitive string comparison bugs.

### Validation Implementation

- [ ] `ValidateFactoryArguments` used instead of `ValidateConstructorArguments` (preferred)
- [ ] Validation returns `ValidationError` (not throwing exceptions)
- [ ] `ref` parameters used appropriately for value normalization
- [ ] Validation logic covers all edge cases
- [ ] Null checks for reference type parameters
- [ ] Range checks for numeric parameters

**Example of correct validation**:
```csharp
static partial void ValidateFactoryArguments(ref string value, ref ValidationError? validationError)
{
    if (string.IsNullOrWhiteSpace(value))
    {
        validationError = new ValidationError("Value cannot be empty.");
        return;
    }

    value = value.Trim(); // Normalize
}
```

### Smart Enum Specific

- [ ] At least one item defined (public static readonly field)
- [ ] Items are of the correct enum type
- [ ] Key values are unique
- [ ] Enum is `sealed` if no derived types exist
- [ ] Key member name is valid (not reserved like "Item", "Key" in certain contexts)
- [ ] Derived types (if any) are properly accessible

### Value Object Specific

**Keyed (Simple)**:
- [ ] Key member type is appropriate
- [ ] Custom key member implementation present if `SkipKeyMember = true`
- [ ] Key member name and type match when using custom implementation
- [ ] Arithmetic operators only used with numeric key types
- [ ] Conversion operators configured appropriately

**Complex**:
- [ ] Members correctly participate in equality unless `[IgnoreMember]` applied
- [ ] Custom equality comparers specified for string members
- [ ] No ignored members that should participate in equality
- [ ] Generic type constraints appropriate for member types

### Union Specific

**Ad-hoc Unions**:
- [ ] 2-5 types specified (not less, not more)
- [ ] Generic or non-generic attribute syntax used correctly
- [ ] Member type names unique and descriptive
- [ ] Nullable reference type annotations correct

**Regular (Inheritance-based) Unions**:
- [ ] Base type is `sealed` or has only `private` constructors
- [ ] Records are `sealed`
- [ ] Derived types are not generic
- [ ] All non-abstract derived types are accessible
- [ ] No manual factory methods conflicting with generated ones

### Serialization Integration

- [ ] Integration package referenced if automatic integration desired
- [ ] Manual registration code correct if not using automatic integration
- [ ] Serialization tested with roundtrip tests
- [ ] Null handling correct for nullable types
- [ ] Span-based JSON converter attribute generated correctly with `#if NET9_0_OR_GREATER` blocks (for string-keyed Smart Enums)
- [ ] `DisableSpanBasedJsonConversion` setting respected when set to `true`

### Framework Integration

**Entity Framework Core**:
- [ ] `.UseThinktectureValueConverters()` called on `DbContextOptionsBuilder`
- [ ] Value converters working correctly in queries
- [ ] No serialization issues with complex types
- [ ] Migrations generated correctly

**ASP.NET Core**:
- [ ] Model binding working for route parameters and query strings
- [ ] `IParsable<T>` implementation used correctly
- [ ] Custom `[ObjectFactory<string>]` appropriate if used
- [ ] Validation errors handled properly in controllers

### Parsing and Formatting

- [ ] `IParsable<T>` tests cover valid and invalid inputs
- [ ] `ISpanParsable<T>` tests present if NET9+ and key type supports it
- [ ] Culture-specific tests for decimal and date types
- [ ] `FormatException` thrown with meaningful messages for invalid input
- [ ] `TryParse` returns `false` for invalid input (doesn't throw)

### Arithmetic Operators (Value Objects)

- [ ] Operators only generated when key type supports them
- [ ] Unchecked arithmetic context (overflow wraps around)
- [ ] Tests cover overflow/underflow scenarios (e.g., `int.MaxValue + 1`)
- [ ] No unexpected exceptions on boundary values

### Generated Code Verification

- [ ] Generated files compile without errors
- [ ] Generated code follows expected patterns
- [ ] No duplicate members generated
- [ ] Conditional compilation (`#if NET9_0_OR_GREATER`) used correctly
- [ ] `#if NET9_0_OR_GREATER` blocks present for span-based JSON converter on string-keyed Smart Enums
- [ ] Generated XML documentation present

### Test Coverage

- [ ] Tests added to existing test classes when appropriate
- [ ] New test classes only created when no fitting class exists
- [ ] Tests follow Arrange-Act-Assert pattern
- [ ] Theory used for parameterized tests when appropriate
- [ ] Edge cases tested (null, empty, min/max values)
- [ ] Verify.Xunit used for snapshot testing generated code
- [ ] Real attributes used in tests (not fake attributes)

### Performance Considerations

- [ ] No unnecessary allocations in hot paths
- [ ] Span-based APIs used when available (NET9+)
- [ ] Zero-allocation span-based JSON deserialization used for string-keyed Smart Enums (NET9+)
- [ ] `[ObjectFactory<ReadOnlySpan<char>>]` with `UseForSerialization` used for value objects needing span-based JSON deserialization
- [ ] StringBuilder used for string concatenation in loops
- [ ] No LINQ in performance-critical code (if avoidable)

## Common Issues to Watch For

### 1. Missing Partial Keyword

**Issue**: Type missing `partial` keyword
```csharp
[SmartEnum<int>]
public class TestEnum { }  // ❌ Missing 'partial'
```

**Fix**: Add `partial` keyword
```csharp
[SmartEnum<int>]
public partial class TestEnum { }  // ✅
```

### 2. String Keys Without Equality Comparer

**Issue**: String-based value object without explicit comparer
```csharp
[ValueObject<string>]
public partial class ProductId { }  // ⚠️ Analyzer warning
```

**Fix**: Add explicit comparer
```csharp
[ValueObject<string>]
[KeyMemberEqualityComparer<ProductId, string, StringComparer>(typeof(StringComparer), nameof(StringComparer.Ordinal))]
public partial class ProductId { }  // ✅
```

### 3. Non-Private Constructors

**Issue**: Public or internal constructor
```csharp
[ValueObject<int>]
public partial class Amount
{
    public Amount(int value) { }  // ❌ Should be private
}
```

**Fix**: Make constructor private
```csharp
[ValueObject<int>]
public partial class Amount
{
    private Amount(int value) { }  // ✅
}
```

### 4. Wrong Validation Method

**Issue**: Using `ValidateConstructorArguments` instead of `ValidateFactoryArguments`
```csharp
static partial void ValidateConstructorArguments(string value)
{
    if (string.IsNullOrWhiteSpace(value))
        throw new ArgumentException("Invalid");  // ❌ Throws exception
}
```

**Fix**: Use `ValidateFactoryArguments` with `ValidationError`
```csharp
static partial void ValidateFactoryArguments(ref string value, ref ValidationError? validationError)
{
    if (string.IsNullOrWhiteSpace(value))
    {
        validationError = new ValidationError("Invalid");  // ✅ Returns error
        return;
    }
}
```

### 5. Mutable Members

**Issue**: Mutable field or property
```csharp
[ValueObject<int>]
public partial class Amount
{
    public int Value { get; set; }  // ❌ Has setter
}
```

**Fix**: Make immutable
```csharp
[ValueObject<int>]
public partial class Amount
{
    public int Value { get; init; }  // ✅ Init-only
}
```

### 6. Smart Enum Items Not Static Readonly

**Issue**: Instance field or non-readonly
```csharp
[SmartEnum<int>]
public partial class Status
{
    public readonly Status Active = new(1);  // ❌ Not static
    public static Status Inactive = new(2);  // ❌ Not readonly
}
```

**Fix**: Make public static readonly
```csharp
[SmartEnum<int>]
public partial class Status
{
    public static readonly Status Active = new(1);    // ✅
    public static readonly Status Inactive = new(2);  // ✅
}
```

### 7. Culture-Dependent Parsing/Formatting

**Issue**: Not passing culture to Parse/Format
```csharp
var value = DecimalValue.Parse("3.14", null);  // ⚠️ Uses current culture
```

**Fix**: Always pass explicit culture
```csharp
var value = DecimalValue.Parse("3.14", CultureInfo.InvariantCulture);  // ✅
```

### 8. Generic Ad-hoc Union

**Issue**: Ad-hoc union declared as generic
```csharp
[Union<string, int>]
public partial class MyUnion<T> { }  // ❌ Ad-hoc unions cannot be generic
```

**Fix**: Remove generic parameter or use regular union
```csharp
[Union<string, int>]
public partial class MyUnion { }  // ✅
```

### 9. Missing SkipKeyMember Implementation

**Issue**: `SkipKeyMember = true` but key member not implemented
```csharp
[ValueObject<int>(SkipKeyMember = true)]
public partial class Amount
{
    // ❌ No Value property/field implemented
}
```

**Fix**: Implement key member with correct name
```csharp
[ValueObject<int>(SkipKeyMember = true, KeyMemberName = "Value")]
public partial class Amount
{
    public int Value { get; }  // ✅
}
```

### 10. Incorrect Arithmetic Overflow Expectations

**Issue**: Expecting arithmetic to throw on overflow
```csharp
var max = IntValue.Create(int.MaxValue);
var result = max + IntValue.Create(1);
// Developer expects OverflowException but code wraps around
```

**Fix**: Document that arithmetic uses unchecked context, or add explicit checks
```csharp
// Arithmetic wraps around (unchecked context)
var max = IntValue.Create(int.MaxValue);
var result = max + IntValue.Create(1);
// result.Value == int.MinValue ✅
```

## Best Practices Summary

1. **Always use `ValidateFactoryArguments`** for validation (not `ValidateConstructorArguments`)
2. **Explicitly specify string comparers** for string-based keys/members
3. **Keep constructors private** to enforce factory method usage
4. **Ensure immutability** (readonly fields, no setters)
5. **Test with multiple cultures** for parsing/formatting culture-sensitive types
6. **Use Theory for parameterized tests** when testing multiple similar scenarios
7. **Add tests to existing classes** before creating new test files
8. **Verify generated code** compiles and follows expected patterns
9. **Check serialization roundtrips** work correctly
10. **Test edge cases** (null, empty, min/max values, overflow/underflow)

## Review Process

1. **Read the changes**: Understand what the PR is trying to accomplish
2. **Check type structure**: Verify partial, constructors, members follow rules
3. **Verify validation**: Ensure proper validation method and error handling
4. **Check string handling**: Explicit comparers for string types
5. **Review tests**: Coverage, organization, edge cases
6. **Test locally**: Run tests, verify generated code
7. **Check documentation**: XML docs for public APIs
8. **Verify framework integration**: Serialization, EF Core, ASP.NET Core work correctly
9. **Look for common issues**: Refer to "Common Issues" section above
10. **Approve or request changes**: Provide clear, actionable feedback
