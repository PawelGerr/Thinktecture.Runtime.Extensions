Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/KeyedValueObjectSourceGeneratorState.cs

1) Default(struct) allowance ignores nullable struct key
- Location: Property `DisallowsDefaultValue`
- Code: `public bool DisallowsDefaultValue => !IsReferenceType && (!Settings.AllowDefaultStructs || KeyMember.IsReferenceType);`
- Details: For struct value objects with a nullable struct key (e.g., `int?`), `default(TValueObject)` yields a `null` key. Current logic allows default when `AllowDefaultStructs == true` and the key is not a reference type, even if it is a nullable struct. This can lead to invalid default instances with `KeyMember == null`.
- Impact: Generated code that relies on `_state.DisallowsDefaultValue` (e.g., in `KeyedValueObjectCodeGenerator`) may incorrectly allow default instances, causing runtime null dereferences or broken equality/hash semantics.
- Suggested fix: Include nullable-struct key in the disallow condition, e.g.:
  `public bool DisallowsDefaultValue => !IsReferenceType && (!Settings.AllowDefaultStructs || KeyMember.IsReferenceType || KeyMember.IsNullableStruct);`
  Alternatively, if available, use a combined property (e.g., `KeyMember.IsReferenceTypeOrNullableStruct`) to capture both cases.

2) Equality/hash may miss changes that could affect codegen if nullability ever influenced generated output
- Location: `Equals`/`GetHashCode`
- Details: The equality members include many fields but not `NullableAnnotation` and `IsNullableStruct`. While for type declarations these may be stable, if future generator logic starts emitting different code conditioned on these flags, incremental caching might not detect changes.
- Impact: Potentially stale generated code if codegen is ever conditioned on these flags without updating equality/hash accordingly.
- Suggested fix: If these flags are, or become, inputs into codegen decisions, include them in `Equals` and `GetHashCode`. If they are guaranteed not to affect codegen, this item can be ignored.
