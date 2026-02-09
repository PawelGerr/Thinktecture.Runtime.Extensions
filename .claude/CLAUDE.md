# Thinktecture.Runtime.Extensions - AI Assistant Guide

This file is organized in tiers:

- **Tier 1** (this file): Architecture essentials and mandatory policies. Always in context -- no need to load anything else for these.
- **Tier 2** (guides): Task-specific workflows. Load the one guide matching your current task.
- **Tier 3** (reference): Detailed lookup tables. Load only when you need exact specifications.

## Mandatory Pre-Flight Protocol

**Execute these checks BEFORE any codebase exploration or tool calls.** This is not advisory -- these are decision gates.

**Step 1 -- Task routing.** Classify the user's intent against the Tier 2 routing table below. Load the matched guide. You MUST NOT make exploratory tool calls (find_symbol, search_for_pattern, get_symbols_overview) until the guide is loaded. Codebase exploration before guide loading is a protocol violation.

**Step 2 -- Delegation check.** If the user's request involves 3 or more of: design, implementation, testing, review, documentation -- you MUST delegate to subagents via the Task tool. Do not produce an inline plan. Creating a comprehensive inline plan is NOT delegation.

> **WRONG**: User says "I want to add feature X." Agent immediately calls find_symbol to explore.
> **RIGHT**: Agent loads `guides/DESIGN-DISCUSSION.md` first, then explores the codebase following the guide's workflow.

> **WRONG**: User asks for feature + tests + docs. Agent produces a 200-line inline plan.
> **RIGHT**: Agent says "This is a multi-phase task requiring subagents" and spawns the first agent.

---

## Common Commands

- **Build**: `dotnet build`
- **Restore**: `dotnet restore`
- **Test**: `dotnet test`
- **Test (filtered)**: `dotnet test --filter "FullyQualifiedName~MyTestClass"`

### Development Requirements

- .NET 10.0 SDK (as specified in global.json)
- C# 14.0 (as specified in Directory.Build.props)
- Multiple .NET versions (8.0, 9.0, 10.0) for framework compatibility testing

---

## Tier 1: Architecture Essentials

A .NET library providing Smart Enums, Value Objects, and Discriminated Unions via Roslyn Source Generators.

### Type Categories

| Type                 | Attribute                                     | Description                                                                                                      |
|----------------------|-----------------------------------------------|------------------------------------------------------------------------------------------------------------------|
| Keyed Smart Enum     | `[SmartEnum<TKey>]`                           | Type-safe enum with underlying key value (int, string, Guid, custom). Items are `public static readonly` fields. |
| Keyless Smart Enum   | `[SmartEnum]`                                 | Type-safe enum without underlying value. Identified by field reference only.                                     |
| Simple Value Object  | `[ValueObject<TKey>]`                         | Single-value immutable type wrapping one underlying value.                                                       |
| Complex Value Object | `[ComplexValueObject]`                        | Multi-property immutable type.                                                                                   |
| Ad-hoc Union         | `[Union<T1, T2, ...>]` or `[AdHocUnion(...)]` | "One of" several types. Up to 5 type parameters. Cannot be generic.                                              |
| Regular Union        | `[Union]`                                     | Inheritance-based union. Abstract base with sealed derived types.                                                |

All types must be declared as `partial`. Source generators produce: factory methods (`Create`, `TryCreate`, `Validate`), equality members, conversion operators, `Switch`/`Map` pattern matching, `IParsable<T>`/`ISpanParsable<T>` (NET9+), and serialization integration.

### Type Details

**Keyed Smart Enums** (`[SmartEnum<TKey>]`):

- Items are `public static readonly` fields. No on-demand creation.
- Interface implementations depend on key type capabilities: `IParsable<T>`, `ISpanParsable<T>` (NET9+), `IComparable<T>`, `IFormattable`
- Span-based JSON deserialization (NET9+, string keys): automatic, opt out via `DisableSpanBasedJsonConversion = true`

**Keyless Smart Enums** (`[SmartEnum]`):

- No key, no lookup by value. Identified solely by field reference.

**Simple Value Objects** (`[ValueObject<TKey>]`):

- String keys MUST specify `[KeyMemberEqualityComparer<...>]`
- Validation: prefer `ValidateFactoryArguments` over `ValidateConstructorArguments`
- Zero-allocation JSON (NET9+): opt-in via `[ObjectFactory<ReadOnlySpan<char>>(UseForSerialization = SerializationFrameworks.SystemTextJson)]`

**Complex Value Objects** (`[ComplexValueObject]`):

