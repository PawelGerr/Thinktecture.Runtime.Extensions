# Testing Guide

This guide provides comprehensive testing strategy and organization rules for Thinktecture.Runtime.Extensions.

## Testing Frameworks

### xUnit

Primary testing framework for all unit and integration tests.

**Key attributes**:

- `[Fact]`: Single test case
- `[Theory]`: Parameterized test with `[InlineData]` or `[MemberData]`
- `[Collection]`: Group tests that share fixtures

### AwesomeAssertions

Fluent assertion library (uses same API as FluentAssertions).

**Common assertions**:

```csharp
result.Should().Be(expected);
result.Should().NotBeNull();
collection.Should().HaveCount(3);
collection.Should().Contain(item);
action.Should().Throw<ArgumentException>().WithMessage("*invalid*");
```

### Verify.XunitV3

Snapshot testing framework for verifying complex outputs (especially generated code).

**Usage**:

```csharp
[Fact]
public Task Should_generate_expected_code()
{
    var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source);
    return Verify(output);
}
```

**Verified files**:

- Created as `*.verified.txt` files next to test files
- Committed to source control
- Automatically compared on test runs

## Testing Strategy

### Test Pattern: Arrange-Act-Assert

All tests should follow the AAA pattern:

```csharp
[Fact]
public void Should_return_correct_value_when_valid_input()
{
    // Arrange
    var input = "test";
    var expected = "TEST";

    // Act
    var result = input.ToUpper();

    // Assert
    result.Should().Be(expected);
}
```

### Theory vs. Individual Tests

**Analyze before creating separate tests**. Use `[Theory]` when:

- Multiple inputs produce different outputs using same logic
- Testing boundary conditions (null, empty, max values)
- Testing combinations of parameters

```csharp
[Theory]
[InlineData(1, 2, 3)]
[InlineData(0, 0, 0)]
[InlineData(-1, -2, -3)]
public void Should_add_numbers_correctly(int a, int b, int expected)
{
    var result = a + b;
    result.Should().Be(expected);
}
```

**Use separate `[Fact]` tests when**:

- Tests have significantly different arrange or assert logic
- Test names need to be descriptive of specific scenarios
- Each test case requires different setup or teardown

### Edge Cases

**Always test edge cases**:

- Null values (for reference types)
- Empty strings/collections
- Boundary values (`int.MaxValue`, `int.MinValue`, `DateTime.MinValue`, etc.)
- Invalid inputs (wrong format, out of range)
- Culture-specific scenarios (for parsing/formatting)

## Test Project Organization

### Overview

The test suite is organized into multiple projects, each with a specific purpose:

1. **`Thinktecture.Runtime.Extensions.SourceGenerator.Tests`**: Tests for source generators and analyzers themselves
2. **`Thinktecture.Runtime.Extensions.Tests.Shared`**: Compilation smoke tests
3. **`Thinktecture.Runtime.Extensions.Tests`**: Core runtime functionality tests
4. **Framework integration test projects**: Tests for specific framework integrations (JSON, MessagePack, Newtonsoft.Json, EF Core 8/9/10, ASP.NET Core, Swashbuckle including 3.0 and 3.1 variants)

### Thinktecture.Runtime.Extensions.Tests.Shared

**Purpose**: Compilation smoke testing - simple type declarations that verify source generators produce compilable code.

**CRITICAL FOR NEW FEATURES**: When adding a new feature, **always create new test types in this project** that exercise the new feature configuration.

**Key characteristics**:

- These are **partial type declarations** with attributes
- Minimal implementation (often just the attribute decoration)
- If the project builds successfully, the source generator produces valid C# code
- These types virtually guarantee compilation correctness
- Other test projects reference and test these types

**Organization**:

- **`TestEnums/`**: Smart enum test types (e.g., `SmartEnum_ClassBased.cs`, `SmartEnum_CaseSensitive.cs`)
- **`TestValueObjects/`**: Value object test types (e.g., `IntBasedStructValueObject.cs`, `StringBasedReferenceValueObject.cs`)
- **`TestAdHocUnions/`**: Ad-hoc union test types
- **`TestRegularUnions/`**: Regular union test types

**Pattern**:

