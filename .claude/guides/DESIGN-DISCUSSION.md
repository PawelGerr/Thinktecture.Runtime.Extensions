# Design Discussion Guide

How to evaluate API design proposals and feature designs for Thinktecture.Runtime.Extensions.

## 1. Design Principles

Every new feature must align with these principles. If a design conflicts with any of them, it needs explicit justification.

**Consistency**: New features follow established patterns -- attribute-driven configuration, source-generated implementation, partial types. Users who know how Smart Enums work should intuitively understand how a new feature works.

**Simplicity for consumers**: The user-facing API should be minimal and intuitive. Complexity belongs in generated code, not in the attribute surface or runtime API. If a user needs to read extensive documentation to use the feature, the API is too complex.

**Immutability by default**: Smart Enums, Value Objects, and Ad-hoc Unions enforce immutability in generated code (readonly fields, no setters). Note: this does not apply to the ad-hoc union's type members themselves (those are user-defined types), nor to Regular Unions (derived types are fully user-controlled).

**Framework integration from the start**: Consider serialization (System.Text.Json, MessagePack, Newtonsoft.Json), EF Core (versions 8/9/10), ASP.NET Core model binding, and Swashbuckle/OpenAPI from the initial design. Retrofitting integration is expensive and error-prone.

**Zero-allocation where possible**: On NET9+, prefer span-based and stack-allocated patterns. Use `ISpanParsable<T>`, `ReadOnlySpan<char>`, and `Utf8JsonReaderHelper` for zero-allocation deserialization paths. Older frameworks get the allocating path.

**Backward compatibility**: Existing generated code must not break when the library is updated. If a change alters generated code shape, it needs a clear migration path and must be flagged as breaking.

## 2. Feature Design Checklist

When designing a new feature, systematically evaluate each of these areas.

### API Surface

- What attribute(s) does the user interact with?
- What new properties does the attribute need? Are they opt-in or opt-out?
- What generated code does the user see (methods, operators, interfaces)?
- Is the naming consistent with existing patterns (e.g., `Create`, `TryCreate`, `Switch`, `Map`)?
- Can the feature be configured per-type? Is there a global default?
- What is the minimal configuration needed to use the feature?

### Source Generator Impact

- Which generator(s) need modification? (SmartEnum, ValueObject, AdHocUnion, RegularUnion, ObjectFactory)
- What state object changes are needed? (New properties, new collections)
- What new code generator(s) are needed? Which `CodeGeneratorFactory` do they belong to?
- Does the pipeline need new transform steps?
- Does the incremental generation cache invalidation still work correctly?

### Analyzer Impact

- What new diagnostic rules are needed?
- What invalid configurations should be detected at compile time?
- Should a code fix be provided for common mistakes?
- Are there diagnostics that should be warnings vs. errors?

### Breaking Change Assessment

- Does this change the shape of currently generated code?
- Do existing attribute usages still compile without modification?
- Do existing tests still pass without modification?
- If breaking: can users migrate incrementally, or is it all-or-nothing?
- If breaking: is the benefit worth the cost to existing users?

### Framework Integration

- **System.Text.Json**: New converter needed? Changes to `ThinktectureJsonConverterFactory`?
- **MessagePack**: New formatter needed? Changes to formatters?
- **Newtonsoft.Json**: New converter needed?
- **EF Core**: New value converters? Changes to `.UseThinktectureValueConverters()`?
- **ASP.NET Core**: Model binding changes? New `IParsable<T>` behavior?
- **Swashbuckle/OpenAPI**: Schema changes? New operation filters?

### Multi-Target Considerations

- Does this feature work on all target frameworks (net8.0, net9.0, net10.0)?
- Does it need `#if` conditional compilation?
- Are there NET9+-only features involved (e.g., `ISpanParsable`, `allows ref struct`)?
- Does the generated code need different paths per target framework?

## 3. Evaluating Alternatives

When multiple approaches exist, evaluate each one systematically:

1. **List each alternative** with a one-sentence summary
2. **Pros/cons** for each, focusing on:
    - User experience (which is easiest for the library consumer?)
    - Blast radius (which changes the least generated code?)
    - Testability (which is easiest to write comprehensive tests for?)
    - Consistency (which best matches existing patterns?)
    - Future extensibility (which leaves room for future enhancements?)
3. **Evaluate against design principles** (Section 1)
4. **Recommend one approach** with clear reasoning

When in doubt, prefer the approach that is simplest for the consumer, even if it requires more work in the source generator.

## 4. Configuration Patterns

How to expose new feature configuration to users:

**Attribute property** -- Best for per-type configuration that modifies the generated code shape.

- Example: `SkipIParsable`, `DisableSpanBasedJsonConversion`, `SkipKeyMember`
- Use when: the configuration is specific to a single type declaration

**Separate attribute** -- Best for orthogonal features that can be independently applied.

- Example: `[ObjectFactory<T>]`, `[KeyMemberEqualityComparer<...>]`, `[MemberEqualityComparer<...>]`
- Use when: the feature has its own parameters, or applies to a member rather than the type

**Convention-based** -- Best for features that should "just work" without user intervention.

- Example: automatic `ISpanParsable<T>` detection based on key type capabilities
- Use when: there is one obvious correct behavior and opt-out is the exception

**Precedence**: Convention > Attribute property > Separate attribute. Start with conventions, add explicit configuration only when needed.

## 5. Naming Conventions

### Attribute Properties

- PascalCase always
- Boolean opt-out properties: prefix with `Skip` (e.g., `SkipIParsable`, `SkipToString`)
- Boolean opt-in properties: prefix with `Enable` or `Allow` (e.g., `AllowDefaultStructs`)
- Boolean disable properties: prefix with `Disable` (e.g., `DisableSpanBasedJsonConversion`)

### Generated Methods

Follow existing patterns exactly:

- Factory: `Create`, `TryCreate`
- Validation: `Validate`, `ValidateFactoryArguments`
- Pattern matching: `Switch`, `Map`
- Conversion: `Get`, implicit/explicit operators
- Parsing: `Parse`, `TryParse`

### Generated Interfaces

Use established Thinktecture interfaces:

- `IConvertible<T>`, `IObjectFactory<T>`, `IMetadataOwner`
- New interfaces follow the `IThinktecture*` or `I*` pattern from the core library

### Type Names

- State objects: `{Feature}State` (e.g., `SmartEnumState`, `ValueObjectState`)
- Code generators: `{Feature}CodeGenerator` (e.g., `SmartEnumSwitchMethodCodeGenerator`)
- Code generator factories: `{Feature}CodeGeneratorFactory`

## 6. Documentation Requirements

Every new feature needs:

- **XML documentation** on all new public types and members (enforced by `.editorconfig`)
- **Update to relevant guide** (`guides/IMPLEMENTATION.md` if new source generator patterns)
- **Update to `reference/ATTRIBUTES.md`** if new attribute properties are added
- **Sample code** demonstrating typical usage
- **Migration notes** if existing behavior changes
