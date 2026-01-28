# Thinktecture.Runtime.Extensions - AI Assistant Guide

This file provides core guidance for working with this repository. For specialized tasks, consult the referenced documentation files.

## Common Commands

### Build and Test

- **Build solution**: `dotnet build`
- **Restore packages**: `dotnet restore`
- **Run all tests**: `dotnet test`

### Development Requirements

- .NET 10.0 SDK (as specified in global.json)
- C# 11+ for generated code
- Multiple .NET versions (8.0, 9.0, 10.0) for framework compatibility testing

## AI Assistant Guidelines

### Task Delegation to Subagents

For larger tasks with multiple distinct steps, **always use specialized subagents** to keep the main context clean and manageable:

**ALWAYS delegate to subagents when:**

- Implementing a new feature (use `feature-implementation-planner` → `feature-implementer`)
- Writing comprehensive tests (use `feature-test-writer`)
- Updating documentation after changes (use `documentation-updater`)
- Reviewing completed implementations (use `feature-reviewer`)
- Any task requiring multiple distinct sequential steps

**Why this matters:**

- Prevents main context from growing too large with implementation details
- Keeps conversation focused on high-level coordination
- Allows specialized agents to maintain their own focused context
- Enables better parallel execution of independent tasks

**Example workflow for "Add feature X":**

1. User requests feature → Use `feature-implementation-planner` to create plan
2. Plan approved → Use `feature-implementer` to write code
3. Implementation complete → Use `feature-test-writer` to add tests
4. Tests passing → Use `documentation-updater` to update docs
5. All done → Use `feature-reviewer` for final review

**Do NOT delegate simple tasks:**

- Single-file edits
- Quick bug fixes
- Simple refactoring
- Answering questions about code

### Zero-Tolerance for Hallucination

When working with external libraries, frameworks, or any APIs (including .NET BCL, ASP.NET Core, Entity Framework Core, serialization frameworks, etc.), you MUST adhere to strict verification guidelines:

### 1. Never Guess or Assume API Behavior

- **DO NOT** make assumptions about API signatures, method names, parameter types, return types, or behavior
- **DO NOT** rely on memory or general knowledge about libraries
- **DO NOT** proceed with implementation if you are not 100% certain about API details

### 2. Verify Using Context7 MCP

When you need information about any external library or framework:

1. **Use `mcp__context7__resolve-library-id`** to find the correct library ID
2. **Use `mcp__context7__get-library-docs`** to retrieve up-to-date, accurate documentation
3. **Base all implementation decisions on verified documentation**, not assumptions

Examples of when to use Context7:

- Working with .NET BCL types (System.Text.Json, System.Linq, System.Collections, etc.)
- Integration with frameworks (ASP.NET Core, Entity Framework Core, xUnit, etc.)
- Third-party libraries (MessagePack, Newtonsoft.Json, ProtoBuf, etc.)
- Roslyn APIs for source generators
- Any API where you are not 100% certain of the exact behavior

### 3. Verification Over Speed

- **It is better to take extra time to verify than to introduce bugs based on incorrect assumptions**
- If documentation is unclear or incomplete, ask the user for clarification
- If Context7 doesn't have the information, explicitly tell the user you need to verify before proceeding

### 4. Explicit Uncertainty Communication

When you encounter uncertainty:

- **State clearly**: "I need to verify the API behavior using Context7"
- **Never proceed silently** with guessed implementations
- **Document verification steps** so the user understands your process

### 5. This Project's Code is Authoritative

- For Thinktecture.Runtime.Extensions library code itself (not external dependencies), use the codebase as the source of truth
- Read actual source code using Serena tools to understand internal behavior
- Only for external dependencies must you use Context7 for verification

### Summary

**Zero hallucination policy**: If you don't know it with 100% certainty, verify it. Use Context7 MCP for external APIs, use Serena tools for internal codebase exploration, and ask the user when neither provides sufficient clarity.

## Architecture Overview

This is a .NET library providing **Smart Enums**, **Value Objects**, and **Discriminated Unions** through Roslyn Source Generators.

### Core Components