- Use `[IgnoreMember]` to exclude properties. Use `[MemberEqualityComparer<...>]` for custom equality.

**Ad-hoc Unions** (`[Union<T1, T2>]` or `[AdHocUnion(...)]`):

- Stateless types (`TXIsStateless = true`): store only discriminator, not instance. Prefer structs.

**Regular Unions** (`[Union]`):

- Base type abstract, derived types sealed. Conversion operators + Switch/Map.

### What Gets Generated

All types: equality members (`Equals`, `GetHashCode`, `==`, `!=`), `Switch`/`Map` pattern matching.

Additionally per type:

- **Keyed Smart Enums / Value Objects**: factory methods (`Create`, `TryCreate`, `Validate`), conversion operators, `IParsable<T>`/`ISpanParsable<T>`, serializer integration
- **Complex Value Objects**: factory methods, validation
- **Ad-hoc Unions**: constructors, implicit/explicit conversion operators, `IsT1`/`AsT1` properties
- **Regular Unions**: implicit/explicit conversion operators from derived types

### Framework Integration Quick Reference

- **Serialization** (System.Text.Json, MessagePack, Newtonsoft.Json): Reference integration package for auto-generation. NET9+ string-based Smart Enums use zero-allocation span-based JSON by default.
- **Entity Framework Core**: Version-specific packages (8/9/10). Call `.UseThinktectureValueConverters()`.
- **ASP.NET Core**: Model binding via auto-generated `IParsable<T>`. Custom parsing via `[ObjectFactory<string>]`.
- **Swashbuckle/OpenAPI**: Schema and operation filters for proper documentation.

### Source Generator Pipeline

```
SyntaxProvider (filter by attribute via ForAttributeWithMetadataName)
  → Transform (extract semantic info from ISymbol into state object)
  → State Object (lightweight, IEquatable<T>, no ISymbol references)
  → Code Generator Factory (selects appropriate generator)
  → Code Generator (produces C# via StringBuilder)
```

### Generator Quick Reference

| Generator                      | State Object                                                                     | Key Code Generators                                                |
|--------------------------------|----------------------------------------------------------------------------------|--------------------------------------------------------------------|
| `SmartEnumSourceGenerator`     | `SmartEnumSourceGeneratorState`                                                  | `SmartEnumCodeGenerator`, `KeyedJsonCodeGenerator`                 |
| `ValueObjectSourceGenerator`   | `KeyedValueObjectSourceGeneratorState`, `ComplexValueObjectSourceGeneratorState` | `KeyedValueObjectCodeGenerator`, `ComplexValueObjectCodeGenerator` |
| `AdHocUnionSourceGenerator`    | `AdHocUnionSourceGenState`                                                       | `AdHocUnionCodeGenerator`                                          |
| `RegularUnionSourceGenerator`  | `RegularUnionSourceGenState`                                                     | `RegularUnionCodeGenerator`                                        |
| `ObjectFactorySourceGenerator` | `ObjectFactorySourceGeneratorState`                                              | `ObjectFactoryCodeGenerator`                                       |
| `AnnotationsSourceGenerator`   | —                                                                                | JetBrains annotations                                              |

### Analyzers

1. `ThinktectureRuntimeExtensionsAnalyzer` -- 54 diagnostic rules (`TTRESG` prefix) for correct usage
2. `ThinktectureRuntimeExtensionsInternalUsageAnalyzer` -- Prevents external use of internal APIs

### Runtime Metadata

Generated types implement `IMetadataOwner`. At runtime, `MetadataLookup` (in `Thinktecture.Internal`) discovers and caches metadata for serialization, model binding, and type conversion. **Object factories (`[ObjectFactory<T>]`) have priority over key-based metadata** when both exist.

### Common Bug Patterns

| Pattern                                                                               | Where to Look                               |
|---------------------------------------------------------------------------------------|---------------------------------------------|
| State equality staleness -- `Equals`/`GetHashCode` not updated after adding new field | State object classes                        |
| Missing `#if` block -- NET9+ feature generated without conditional compilation        | Code generators                             |
| Attribute property not extracted -- new property added but not read                   | `AttributeDataExtensions`, settings classes |
| Object factory priority -- factory overrides key-based metadata unexpectedly          | `MetadataLookup.FindMetadataForConversion`  |

### Key Directories

- **`src/Thinktecture.Runtime.Extensions`** -- Core library (interfaces, attributes, runtime helpers)
- **`src/Thinktecture.Runtime.Extensions.SourceGenerator`** -- Source Generators and Analyzers
- **Framework Integration** -- `.Json`, `.MessagePack`, `.Newtonsoft`, `.EntityFrameworkCore8/9/10`, `.AspNetCore`, `.Swashbuckle`

