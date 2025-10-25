Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/TypedMemberStateFactory.cs

- Missing constructor and method argument validation:
  - The private constructor and the public Create(Compilation) factory don’t validate `compilation`. Passing null (e.g., misuse) leads to immediate NREs inside the constructor.
  - The public `ITypedMemberState Create(ITypeSymbol type)` does not validate `type`, which will NRE on `type.SpecialType`.
  - Recommendation: Add `ArgumentNullException.ThrowIfNull(compilation);` in `Create`, and `ArgumentNullException.ThrowIfNull(type);` in `Create(ITypeSymbol)`.

- Cached “nullable” variants for value types are never used:
  - `CreateStates` builds both NotNullable and Nullable states for a given type. However, in `Create(ITypeSymbol)`, the returned state for value types (e.g., `int?`, `DateTime?`) will never hit the cached Nullable variant:
    - `Nullable<T>` has `SpecialType == System_Nullable_T`, so the `switch` won’t hit the underlying primitive/special case (e.g., `System_Int32`, `System_DateTime`).
    - The selection between `.Nullable` and `.NotNullable` is based solely on `type.NullableAnnotation == Annotated`, which is relevant for reference types, not for `Nullable<T>` value types.
    - The fallback `_statesByTokens` key is built from the (module, metadata token) of the passed `type`; for `Nullable<T>` that points to `Nullable<T>` not the underlying `T`, so it won’t match the cached entry for `T`.
  - Impact: Unnecessary allocations of `TypedMemberState` for `Nullable<T>` types; the “nullable” cache for value types is effectively dead.
  - Recommendation: Detect `Nullable<T>` in `Create(ITypeSymbol)`:
    - If `type is INamedTypeSymbol { ConstructedFrom.SpecialType: SpecialType.System_Nullable_T } named`, let `underlying = (INamedTypeSymbol)named.TypeArguments[0]`. Then route to the cache for `underlying` and return its `.Nullable`.
    - Alternatively, insert entries into `_statesByTokens` keyed by both `T` and `Nullable<T>` (using `ConstructedFrom.SpecialType` + underlying token), but the first approach is cleaner.

- Inconsistent special-type handling (e.g., TimeSpan):
  - Special types are pre-cached for many primitives and DateTime, but TimeSpan is cached via `_statesByTokens` rather than a dedicated branch. This asymmetry isn’t a correctness bug, but it’s confusing and may reduce hit rate symmetry across types.
  - Recommendation: Either add a `SpecialType.System_TimeSpan` branch for consistency, or document why it’s in the token map rather than the special-type switch.

- Fragile identity via (module, metadata token):
  - `_statesByTokens` keys by `(type.ContainingModule.MetadataName, type.MetadataToken)`. That’s generally stable within a single Compilation, but metadata tokens are not stable across compilations/versions.
  - Also, for constructed generic types like `Nullable<T>`, the token is for the constructed type, not the argument `T`, which explains the missed cache hits described above.
  - Recommendation: Add explicit `Nullable<T>` detection as above. Optionally switch to identities based on the underlying definition + arguments (e.g., `INamedTypeSymbol.ConstructedFrom` plus normalized TypeArgument identities) for non-special types.

- Duplicate type selection logic silently drops caching:
  - In `CreateAndAddStatesForSystemRuntime`, if multiple matching types are found (e.g., the type exists in both System.Runtime.dll and System.Private.CoreLib.dll), encountering a second match bails out with `return;` and adds none.
  - Impact: No cache entry for these well-known types if they appear in both modules for some target, leading to missed cache hits (performance).
  - Recommendation: Prefer a deterministic selection, e.g., choose `System.Private.CoreLib.dll` over `System.Runtime.dll` (or vice versa), and proceed. Don’t early-return without caching.

- IReadOnlyDictionary over a mutable Dictionary:
  - `_statesByTokens` is typed as `IReadOnlyDictionary` but references a mutable `Dictionary`. While currently filled only in the constructor, the mutability is not enforced.
  - Recommendation: Wrap with `new ReadOnlyDictionary<...>(lookup)` or use `ImmutableDictionary` to freeze state.

- Potential module-name fragility:
  - Hardcoded module names: `"System.Runtime.dll"` and `"System.Private.CoreLib.dll"`. This is fine for modern .NET, but if targets expand or references are split differently, cache lookups may be skipped.
  - Recommendation: Document assumptions (supported TFMs) and consider a small extension list if future framework versions relocate these types.