- **`src/Thinktecture.Runtime.Extensions`**: Core library with base interfaces, attributes, and runtime helpers
- **`src/Thinktecture.Runtime.Extensions.SourceGenerator`**: Roslyn Source Generators (6) and Analyzers (2) that create boilerplate code and validate usage
- **Framework Integration Projects**: Separate projects for JSON, MessagePack, Newtonsoft.Json, ProtoBuf, EF Core (8/9/10), ASP.NET Core, and Swashbuckle

### Source Generators

1. **`SmartEnumSourceGenerator`**: For `[SmartEnum<T>]` or `[SmartEnum]` - factory methods, operators, interfaces, Switch/Map
2. **`ValueObjectSourceGenerator`**: For `[ValueObject<T>]` or `[ComplexValueObject]` - factory methods, equality, operators, interfaces
3. **`AdHocUnionSourceGenerator`**: For `[Union<T1, T2, ...>]` or `[AdHocUnion(...)]` - conversion operators, Switch/Map
4. **`RegularUnionSourceGenerator`**: For `[Union]` inheritance-based unions - factory methods, Switch/Map
5. **`ObjectFactorySourceGenerator`**: For `[ObjectFactory<T>]` - custom serialization/parsing logic
6. **`AnnotationsSourceGenerator`**: JetBrains annotations if not already present

### Analyzers

1. **`ThinktectureRuntimeExtensionsAnalyzer`**: 40+ diagnostic rules validating correct usage (partial types, constructors, members, comparers, etc.)
2. **`ThinktectureRuntimeExtensionsInternalUsageAnalyzer`**: Prevents usage of internal library APIs outside Thinktecture modules

### Key Concepts

#### Smart Enums

- **Keyless `[SmartEnum]`**: Type-safe enums without underlying values (items as public static readonly fields)
- **Keyed `[SmartEnum<TKey>]`**: Type-safe enums with underlying key values (int, string, Guid, custom types)
    - Can be generic types
    - Supports `IParsable<T>`, `ISpanParsable<T>` (NET9+, zero-allocation), `IComparable<T>`, `IFormattable`
    - Configurable operators, conversion, Switch/Map generation

#### Value Objects

- **Simple `[ValueObject<TKey>]`**: Single-value immutable types (e.g., Amount, ProductId)
    - String keys MUST specify `[KeyMemberEqualityComparer<...>]`
    - Supports arithmetic operators, `IParsable<T>`, `ISpanParsable<T>` (NET9+)
    - Can be generic types
- **Complex `[ComplexValueObject]`**: Multi-property immutable types (e.g., DateRange)
    - Use `[IgnoreMember]` to exclude properties
    - Use `[MemberEqualityComparer<...>]` for custom per-member equality
    - Can be generic types

#### Discriminated Unions

- **Ad-hoc `[Union<T1, T2>]` or `[AdHocUnion(typeof(T1), typeof(T2))]`**: Simple 2-5 type combinations
    - Implicit conversion operators, IsT1/AsT1 properties, Switch/Map
    - Stateless types (`TXIsStateless = true`): Memory-efficient members that store only discriminator, not instance data (prefer structs to avoid null-handling)
- **Regular `[Union]`**: Inheritance-based unions with derived types
    - Static factory methods, Switch/Map over all derived types

### Source Generation Pattern

Types must be `partial` classes/structs. Generator creates:

- Constructors and factory methods (`Create`, `TryCreate`, `Validate`)
- Equality members (`Equals`, `GetHashCode`, operators)
- Conversion operators and `IParsable<T>` implementations
- Pattern matching methods (`Switch`, `Map`)
- Integration with serializers and frameworks

### Runtime Metadata System

Generated types implement `IMetadataOwner` interface with runtime metadata that enables:

- Serialization framework integration (JSON, MessagePack, Protobuf, etc.)
- Type conversion and model binding
- Discovery of key types and validation error types
- Custom object factory resolution

The `MetadataLookup` class provides cached metadata discovery via reflection. Object factories have priority over key-based metadata for conversion scenarios.

**For detailed information**: See "Runtime Metadata System" section in [CLAUDE-FEATURE-DEV.md](CLAUDE-FEATURE-DEV.md) - essential reading when implementing serialization integrations or custom conversion logic.

## Development Guidelines

### Code Style

