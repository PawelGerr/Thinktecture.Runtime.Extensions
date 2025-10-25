Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/KeyedMessagePackCodeGenerator.cs

- Constructor parameter validation missing:
  - The constructor does not validate `state` or `stringBuilder`. Misuse/reflection could pass null and cause NREs.
  - Recommendation: Add `ArgumentNullException.ThrowIfNull(state);` and `ArgumentNullException.ThrowIfNull(stringBuilder);`.

- Potential missing accessibility on partial type:
  - The generator emits `partial <TypeKind> <Name>` without an explicit accessibility modifier. All partial declarations must match accessibility; omitting it can cause CS0267 if the user-declared partial is `public`/`internal`.
  - Recommendation: Emit the same accessibility as the original declaration (e.g., `public partial ...`) using type metadata.

- Key type resolution can be null and generate invalid code:
  - `keyType` is derived as `customFactory?.TypeFullyQualified ?? _state.KeyMember?.TypeFullyQualified`. If both are null (e.g., missing key member and no custom factory), `keyType` is null; the attribute then emits `...Formatter< TType, {null}, ValidationError >`, producing malformed code.
  - Recommendation: Guard against null `keyType` (throw a diagnostic or fail early) and ensure a valid type name is emitted.

- Serializer framework gating not validated:
  - The code selects a factory for `SerializationFrameworks.MessagePack` but does not validate whether `_state.SerializationFrameworks` actually includes MessagePack. If the framework is not requested/available, the generator still emits the attribute.
  - Recommendation: Check `_state.SerializationFrameworks.Has(SerializationFrameworks.MessagePack)` (or equivalent) to conditionally emit code.

- Multiple factories ambiguity:
  - Uses `FirstOrDefault` to select an object factory with MessagePack flag. If multiple factories match, the outcome is order-dependent and unspecified.
  - Recommendation: Enforce uniqueness or choose deterministically; otherwise emit a diagnostic for ambiguity.

- Cancellation token ignored:
  - `Generate(CancellationToken)` does not observe `cancellationToken`. While the method is short, ignoring cancellation can be problematic in larger generators.
  - Recommendation: Optionally call `cancellationToken.ThrowIfCancellationRequested();` at logical points.

- Attribute generic arity coupling:
  - The formatter selection uses `_state.Type.IsReferenceType ? ThinktectureMessagePackFormatter : ThinktectureStructMessagePackFormatter` and always supplies `<TType, TKey, ValidationError>`. If the formatter generic arity or parameter order changes in the formatter API, this code will silently break.
  - Recommendation: Centralize formatter selection and generic argument emission in a helper that is validated against the formatter API; consider compile-time tests or analyzer checks.

- Namespace hardcoding risk:
  - Uses `global::Thinktecture.Formatters` for formatter types. If the formatter namespace changes across versions/packages, the emitted code breaks.
  - Recommendation: Centralize well-known type names in a constants/provider and/or detect presence via type symbols to ensure correct names are emitted.

- Minor formatting considerations:
  - `GENERATED_CODE_PREFIX` is followed by an extra newline and additional appended newlines at the end. Ensure this matches repository formatting conventions to avoid unnecessary diffs.