### Key Files

- `Thinktecture.Runtime.Extensions.slnx` -- Main solution file (.slnx format)
- `Directory.Build.props` -- Global MSBuild properties (version, framework targets)
- `Directory.Packages.props` -- Centralized NuGet package version management (all versions here)
- `global.json` -- .NET SDK version specification
- `.editorconfig` -- Code style configuration (especially `src/.editorconfig`)

---

## Tier 1: Cross-Cutting Policies

These rules apply to ALL tasks. They are mandatory and non-negotiable.

### Zero-Hallucination Policy

**NEVER write, plan, or design code that calls an external API without first verifying its signature via Context7.** This includes .NET BCL methods you think you know. Training data may be outdated or wrong -- always verify.

**Early trigger**: If the user's prompt names a specific external API by name (e.g., `JsonSerializer.Deserialize`, `ISpanParsable<T>.Parse`), verify that API as your first action -- before any codebase exploration. See Pre-Flight Protocol Step 1.

**What Requires Verification**: All external APIs -- .NET BCL (`System.*`, `Microsoft.*`), Roslyn APIs (`Microsoft.CodeAnalysis.*`), serialization frameworks (System.Text.Json, MessagePack, Newtonsoft.Json), testing frameworks (xunit.v3, AwesomeAssertions, Verify.XunitV3), framework integrations (EF Core, ASP.NET Core, Swashbuckle), and any third-party NuGet package.

**Verification Workflow** (mandatory, not optional):

1. **Recognize**: The task involves an external API (named in the prompt or needed for implementation).
2. **Stop**: Do not write the code yet. Do not defer verification until later.
3. **Verify via Context7 MCP**: Call `mcp__context7__resolve-library-id` then `mcp__context7__query-docs` to confirm the exact method signature, parameters, and return type.
4. **Implement**: Base all implementation decisions on verified documentation only.

Skipping step 3 is a policy violation, even if you are confident you know the API. "I'll verify later" is also a violation -- verify now.

> **Common violation**: User says "Add a method using JsonSerializer.Deserialize with JsonTypeInfo." Agent explores the codebase for 20+ tool calls, then writes code using the API from memory. **This is wrong.** The agent's first tool call should have been `resolve-library-id` for System.Text.Json.

**Internal Code is Authoritative**: For this repository, use Serena tools (`find_symbol`, `get_symbols_overview`, `search_for_pattern`) to read actual source code. Do not use Context7 for internal APIs.

**When Context7 Lacks the Information**: Use web search as fallback. If that also fails, tell the user: "I could not verify [API]. I need clarification before proceeding." Never guess.

### Code Style Policy

**General**:

- Follow `.editorconfig` settings (especially `src/.editorconfig`)
- XML documentation required for all publicly visible types and members in `src/Thinktecture.Runtime.Extensions` and framework integration projects
- XML documentation NOT required in source generator, analyzer, test, and sample projects
- Do not use `#region`/`#endregion`

**Source Generator Code** (treat as a checklist -- verify all items before presenting code):

- All generators implement `IIncrementalGenerator` with the incremental pipeline pattern
- State objects must implement `IEquatable<T>` with proper `Equals` and `GetHashCode` -- this is critical for incremental generation caching
- State objects must be lightweight: extract data from `ISymbol` into plain properties, never carry `ISymbol` references
- Use `ImmutableArray<T>` for collections in state objects
- Use `unchecked` blocks in `GetHashCode` implementations with the `(hashCode * 397) ^ ...` pattern
- Code generators use `StringBuilder` for output construction
- Use Fully Qualified Names: Use `global::` prefix for type references (e.g., `global::System.ArgumentException`) -- do not generate using directives. Exception: C# language keywords (`string`, `int`, `bool`, etc.) are used as-is
- Use `#if NET9_0_OR_GREATER` conditional compilation in generated code for NET9+-only features
- Generated files use file-scoped namespaces

**Runtime Library Code**:

- Public API surface must have XML documentation
- Internal implementation classes go in `Thinktecture.Internal` namespace
- Use `[MethodImpl(MethodImplOptions.AggressiveInlining)]` for performance-critical small methods
- Thread `CancellationToken` through long-running operations

**Analyzer Code**:

- Diagnostic IDs use `TTRESG` prefix (e.g., `TTRESG001`)
- Category is always `"Thinktecture.Runtime.Extensions"`
- Enable concurrent execution: `context.EnableConcurrentExecution()`
- Configure generated code analysis: `context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None)`
- Report diagnostics with precise locations

