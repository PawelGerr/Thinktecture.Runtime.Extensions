# Investigation & Review Guide

This guide serves two purposes: systematic bug investigation (Part 1) and comprehensive code review (Part 2). Both share the same foundational knowledge of the library's architecture and common failure modes.

---

## Part 1: Bug Investigation

A structured workflow for systematically investigating bugs in the Thinktecture.Runtime.Extensions source generator library.

### 1. Bug Classification

Quickly categorize the bug to focus your investigation:

#### Source Generator Bug

Generated code is incorrect, missing, or does not compile.

- **Symptoms**: Build errors in generated files, unexpected output in `*.verified.txt` snapshots, missing members or interfaces on generated types.
- **Root cause area**: State objects, code generators, code generator factories, conditional compilation.

#### Analyzer Bug

False positives, false negatives, or wrong diagnostic messages.

- **Symptoms**: Diagnostic reported on valid code, diagnostic missing on invalid code, incorrect severity or message text.
- **Root cause area**: `ThinktectureRuntimeExtensionsAnalyzer`, symbol analysis logic, attribute detection.

#### Runtime Bug

Generated code compiles but behaves incorrectly at runtime.

- **Symptoms**: Wrong equality results, factory methods returning unexpected values, conversion operators failing, Switch/Map dispatching to wrong branch.
- **Root cause area**: Generated method bodies, validation logic, metadata emission.

#### Integration Bug

Works standalone but fails with a framework (JSON, EF Core, ASP.NET Core, MessagePack, etc.).

- **Symptoms**: Serialization round-trip failure, EF Core value converter not applied, model binding returning null, wrong JSON converter selected.
- **Root cause area**: `MetadataLookup`, converter/formatter registration, object factory priority, `ThinktectureJsonConverterFactory` selection logic.

#### Configuration Bug

Attribute properties do not produce the expected behavior.

- **Symptoms**: Setting `KeyMemberKind = MemberKind.Property` has no effect, `DisableSpanBasedJsonConversion = true` still generates span-based code, operator configuration ignored.
- **Root cause area**: Attribute data extraction (`AttributeDataExtensions`), settings classes (`AllEnumSettings`, `SmartEnumSettings`), state object construction.

### 2. Reproduction Workflow

Follow these steps to create a reliable reproduction:

#### Step 1: Identify the Minimal Type Declaration

Strip the user's code down to the smallest `partial` type declaration that triggers the issue. Include only the attribute, the type keyword, and the minimal members needed.

#### Step 2: Check Configuration Dimensions

Determine if the bug is specific to certain configurations:

- **struct vs class** (value type vs reference type)
- **keyed vs keyless** (e.g., `[SmartEnum<int>]` vs `[SmartEnum]`)
- **generic vs non-generic** type parameters
- **key type** (string, int, Guid, custom)
- **target framework** (net8.0 vs net9.0 vs net10.0)
- **sealed vs non-sealed**
- **nested vs top-level** type

#### Step 3: Create a Compilation Test Type

Add a minimal reproduction type in the `Tests.Shared` project under the appropriate folder:

- `TestEnums/` for Smart Enum bugs
- `TestValueObjects/` for Value Object bugs
- `TestAdHocUnions/` for Ad-hoc Union bugs
- `TestRegularUnions/` for Regular Union bugs

Name the type descriptively (e.g., `SmartEnum_IntBased_WithComparisonOperators_Bug1234`).

#### Step 4: Verify the Issue

Build the solution (`dotnet build`) and observe:

- **Source generator bug**: Compilation error in generated code, or snapshot mismatch.
- **Analyzer bug**: Diagnostic presence/absence in Error List.
- **Runtime bug**: Write a failing test in the appropriate test project.
- **Integration bug**: Write a failing test in the relevant integration test project.

### 3. Investigation by Bug Type

#### Source Generator Bugs

**Inspect generated code:**

- Check `*.verified.txt` snapshot files for the expected output.
- Use `SourceGeneratorTestsBase.GetGeneratedOutput<T>()` to see the actual output for your reproduction type.
- Look in the `obj/` directory of the test project for generated `.g.cs` files.

**Trace the pipeline:**

```
Attribute on type
  --> SyntaxProvider filters the type
  --> Transform extracts semantic info into state object
  --> Code generator factory selects appropriate generator
  --> Code generator produces output
```

**Key files to check by generator:**

