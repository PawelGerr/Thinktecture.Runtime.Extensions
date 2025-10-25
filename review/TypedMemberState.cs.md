Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/TypedMemberState.cs

- Missing constructor argument validation:
  - The constructor doesn’t validate `type`. If null leaks in (reflection/misuse), dereferencing will throw immediately.
  - Recommendation: Add `ArgumentNullException.ThrowIfNull(type);`.

- IsReferenceTypeOrNullableStruct incorrectly treats type parameters:
  - Property computes `IsReferenceTypeOrNullableStruct => IsReferenceType || IsNullableStruct || (IsTypeParameter && !IsStruct)`. For type parameters constrained to `struct`, `IsTypeParameter` is true and `IsStruct` is false (Roslyn does not mark type parameters as value types), so the expression yields true even though `T : struct` is neither reference type nor nullable struct.
  - Recommendation: If `type` is `ITypeParameterSymbol`, inspect constraints (`HasReferenceTypeConstraint`, `HasValueTypeConstraint`, `ReferenceTypeConstraintNullableAnnotation`) and compute this property accordingly.

- Incomplete equality/hash: `NullableAnnotation` not considered:
  - `Equals`/`GetHashCode` include many fields but omit `NullableAnnotation`, despite exposing it publicly. Two states with different nullability annotations will compare equal and hash the same.
  - Recommendation: Include `NullableAnnotation` in both equality and hash code.

- Potentially inaccurate IsNullableStruct detection:
  - Calculation: `type.OriginalDefinition.SpecialType == System_Nullable_T || (type.IsValueType && type.NullableAnnotation == Annotated)`. The second branch can misclassify some symbols (value types typically represent `Nullable<T>` rather than a value type with `Annotated`). This path may never legitimately be needed and could mask issues.
  - Recommendation: Prefer a stricter check: `type.OriginalDefinition.SpecialType == System_Nullable_T` or `type is INamedTypeSymbol { ConstructedFrom.SpecialType: System_Nullable_T }`. Remove or carefully justify the `IsValueType && Annotated` branch.

- IsToStringReturnTypeNullable detection is brittle:
  - Implementation returns on the first `ToString` member encountered and defaults to `true` if none are found. The member array order isn’t guaranteed; you may inspect a non-parameterless overload first, yielding a misleading result. Defaulting to `true` implies nullable returns by default, which is likely incorrect (standard `ToString()` returns non-null).
  - Recommendation: Specifically select the parameterless `ToString()` override if present; otherwise prefer a conservative default of `false`. If multiple overloads exist, only use the parameterless result for this property.

- Equality limited to concrete implementation:
  - `Equals(ITypedMemberState?)` returns true only if the runtime type is `TypedMemberState`. If cross-implementation equality is intended for `ITypedMemberState`, this is too strict.
  - Recommendation: Clarify interface equality semantics. If cross-type equality is desired, compare interface-defined identity fields instead of requiring `TypedMemberState`.

- String-based identities and randomized hash:
  - `TypeFullyQualified`/`TypeMinimallyQualified` are derived using display formats; while suitable for codegen, these strings can vary with context/configuration. Hashing uses `string.GetHashCode()` (randomized per process).
  - Recommendation: Ensure `ToFullyQualifiedDisplayString()` yields a canonical, stable representation. If hashes are persisted, compute a deterministic hash over a normalized representation.