**Test Code**:

- Use xunit.v3 as the test framework (xUnit v3 -- different API surface from v2)
- Use AwesomeAssertions for fluent assertions
- Use Verify.XunitV3 for snapshot testing generated code
- Test naming: `Should_[ExpectedBehavior]_when_[Condition]`

**Multi-Target Framework**:

- Core library targets net8.0 as base; integration packages are multi-targeted (net8.0, net9.0, net10.0)
- Use `#if NET9_0_OR_GREATER` for span-based features, `allows ref struct`, `ISpanParsable<T>`
- Always test on all target frameworks

**Generated Code Patterns** (rules for what source generators emit):

- Generated arithmetic operators: implement both checked and unchecked versions
- Thread `IFormatProvider` through all generated parsing/formatting methods
- Use `StaticAbstractInvoker.ParseValue<TKey>` for zero-allocation span-based parsing on NET9+

These policies are the single source of truth. There are no separate policy files.

---

## Tier 2: Task Guides

When starting a task, identify the user's intent and load **one** guide for detailed workflow instructions. See Pre-Flight Protocol Step 2.

**Your next tool call after reading this file must be loading the matched guide.** Do not call `find_symbol`, `search_for_pattern`, `get_symbols_overview`, or any codebase exploration tool before loading the guide. The guide contains project-specific patterns and constraints that override general knowledge.

> **Common violation**: User says "I want to add feature X." Agent immediately calls find_symbol or search_for_pattern to explore the codebase. **This is wrong.** The agent should load `guides/DESIGN-DISCUSSION.md` first, then follow the workflow inside it.

| User Intent                              | Load Guide                             | Agent (if complex)                                     |
|------------------------------------------|----------------------------------------|--------------------------------------------------------|
| Bug report / "X doesn't work"            | `guides/INVESTIGATION.md`              | `bug-investigator`                                     |
| Feature request / "I want to add X"      | `guides/DESIGN-DISCUSSION.md`          | `design-advisor` then `feature-implementation-planner` |
| "Implement the plan"                     | `guides/IMPLEMENTATION.md`             | `feature-implementer`                                  |
| Write/update tests                       | `guides/TESTING.md`                    | `test-writer`                                          |
| Troubleshooting / quick fix              | `guides/INVESTIGATION.md`              | (inline response)                                      |
| Review code changes                      | `guides/INVESTIGATION.md`              | `feature-reviewer`                                     |
| Write user documentation                 | (no guide needed -- embedded in agent) | `documentation-updater`                                |
| "How do I add a new analyzer/attribute?" | `guides/IMPLEMENTATION.md`             | (inline response)                                      |

**Before ANY code changes, follow the Cross-Cutting Policies above** (they are already in this file).

**Serena memories** -- read when working in related areas (check `list_memories` for available topics).

### Task Delegation to Subagents

**Delegation means spawning a subagent via the Task tool.** Creating an inline plan is NOT delegation. If you find yourself writing an implementation plan longer than 10 lines, stop and delegate instead.

**ALWAYS delegate when the user's request involves 3 or more of:** design, implementation, testing, review, documentation. This is a hard trigger -- do not override it. See Pre-Flight Protocol Step 3.

**Also delegate when:**

- Implementing a new feature (multi-step)
- Writing comprehensive tests
- Updating documentation after changes
- Reviewing completed implementations

**Do NOT delegate:**

- Single-file edits, quick bug fixes, simple refactoring, answering questions
- Note: if the user's request explicitly includes "tests and documentation" alongside implementation, that is 3+ phases and ALWAYS triggers delegation, even if it looks like a simple feature

**Typical workflow for a feature request:**

1. `design-advisor` -- discuss design and tradeoffs
2. `feature-implementation-planner` -- create detailed implementation plan
3. `feature-implementer` -- write code
4. `test-writer` -- add tests
5. `feature-reviewer` -- review changes
6. `documentation-updater` -- update docs

> **Common violation**: User asks for "feature X including tests and documentation." Agent produces a 200-line inline implementation plan. **This is wrong.** The agent should say "This requires multiple specialized phases. I'll coordinate subagents." and spawn the first agent.

---

## Tier 3: Detailed Reference

Load these only when you need exact specifications during a task. Do not load preemptively.

| Resource                  | When to Load                                                                   |
|---------------------------|--------------------------------------------------------------------------------|
| `reference/ATTRIBUTES.md` | Need exact attribute property names, types, defaults, or configuration options |