| Generator     | State Object                                                                     | Code Generator(s)                                                  |
|---------------|----------------------------------------------------------------------------------|--------------------------------------------------------------------|
| SmartEnum     | `SmartEnumSourceGeneratorState`                                                  | `SmartEnumCodeGenerator`, `KeyedJsonCodeGenerator`                 |
| ValueObject   | `KeyedValueObjectSourceGeneratorState`, `ComplexValueObjectSourceGeneratorState` | `KeyedValueObjectCodeGenerator`, `ComplexValueObjectCodeGenerator` |
| AdHocUnion    | `AdHocUnionSourceGenState`                                                       | `AdHocUnionCodeGenerator`                                          |
| RegularUnion  | `RegularUnionSourceGenState`                                                     | `RegularUnionCodeGenerator`                                        |
| ObjectFactory | `ObjectFactorySourceGeneratorState`                                              | `ObjectFactoryCodeGenerator`                                       |

**Common causes:**

- Missing condition in state extraction (attribute property not read).
- Incorrect type check (e.g., checking `IsReferenceType` when it should check `IsSealed`).
- Wrong conditional compilation (`#if NET9_0_OR_GREATER` missing or misplaced).
- State object equality not updated, causing incremental generation cache staleness.
- Code generator factory not selecting the right specialized generator.

**Use Serena tools:**

- `find_symbol` to locate state classes and their properties.
- `get_symbols_overview` to see all members of a code generator class.
- `search_for_pattern` to find where a specific attribute property is consumed.

#### Analyzer Bugs

**Check the diagnostic descriptor:**

- Find the `DiagnosticDescriptor` by its ID (e.g., `TTRESG042`).
- Verify the ID, message format, severity, and category are correct.

**Trace the analysis:**

```
RegisterSymbolAction (SymbolKind.NamedType)
  --> Attribute detection on the symbol
  --> Condition checks (is partial? has valid members? etc.)
  --> Diagnostic reporting with location
```

**Key files:**

- `ThinktectureRuntimeExtensionsAnalyzer.cs` and the analysis methods it delegates to.
- Look for `context.ReportDiagnostic(...)` calls that correspond to the diagnostic in question.

**Common causes:**

- Wrong symbol kind check (expecting `MethodKind.Constructor` but receiving `MethodKind.StaticConstructor`).
- Missing null check on symbol or attribute data.
- Incorrect attribute detection (wrong fully-qualified attribute name, not handling generic attributes).
- Condition logic inverted (reporting when it should not, or vice versa).
- Location pointing to the wrong syntax node.

#### Runtime Bugs

**Write a runtime behavior test:**
Add a test in the `Thinktecture.Runtime.Extensions.Tests` project using the reproduction type from `Tests.Shared`.

**Common areas to check:**

- Equality: `Equals`, `GetHashCode`, `==`, `!=` operators.
- Factory methods: `Create`, `TryCreate`, `Validate` return values.
- Conversion operators: implicit/explicit casts between type and key.
- Parsing: `Parse`, `TryParse` with various inputs and format providers.
- Switch/Map: correct dispatch for all items/members.

**Framework-version-specific bugs:**

- Run tests targeting `net8.0`, `net9.0`, and `net10.0` separately.
- Check for `#if` directive issues in generated code that cause different behavior per framework.

#### Integration Bugs

**Test in the specific integration test project:**

- `Thinktecture.Runtime.Extensions.Json.Tests` for System.Text.Json issues.
- `Thinktecture.Runtime.Extensions.Newtonsoft.Tests` for Newtonsoft.Json issues.
- `Thinktecture.Runtime.Extensions.MessagePack.Tests` for MessagePack issues.
- EF Core test projects for Entity Framework issues.
- ASP.NET Core test projects for model binding issues.

**Check MetadataLookup behavior:**

- Does `MetadataLookup.Find(typeof(YourType))` return the expected metadata?
- Does `MetadataLookup.FindMetadataForConversion(...)` select the right conversion?
- Is the object factory priority causing an unexpected override?

**Common causes:**

- Missing converter/formatter registration in the integration package.
- Incorrect metadata emission by the source generator (wrong key type, missing object factory).
- Object factory priority overriding key-based metadata unexpectedly.
- `ThinktectureJsonConverterFactory` selecting the wrong converter (span-based vs regular).
- Missing `IMetadataOwner` implementation on the generated type.

### 4. Inspecting Generated Code

#### Snapshot Tests

The primary way to inspect generated code. Look for `*.verified.txt` files in the source generator test project. These contain the exact generated output for each test scenario.

#### Build Output

Generated files appear in `obj/{Configuration}/{TargetFramework}/generated/` during build. Navigate there to see what the generator actually produced for a real compilation.

#### Generator Test Base

Use `SourceGeneratorTestsBase.GetGeneratedOutput<T>()` to programmatically generate output for arbitrary input:

```csharp
var source = @"
    [SmartEnum<int>]
    public partial class TestEnum
    {
        public static readonly TestEnum Item1 = new(1);
    }
";
var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source);
// Inspect 'output' to see what was generated
```

#### Comparing Expected vs Actual

When a snapshot test fails, the test runner produces a `*.received.txt` file alongside the `*.verified.txt` file. Diff these two files to see exactly what changed.