```csharp
[ValueObject<int>(KeyMemberKind = MemberKind.Property,
                  KeyMemberName = "Property",
                  KeyMemberAccessModifier = AccessModifier.Public)]
public partial struct IntBasedStructValueObject;
```

**Notable test types for span-based JSON deserialization**:

- `TestEnums/SmartEnum_StringBased_WithDisabledSpanBasedJsonConversion.cs`: Compilation test verifying that `DisableSpanBasedJsonConversion = true` on a string-based Smart Enum produces compilable generated code that falls back to string-based JSON deserialization
- `TestValueObjects/ComplexValueObjectWithReadOnlySpanBasedObjectFactoryForJson.cs`: Compilation test verifying that a complex value object with `[ObjectFactory<ReadOnlySpan<char>>]` produces compilable generated code supporting span-based JSON deserialization

**Best practices**:

- Create separate types for each meaningful feature variation or configuration
- Name types descriptively to indicate what feature/configuration they test
- Keep implementations minimal - focus on compilation verification

## Test Organization

### Folder Structure

Tests should mirror source code structure:

```
Source: src/Thinktecture.Runtime.Extensions/Ns1/Ns2/MyClass.cs
Tests:  test/Thinktecture.Runtime.Extensions.Tests/Ns1/Ns2/MyClassTests/
```

### File Organization Rules

**1. Check for Existing Test Classes FIRST**

Before creating a new test file, search for existing test classes that test:

- The same class/method
- Related functionality
- The same feature area

**2. Add to Existing Test Classes When Appropriate**

Add new tests to existing test classes when:

- Testing the same method with different scenarios
- Testing closely related functionality
- The test logically belongs with existing tests

**3. Create New Test Classes Only When Necessary**

Create a new test class/file when:

- No fitting existing test class exists
- Testing a different public member of a class
- Testing a completely separate feature

### Ideal Structure

```
test/Project.Tests/
  MyClassTests/
    MyMethod.cs              // Tests for MyClass.MyMethod
    MyOtherMethod.cs         // Tests for MyClass.MyOtherMethod
    Constructor.cs           // Tests for MyClass constructor
```

**Consolidate related test cases** in the same file rather than creating many tiny files.

## Testing Generated Code

### CompilationTestBase

Use `SourceGeneratorTestsBase` for testing source generators:

```csharp
public class MyGeneratorTests : SourceGeneratorTestsBase
{
    [Fact]
    public Task Should_generate_smart_enum()
    {
        var source = @"
            namespace Thinktecture.Tests
            {
                [SmartEnum<int>]
                public partial class TestEnum
                {
                    public static readonly TestEnum Item1 = new(1);
                }
            }
        ";

        var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source);

        return Verify(output);
    }
}
```

### Compilation Creation

`CompilationTestBase` provides:

- **`GetGeneratedOutput<T>`** (in `SourceGeneratorTestsBase`): Compiles source, runs generator, returns output (synchronous, not async)
- **`CreateCompilation`**: Creates `CSharpCompilation` with references
- Access to common references (System.Runtime, System.Linq, etc.)

### Testing Attributes

**Use real attributes** from the library:

- `SmartEnumAttribute<T>`
- `ValueObjectAttribute<T>`
- `ComplexValueObjectAttribute`
- `UnionAttribute<T1, T2>`
- `AdHocUnionAttribute`
- `ObjectFactoryAttribute<T>`

**Don't create fake attributes** in test code.

## Testing Analyzers

### Diagnostic Tests

Test that analyzers produce expected diagnostics:

```csharp
[Fact]
public async Task Should_report_diagnostic_when_type_not_partial()
{
    var source = @"
        [SmartEnum<int>]
        public class TestEnum { }  // Missing 'partial'
    ";

    var compilation = await CreateCompilationAsync(source);
    var diagnostics = await GetDiagnosticsAsync(compilation, new MyAnalyzer());

    diagnostics.Should().ContainSingle()
        .Which.Id.Should().Be("TTRESG001");
}
```

### Code Fix Tests

Test that code fix providers produce correct fixes:

