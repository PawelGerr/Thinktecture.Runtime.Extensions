Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/KeyedJsonCodeGenerator.cs

- Constructor parameter validation missing:
  - No checks for `state` or `stringBuilder`. Misuse/reflection could pass null leading to NRE at runtime.
  - Recommendation: Add `ArgumentNullException.ThrowIfNull(state);` and `ThrowIfNull(stringBuilder);`.

- Potential missing accessibility on partial type:
  - The generator emits `partial <TypeKind> <Name>` without an accessibility modifier. C# requires all partial declarations of a type to have the same accessibility; omitting it here can cause CS0267 if the user-declared partial includes e.g. `public`.
  - Recommendation: Emit the same accessibility as the original type (e.g., `public partial ...`) using type metadata.

- Key type resolution can be null and generate invalid code:
  - `keyType` is computed as `customFactory?.TypeFullyQualified ?? _state.KeyMember?.TypeFullyQualified`. If both are null (e.g., missing key member and no custom factory), `keyType` is null. When `isString` is false, the code appends `keyType` into the generic argument list, resulting in an omitted parameter and malformed attribute.
  - Recommendation: Guard against null `keyType` (throw with diagnostic or fallback) when non-string and ensure a valid type is emitted.

- String key detection fragile for custom factory:
  - `isString` switches on `SpecialType == System_String`. If a custom factory targets a type alias or a non-specialized `string`-compatible type, detection could fail.
  - Recommendation: Prefer special type when available, otherwise compare against a canonical `System.String` type identity. Consider treating `ReadOnlySpan<char>`-based factories distinctly if supported.

- Generic arity contract relies on string-only special casing:
  - The generator emits `ThinktectureJsonConverterFactory<TType, ValidationError>` for string keys and `ThinktectureJsonConverterFactory<TType, TKey, ValidationError>` for non-string keys. This assumes dedicated factory types with different arities exist and match exactly this split.
  - Recommendation: Verify/encode the factory’s arity contract explicitly (e.g., via a small helper that selects the correct factory type) to avoid mismatches if new key categories are added.

- Multiple factories ambiguity:
  - Uses `FirstOrDefault` to select a factory with `UseForSerialization.Has(SystemTextJson)`. When multiple candidates exist, selection is order-dependent and unspecified.
  - Recommendation: Enforce uniqueness or select deterministically (e.g., most specific match) and emit a diagnostic if ambiguous.

- CancellationToken ignored:
  - `Generate(CancellationToken)` does not observe `cancellationToken`. For long-running generation (unlikely here) this could block cancellation.
  - Recommendation: Optionally check `cancellationToken.ThrowIfCancellationRequested()` in appropriate places.

- Minor formatting concerns:
  - `GENERATED_CODE_PREFIX` line endings are supplemented by appending an extra newline; ensure final output adheres to repository line-ending policy and does not duplicate blank lines across different combinations.

- Defensive assumptions for ValidationError type:
  - The attribute always appends `_state.AttributeInfo.ValidationError`. If this type is default/unset for a given target, codegen will fail.
  - Recommendation: Validate presence of the validation error type in `state` prior to emission and fail early with a clear diagnostic if missing.
