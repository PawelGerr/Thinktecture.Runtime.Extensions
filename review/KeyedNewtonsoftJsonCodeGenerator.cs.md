Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/KeyedNewtonsoftJsonCodeGenerator.cs

- Constructor parameter validation missing:
  - The constructor does not validate `state` or `stringBuilder`. Misuse/reflection could pass null and cause NREs.
  - Recommendation: Add `ArgumentNullException.ThrowIfNull(state);` and `ArgumentNullException.ThrowIfNull(stringBuilder);`.

- Potential missing accessibility on partial type:
  - The generator emits `partial <TypeKind> <Name>` without an explicit accessibility modifier. All partial declarations must match accessibility; omitting it can cause CS0267 if the user-declared partial is `public`/`internal`.
  - Recommendation: Emit the same accessibility as the original declaration (e.g., `public partial ...`) using type metadata.

- Key type resolution can be null and generate invalid code:
  - `keyType` is computed as `customFactory?.TypeFullyQualified ?? _state.KeyMember?.TypeFullyQualified`. If both are null (e.g., missing key member and no custom factory), the emitted attribute generic argument list will include a null/empty type, producing invalid code.
  - Recommendation: Guard against null `keyType` (emit diagnostic and short-circuit) and ensure a valid type is always emitted.

- Serializer framework gating not validated:
  - The code selects a factory matching `SerializationFrameworks.NewtonsoftJson` but does not check `_state.SerializationFrameworks` to confirm Newtonsoft.Json is enabled for this type.
  - Recommendation: Gate emission on `_state.SerializationFrameworks.Has(SerializationFrameworks.NewtonsoftJson)` (or equivalent) to avoid generating code when the framework isn’t requested.

- Multiple factories ambiguity:
  - Uses `FirstOrDefault` to pick an object factory flagged for Newtonsoft.Json. If multiple factories match, selection becomes order-dependent and unspecified.
  - Recommendation: Enforce uniqueness or make selection deterministic (most specific match) and emit a diagnostic on ambiguity.

- Cancellation token ignored:
  - `Generate(CancellationToken)` doesn’t observe `cancellationToken`.
  - Recommendation: Optionally call `cancellationToken.ThrowIfCancellationRequested();` at logical points.

- Converter generic arity coupling:
  - The attribute uses `ThinktectureNewtonsoftJsonConverter<TType, TKey, ValidationError>`. If the converter’s generic arity or parameter order changes, generation will break.
  - Recommendation: Centralize converter type/arity emission in a helper that’s validated against the converter API and covered by tests.

- Namespace hardcoding risk:
  - Uses `global::Thinktecture.Json.ThinktectureNewtonsoftJsonConverter`. If the converter’s namespace changes across package versions, the emitted code won’t compile.
  - Recommendation: Centralize well-known type names in constants or resolve via symbols to ensure correct, versioned names are emitted.

- Defensive assumptions for ValidationError type:
  - Always appends `_state.AttributeInfo.ValidationError`. If this isn’t set for a given target, code generation will fail.
  - Recommendation: Validate presence of the validation error type and fail early with a clear diagnostic if missing.

- Minor formatting:
  - Surrounding newlines around `GENERATED_CODE_PREFIX` and at the end may introduce extra blank lines. Ensure conformity with repo formatting to avoid noisy diffs.