### 5. Root Cause Analysis

Once you have identified the failing behavior:

1. **Trace back to the source**: Which state field, condition, or code generation path is wrong?
2. **Check related type combinations**: If the bug affects int-keyed enums, does it also affect string-keyed enums? Guid-keyed? Custom-keyed?
3. **Check struct vs class**: Does the bug manifest differently for value types vs reference types?
4. **Check generic types**: Does the bug affect generic type declarations?
5. **Review related tests**: Existing tests may need updating if the expected behavior was wrong. Search for existing snapshot files and runtime tests covering similar configurations.
6. **Check state object equality**: If the bug involves stale generation, verify that `Equals` and `GetHashCode` on the state object include all relevant fields.

### 6. Writing the Fix

#### Regression Test First

**ALWAYS** write a regression test before applying the fix. The test must:

- Fail before the fix is applied.
- Pass after the fix is applied.
- Be placed in the appropriate test project.

For source generator bugs, this is typically a snapshot test (Verify.XunitV3) or a new compilation test type in `Tests.Shared`.

For runtime bugs, this is a `[Fact]` or `[Theory]` test in the relevant test project.

For analyzer bugs, this is a diagnostic presence/absence test.

#### Apply the Fix

- Make the minimal change needed to fix the root cause.
- Verify all existing tests still pass (`dotnet test`).
- If changing state object fields, update `Equals` and `GetHashCode`.
- If changing generated code, update affected `*.verified.txt` snapshots.

#### Policy Compliance

- Follow the **Zero-Hallucination Policy** and **Code Style Policy** from CLAUDE.md (always in context).

### 7. Common Bug Patterns

These are recurring bug patterns to watch for:

| Pattern                            | Description                                                                    | Where to Look                                 |
|------------------------------------|--------------------------------------------------------------------------------|-----------------------------------------------|
| State equality staleness           | State object `Equals`/`GetHashCode` not updated after adding new field         | State object classes                          |
| Missing `#if` block                | NET9+ feature generated without conditional compilation                        | Code generators                               |
| Nullable annotation mismatch       | Generated code has wrong nullability annotation                                | Code generators, type information             |
| Object factory priority            | Object factory overrides key-based metadata unexpectedly                       | `MetadataLookup.FindMetadataForConversion`    |
| Generic type parameter errors      | Generated code does not handle generic type parameters correctly               | State objects, code generators                |
| Missing `partial` check            | Analyzer does not report missing `partial` keyword in all cases                | Analyzer symbol analysis                      |
| Attribute property not extracted   | New attribute property added but not read in `AttributeDataExtensions`         | `AttributeDataExtensions`, settings classes   |
| Wrong `SpecialType` check          | Checking for `SpecialType.System_String` when the key is a custom type         | State construction, code generator conditions |
| Serializer generator not triggered | Serialization package referenced but code generator factory does not detect it | Serialization code generator factories        |
| Nested type handling               | Generated code does not account for containing types in namespace/type path    | `ContainingTypeState`, type information       |

---

## Part 2: Code Review Checklists

Checklists for reviewing code changes to this library's source generators, analyzers, runtime code, and integration packages.

### General Code Quality

- [ ] Code follows `.editorconfig` settings (indentation, spacing, naming conventions)
- [ ] No `#region`/`#endregion` directives used
- [ ] XML documentation present for all public types and members in `src/Thinktecture.Runtime.Extensions` and integration projects (not required in generator, analyzer, test, or sample projects)
- [ ] No compiler warnings introduced
- [ ] No commented-out code blocks left in

### Source Generator Changes

#### State Objects

- [ ] State object implements `IEquatable<T>` with correct `Equals` and `GetHashCode`
- [ ] `GetHashCode` uses `unchecked` block with `(hashCode * 397) ^ ...` pattern
- [ ] **Every property** included in both `Equals` and `GetHashCode` -- missing any property breaks incremental generation caching and causes stale output
- [ ] No `ISymbol` or other Roslyn compilation references stored in state -- only plain data types and `ImmutableArray<T>`
- [ ] New properties initialized from settings or semantic model during transform step

#### Pipeline

- [ ] Syntax provider uses `ForAttributeWithMetadataName` for efficient early filtering
- [ ] Transform step extracts all needed data into the state object (no deferred symbol access)
- [ ] Pipeline combines providers correctly (e.g., `CompilationProvider.Combine` for per-compilation data)
- [ ] No expensive operations in syntax filtering stage

#### Code Generation