```csharp
[Fact]
public async Task Should_add_partial_keyword()
{
    var source = @"
        [SmartEnum<int>]
        public class TestEnum { }
    ";

    var expected = @"
        [SmartEnum<int>]
        public partial class TestEnum { }
    ";

    await VerifyCodeFixAsync(source, expected, new MyCodeFixProvider());
}
```

## Testing Serialization

### JSON Serialization (System.Text.Json)

```csharp
[Fact]
public void Should_serialize_and_deserialize_smart_enum()
{
    // Arrange
    var options = new JsonSerializerOptions();
    options.Converters.Add(new ThinktectureJsonConverterFactory());
    var value = TestEnum.Item1;

    // Act
    var json = JsonSerializer.Serialize(value, options);
    var deserialized = JsonSerializer.Deserialize<TestEnum>(json, options);

    // Assert
    deserialized.Should().Be(value);
}
```

#### Zero-Allocation Span-Based JSON Deserialization

String-based Smart Enums and Value Objects support zero-allocation JSON deserialization via `ThinktectureSpanParsableJsonConverter`. This converter avoids allocating a `string` during deserialization by reading the UTF-8 JSON payload directly into a `ReadOnlySpan<char>` and calling `ISpanParsable<T>.Parse`.

**Key test areas**:

- **`ThinktectureSpanParsableJsonConverter` tests** (`SpanParsableJsonConverterTests.cs`): Verify that types implementing `ISpanParsable<T>` are deserialized through the span-based path. Test both `ThinktectureSpanParsableJsonConverterFactory` (standalone) and the integration through `ThinktectureJsonConverterFactory`.
- **`Utf8JsonReaderHelper` tests** (`Utf8JsonReaderHelperTests.cs`): Verify correct UTF-8 byte to `ReadOnlySpan<char>` conversion, including multi-byte characters, empty strings, and edge cases. This helper is the core of the zero-allocation path.
- **Opt-out via `DisableSpanBasedJsonConversion`**: Smart Enums can disable span-based JSON conversion by setting `DisableSpanBasedJsonConversion = true` on the `[SmartEnum<T>]` attribute. Test that the generated JSON converter falls back to string-based deserialization. The compilation test type `SmartEnum_StringBased_WithDisabledSpanBasedJsonConversion` verifies the generated code compiles correctly.
- **Value objects with `[ObjectFactory<ReadOnlySpan<char>>]`**: Value objects that declare a `ReadOnlySpan<char>`-based object factory also get span-based JSON deserialization. The compilation test type `ComplexValueObjectWithReadOnlySpanBasedObjectFactoryForJson` verifies this path. Snapshot tests in `JsonObjectFactoryCodeGeneratorFactoryTests` verify the generated converter code.
- **Source generator snapshot tests**: `Generate.Snapshot_StringKeyWithSpanJsonConverter.verified.txt` verifies the generated JSON converter includes the span-based deserialization code path.

**Benchmarks**: `SmartEnumJsonDeserializationBenchmarks` and `ValueObjectJsonDeserializationBenchmarks` (in the `Benchmarking` sample project) measure the allocation reduction. These are not unit tests but are useful for validating performance characteristics.

### MessagePack Serialization

```csharp
[Fact]
public void Should_serialize_and_deserialize_with_messagepack()
{
    // Arrange
    var options = MessagePackSerializerOptions.Standard
        .WithResolver(CompositeResolver.Create(
            ThinktectureMessageFormatterResolver.Instance,
            StandardResolver.Instance));

    var value = TestEnum.Item1;

    // Act
    var bytes = MessagePackSerializer.Serialize(value, options);
    var deserialized = MessagePackSerializer.Deserialize<TestEnum>(bytes, options);

    // Assert
    deserialized.Should().Be(value);
}
```

## Testing Framework Integration

### Entity Framework Core

Test value converters and configurations:

```csharp
[Fact]
public async Task Should_convert_value_object_in_ef_core()
{
    // Arrange
    var options = new DbContextOptionsBuilder<TestDbContext>()
        .UseInMemoryDatabase("TestDb")
        .UseThinktectureValueConverters()
        .Options;

    using var context = new TestDbContext(options);

    var entity = new TestEntity { Id = ProductId.Create("PROD-001") };
    context.TestEntities.Add(entity);
    await context.SaveChangesAsync();

    // Act
    var loaded = await context.TestEntities.FirstAsync();

    // Assert
    loaded.Id.Should().Be(entity.Id);
}
```