- Follow `.editorconfig` settings (especially in `src/.editorconfig`)
- **XML documentation required** for all publicly visible types and members (except source generator, analyzer, test, and sample projects)
- Multi-target framework support (net8.0 base, with EF Core version-specific projects)
- Don't use `#region`/`#endregion`

### Validation Implementation

- **Always prefer `ValidateFactoryArguments`** over `ValidateConstructorArguments`
    - `ValidateFactoryArguments` returns `ValidationError` for better framework integration
    - `ValidateConstructorArguments` can only throw exceptions, integrates poorly with frameworks
- Use `ref` parameters to normalize values during validation

### Framework Integration Quick Reference

**Serialization**: System.Text.Json, MessagePack, Newtonsoft.Json, ProtoBuf - reference package for auto-generation or manually register converters

**Entity Framework Core**: Version-specific packages (8/9/10) - call `.UseThinktectureValueConverters()` on DbContextOptionsBuilder

**ASP.NET Core**: Model binding via `IParsable<T>` interface (auto-generated) - use `[ObjectFactory<string>]` for custom parsing

**Swashbuckle/OpenAPI**: Schema and operation filters for proper OpenAPI documentation

## Quick Troubleshooting

### Common Issues

1. **"Type must be partial"**: Add `partial` keyword to your class/struct declaration
2. **"String-based value object needs equality comparer"**: Add `[KeyMemberEqualityComparer<MyType, string, StringComparer>]` attribute
3. **"Smart enum has no items"**: Ensure items are public static readonly fields of the enum type
4. **Serialization not working**: Ensure integration package is referenced, or manually register converters/formatters
5. **EF Core not converting**: Call `.UseThinktectureValueConverters()` on DbContextOptionsBuilder
6. **ISpanParsable not available**: Requires NET9+; ensure project targets `net9.0` or later and key type implements `ISpanParsable<TKey>`

### Best Practices

1. **String-based keys/members**: Always explicitly specify equality comparer to avoid culture-sensitive comparisons
2. **Validation**: Prefer `ValidateFactoryArguments` over `ValidateConstructorArguments`
3. **Immutability**: All members should be readonly (fields) or have no setter/private init (properties)
4. **Constructors**: Keep constructors private to enforce use of factory methods
5. **Smart Enum items**: Must be public static readonly fields
6. **Partial keyword**: Types must be marked `partial` for source generators to work
7. **Culture-specific parsing**: Always pass appropriate `IFormatProvider` when parsing/formatting culture-sensitive types
8. **Arithmetic operators**: Use unchecked arithmetic context - overflow/underflow wraps around

## Project Structure

### Key Files

- `Thinktecture.Runtime.Extensions.slnx`: Main solution file (.slnx format)
- `Directory.Build.props`: Global MSBuild properties (version, framework targets)
- `Directory.Packages.props`: Centralized NuGet package version management - **manage all package versions here**
- `global.json`: .NET SDK version specification (currently 9.0.0)
- `.editorconfig`: Code style configuration (especially in `src/`)

## Specialized Documentation

For specific tasks, consult these specialized documentation files:

- **[CLAUDE-FEATURE-DEV.md](CLAUDE-FEATURE-DEV.md)**: Source generator architecture, runtime metadata system (MetadataLookup), implementing new features, state objects, code generator patterns
- **[CLAUDE-TESTING.md](CLAUDE-TESTING.md)**: Testing strategy, test organization, frameworks (xUnit, Verify, AwesomeAssertions)
- **[CLAUDE-REVIEW.md](CLAUDE-REVIEW.md)**: Code review checklists, best practices verification, common pitfalls
- **[CLAUDE-ATTRIBUTES.md](CLAUDE-ATTRIBUTES.md)**: Complete attribute reference with all properties and configuration options

**When to consult specialized docs:**

- Implementing a new feature → Read CLAUDE-FEATURE-DEV.md
- Working with serialization/framework integration → Read CLAUDE-FEATURE-DEV.md (Runtime Metadata System section)
- Writing or updating tests → Read CLAUDE-TESTING.md
- Reviewing code changes → Read CLAUDE-REVIEW.md
- Working with attributes or need configuration details → Read CLAUDE-ATTRIBUTES.md