- [ ] Generated code uses fully qualified type names (e.g., `global::System.ArgumentException`) to avoid namespace conflicts -- do not generate using directives. Exception: C# language keywords (string, int, bool, etc.) are used as-is without `global::`
- [ ] File-scoped namespaces used in generated code
- [ ] `#if NET9_0_OR_GREATER` conditional compilation for NET9+-only features (span-based APIs, `allows ref struct`)
- [ ] Generated XML documentation present for public members
- [ ] `StringBuilder` used for output construction
- [ ] No duplicate members generated across different code generators for the same type
- [ ] Nullable annotations (`?`) emitted where appropriate

#### Settings and Constants

- [ ] New attribute properties have a corresponding `string const` in `Constants.Attributes.Properties`
- [ ] `FindXxx()` extension method added to `AttributeDataExtensions` for new properties
- [ ] Settings class updated with new property, included in `Equals` and `GetHashCode`
- [ ] Default values match the attribute property defaults

### Analyzer Changes

- [ ] Diagnostic ID follows `TTRESG` prefix convention (IDs < 100 are primarily errors with rare exceptions; >= 100 = warnings/info)
- [ ] Category is `"Thinktecture.Runtime.Extensions"`
- [ ] `context.EnableConcurrentExecution()` called
- [ ] `context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None)` called
- [ ] Diagnostics reported with precise `Location` from syntax nodes (not symbol locations)
- [ ] Error messages are clear and actionable
- [ ] No false positives on valid configurations
- [ ] No false negatives on invalid configurations
- [ ] If code fix provided: fix produces compilable code and is registered for the correct diagnostic ID

### Runtime Library Changes

- [ ] Public API has XML documentation
- [ ] Internal implementation uses `Thinktecture.Internal` namespace
- [ ] Thread safety considered for shared/cached state (e.g., `MetadataLookup` dictionaries)
- [ ] `[MethodImpl(MethodImplOptions.AggressiveInlining)]` on small performance-critical methods
- [ ] No unnecessary allocations in hot paths
- [ ] `CancellationToken` threaded through long-running operations
- [ ] Backward compatible -- existing generated code continues to work without recompilation

#### Metadata System

- [ ] `IMetadataOwner` implementation generated correctly for new type categories
- [ ] `MetadataLookup` cache invalidation not broken
- [ ] Object factory priority respected (factories override key-based metadata)
- [ ] `ConversionMetadata` expressions correct for new conversion paths

### Integration Package Changes

- [ ] Uses `MetadataLookup.FindMetadataForConversion()` with appropriate filters
- [ ] Handles null metadata gracefully (type might not have metadata)
- [ ] Object factory priority respected over key-based metadata
- [ ] Round-trip tests: serialize then deserialize, verify equality
- [ ] Null handling correct for nullable types
- [ ] `ValidationErrorType` checked for validation failures during deserialization
- [ ] Version-specific EF Core packages (8/9/10) updated consistently

#### Span-Based JSON (NET9+)

- [ ] `#if NET9_0_OR_GREATER` blocks present for span-based converter registration
- [ ] `DisableSpanBasedJsonConversion` setting respected when `true`
- [ ] `ThinktectureSpanParsableJsonConverterFactory` used for span-capable types
- [ ] Fallback to regular `ThinktectureJsonConverterFactory` on older frameworks

### Test Changes

- [ ] Tests added to existing test classes when appropriate (not unnecessary new files)
- [ ] Tests follow Arrange-Act-Assert pattern
- [ ] Test naming: `Should_[ExpectedBehavior]_when_[Condition]`
- [ ] `[Theory]` used for parameterized tests when multiple inputs test the same logic
- [ ] Edge cases tested (null, empty, min/max values, invalid input)
- [ ] Verify.XunitV3 used for snapshot testing generated code
- [ ] Real attributes used in tests (not fake/mock attributes)
- [ ] Multi-framework coverage (tests run on net8.0, net9.0, net10.0)

#### Compilation Test Types (`Tests.Shared`)

When reviewing new types in `Tests.Shared`, verify they follow the consumer-facing patterns the library enforces. These types exist solely to verify the source generators produce compilable output:

- [ ] Type is `partial`
- [ ] Constructors are private
- [ ] Smart enum items are `public static readonly` fields
- [ ] String-based types have explicit equality comparer attributes
- [ ] `ValidateFactoryArguments` used (not `ValidateConstructorArguments`)
- [ ] Type named descriptively to indicate what feature/configuration it tests
- [ ] Minimal implementation -- only enough to trigger generation

### Review Process

1. **Identify change category**: Source generator, analyzer, runtime library, integration package, or tests
2. **Apply the relevant checklist** from the sections above
3. **For generator changes**: Verify state object equality is complete (this is the most common source of bugs)
4. **Run tests locally**: `dotnet test` across all target frameworks
5. **Inspect generated output**: Check `obj/` directory for generated files if generator code changed
6. **Check backward compatibility**: Existing generated code must not break
7. **Verify documentation**: XML docs for new public API surface
8. **Approve or request changes**: Provide clear, actionable feedback
