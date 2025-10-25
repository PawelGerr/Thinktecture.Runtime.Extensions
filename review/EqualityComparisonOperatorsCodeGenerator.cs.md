Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/EqualityComparisonOperatorsCodeGenerator.cs

- Fragile equality comparer accessor contract:
  - When a custom comparer is provided, the generator emits:
    - `[_equalityComparer.Value.Comparer]` and, if `IsAccessor == true`, appends `.EqualityComparer`, then calls `.Equals(...)`.
  - This assumes the accessor either exposes an `.EqualityComparer` property or that the base expression is not already an `IEqualityComparer<T>`. In common cases (e.g., `StringComparer.Ordinal`, `EqualityComparer<T>.Default`), appending `.EqualityComparer` is invalid. If the provided expression already denotes an `IEqualityComparer<T>`, the correct call is `[expr].Equals(...)` without extra property access.
  - Recommendation: Treat the comparer payload as a full `IEqualityComparer<T>` expression (no property suffixing), or encode intent via a structured type (e.g., `ComparerInfo` semantics refined) and branch accordingly. Avoid hardcoding `.EqualityComparer`.

- Potential mismatch with project-wide string comparer semantics:
  - For string key members, the generator uses `StringComparer.OrdinalIgnoreCase.Equals(...)`. Other parts of the system (e.g., equality for value objects) may mandate explicit comparer configuration (often `Ordinal`). Using ignore-case by default can silently diverge from configured equality semantics and create inconsistent behavior between `==`/`!=` and other equality paths.
  - Recommendation: Prefer an explicitly configured comparer if available (same one used for equality elsewhere). If none, consider `StringComparer.Ordinal` as safer default, or make the behavior configurable.

- Base type interface vs emitted operators: key-type overloads
  - `GenerateBaseTypes` adds `IEqualityOperators<TSelf,TKey,bool>` if `_withKeyTypeOverloads` and `KeyMember != null`. Emission of operator overloads for key-type is correctly guarded in `GenerateKeyOverloads`, but future changes could desynchronize these conditions.
  - Recommendation: Keep the base-type addition and operator emission checks tightly aligned (ideally centralized in one place) to avoid declaring an interface without emitting all required operators.

- Private helper named Equals risks confusion:
  - The private static helper is named `Equals(T? obj, TKey? value)`. While legal, it can be confused with `object.Equals` overloads and may reduce readability in generated code.
  - Recommendation: Rename to something unambiguous like `KeyEquals` or `EqualsKey` to avoid shadowing mental models and improve clarity.

- Reference-type null handling asymmetry (minor):
  - For reference types, the `==` operator implements either reference equality (if `IsEqualWithReferenceEquality`) or defers to `obj.Equals(other)` with proper null checks. In key overloads, null handling is more granular and depends on both `Type.IsReferenceType` and `KeyMember.IsReferenceType`, leading to slightly different paths.
  - Recommendation: This is likely intentional; ensure invariants are documented (e.g., reference-equality intent vs value semantics), and keep behaviors consistent across T–T and T–TKey paths.

- Naming/style consistency:
  - `CodeGeneratorName` and `FileNameSuffix` follow established patterns; ensure tooling relying on these identifiers is aware they vary by generator type. This is informational; no change required.
