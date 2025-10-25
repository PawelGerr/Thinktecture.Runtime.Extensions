Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/InterfaceCodeGeneratorState.cs

- Default(struct) permits invalid null state:
  - As a readonly struct, `default(InterfaceCodeGeneratorState)` results in `Type == null`, `KeyMember == null`, and `CreateFactoryMethodName == null` even though all are non-nullable. Any dereference or hashing will throw.
  - Recommendation: Prefer a class to avoid invalid default, or add an `IsDefault/IsValid` convention and guard usage everywhere. Alternatively, keep the struct but ensure creation is fully controlled and defaults never escape.

- Missing constructor argument validation:
  - The constructor does not validate `type`, `keyMember`, or `createFactoryMethodName`. Misuse (reflection/external) may pass nulls and cause NREs later (e.g., `CreateFactoryMethodName.GetHashCode()`).
  - Recommendation: Add `ArgumentNullException.ThrowIfNull(type);`, `ArgumentNullException.ThrowIfNull(keyMember);`, and `ArgumentException.ThrowIfNullOrWhiteSpace(createFactoryMethodName);`.

- Equality/hash rely on downstream value semantics:
  - `Equals`/`GetHashCode` defer to `Type.Equals` and `KeyMember.Equals`. If these interfaces’ implementations are not strict value-based or change over time, equality semantics here become unstable.
  - Recommendation: Ensure `ITypeInformation` and `IMemberInformation` implement consistent value-based equality/hash. Consider documenting the dependency or comparing a canonical identity (e.g., fully-qualified names plus relevant flags) directly.

- String comparison/hash intent not explicit:
  - Uses `==` and `GetHashCode()` on `CreateFactoryMethodName`. While ordinal string equality/hashing is fine in .NET, it’s implicit.
  - Recommendation: Document ordinal intent or use `StringComparer.Ordinal` in external comparers/collections where applicable.

- Consistency with project conventions:
  - Many state types implement `IHashCodeComputable`; this one does not.
  - Recommendation: Implement `IHashCodeComputable` if consistency is desired for testability and determinism across state objects.

- Operator overloads are redundant:
  - `==`/`!=` simply call `Equals`. This is acceptable but adds API surface without additional guarantees.
  - Recommendation: Keep if needed for ergonomic comparisons; otherwise could rely on `Equals` to minimize surface.