### ASP.NET Core Model Binding

Test that model binding works correctly:

```csharp
[Fact]
public async Task Should_bind_value_object_from_route()
{
    // Arrange
    var client = _factory.CreateClient();

    // Act
    var response = await client.GetAsync("/api/products/PROD-001");

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
}
```

## Testing Parsing and Formatting

### IParsable Tests

```csharp
[Theory]
[InlineData("1", 1)]
[InlineData("42", 42)]
public void Should_parse_from_string(string input, int expectedKey)
{
    // Act
    var result = TestEnum.Parse(input, null);

    // Assert
    result.Key.Should().Be(expectedKey);
}

[Theory]
[InlineData("invalid")]
[InlineData("")]
[InlineData(null)]
public void Should_throw_when_parsing_invalid_string(string input)
{
    // Act
    Action act = () => TestEnum.Parse(input, null);

    // Assert
    act.Should().Throw<FormatException>();
}
```

### ISpanParsable Tests (NET9+)

```csharp
#if NET9_0_OR_GREATER
[Theory]
[InlineData("1", 1)]
[InlineData("42", 42)]
public void Should_parse_from_span(string input, int expectedKey)
{
    // Act
    var result = TestEnum.Parse(input.AsSpan(), null);

    // Assert
    result.Key.Should().Be(expectedKey);
}

[Fact]
public void Should_try_parse_from_span()
{
    // Arrange
    var input = "42".AsSpan();

    // Act
    var success = TestEnum.TryParse(input, null, out var result);

    // Assert
    success.Should().BeTrue();
    result.Key.Should().Be(42);
}
#endif
```

### Culture-Specific Tests

Test with multiple cultures for decimal and date types:

```csharp
[Theory]
[InlineData("en-US", "3.14")]
[InlineData("de-DE", "3,14")]
[InlineData("fr-FR", "3,14")]
public void Should_parse_decimal_with_culture(string cultureName, string input)
{
    // Arrange
    var culture = CultureInfo.GetCultureInfo(cultureName);

    // Act
    var result = DecimalValue.Parse(input, culture);

    // Assert
    result.Value.Should().BeApproximately(3.14m, 0.01m);
}
```

## Testing Arithmetic Operators

### Overflow/Underflow Tests

Arithmetic operators use **unchecked context** (wrap around on overflow):

```csharp
[Fact]
public void Should_wrap_around_on_overflow()
{
    // Arrange
    var max = IntValue.Create(int.MaxValue);
    var one = IntValue.Create(1);

    // Act
    var result = max + one;

    // Assert
    result.Value.Should().Be(int.MinValue); // Wrapped around
}

[Fact]
public void Should_wrap_around_on_underflow()
{
    // Arrange
    var min = IntValue.Create(int.MinValue);
    var one = IntValue.Create(1);

    // Act
    var result = min - one;

    // Assert
    result.Value.Should().Be(int.MaxValue); // Wrapped around
}
```

## Test Naming Conventions

Use descriptive names following the pattern:

```
Should_[ExpectedBehavior]_when_[Condition]
Should_[ExpectedBehavior]
```

Examples:

- `Should_return_null_when_input_is_null`
- `Should_throw_ArgumentException_when_key_is_invalid`
- `Should_generate_IParsable_implementation`
- `Should_serialize_to_json_correctly`

## Common Testing Pitfalls

1. **Not testing null inputs**: Always test null for reference types
2. **Forgetting edge cases**: Test boundary values (min, max, empty)
3. **Culture-dependent tests**: Use `CultureInfo.InvariantCulture` or test with multiple cultures
4. **Not testing async methods**: Use `async Task` for tests with async code
5. **Overly broad assertions**: Assert specific values, not just "not null"
6. **Testing implementation details**: Test public API behavior, not internal implementation
7. **Flaky tests**: Avoid tests that depend on timing, external resources, or random values

## Continuous Integration

Tests are run automatically on:

- Every commit to any branch
- Pull requests
- Release builds

**All tests must pass** before merging to main branch.
