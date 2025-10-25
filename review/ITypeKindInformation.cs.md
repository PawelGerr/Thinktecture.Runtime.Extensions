Issues found in ITypeKindInformation.cs

1) Warning: Ambiguity between struct vs. value type semantics
- Problem: The property is named `IsStruct`, but Roslyn terminology and many APIs use “value type” semantics (`ITypeSymbol.IsValueType`). It&#39;s unclear whether `IsStruct` is meant to indicate:
  - any value type (incl. enums and record structs), or
  - specifically a user-declared struct (excluding enums), or
  - "non-nullable value type" semantics.
- Risk: Callers may misinterpret the flag and implement inconsistent checks.
- Fix options:
  - Rename to `IsValueType` if the intent is value-type semantics.
  - If the intent is "declared as struct" (excluding enums), consider adding a separate flag (`IsStructDeclaration`) and also exposing `IsValueType`.

2) Warning: Record classification overlap
- Problem: `IsRecord` doesn&#39;t distinguish between record class and record struct. In combination with `IsStruct` and `IsReferenceType`, invariants are not specified.
- Risk: Implementations and consumers may disagree on expected combinations (e.g., should a record struct set both `IsRecord == true` and `IsStruct == true`?).
- Fix options:
  - Add dedicated flags `IsRecordClass` and `IsRecordStruct`, or
  - Replace with an enum `RecordKind { None, Class, Struct }`, or
  - Clearly document invariants (e.g., "IsRecord && IsStruct implies record struct; IsRecord && IsReferenceType implies record class").

3) Warning: Booleans cannot represent "unknown" for type parameters
- Problem: For type parameters without constraints the kind (reference vs value) is unknown at compile-time. The boolean shape (`IsReferenceType`, `IsStruct`) cannot express "unknown" state.
- Risk: Incorrect assumptions in generators for unconstrained or partially constrained type parameters.
- Fix options:
  - Use tri-state (`bool? IsReferenceType`, `bool? IsValueType`) where `null` means unknown, or
  - Expose constraint-centric properties like `HasReferenceTypeConstraint`, `HasValueTypeConstraint`, `HasUnmanagedTypeConstraint`, etc., or
  - Provide a `TypeParameterConstraintKind`/flags model and specify how the other properties should be interpreted when `IsTypeParameter == true`.

4) Warning: Potential completeness gaps for common kind checks
- Problem: The interface does not expose `IsInterface` and `IsEnum`, which are common discriminators used in source generation. Callers may need to re-query Roslyn symbols elsewhere, fragmenting logic.
- Risk: Inconsistent or duplicated checks outside this abstraction.
- Fix options:
  - Add `IsInterface` and `IsEnum`, or
  - Document that these checks are intentionally out of scope and how callers should obtain them consistently.

5) Warning: Invariants between properties are not defined
- Problem: The interface doesn&#39;t specify exclusivity or relationships, e.g.:
  - Can both `IsReferenceType` and `IsStruct` be `true`? (should be mutually exclusive)
  - What are expected values when `IsTypeParameter == true`?
- Risk: Divergent implementations and subtle bugs in code generation logic.
- Fix options:
  - Define explicit invariants in XML docs (e.g., mutual exclusivity, implications with records and type parameters).
  - Consider replacing the booleans with a single "kind" enum (e.g., `TypeKindInfo { Unknown, ReferenceType, Struct, RecordClass, RecordStruct, TypeParameter, ... }`) to make invalid combinations unrepresentable.
