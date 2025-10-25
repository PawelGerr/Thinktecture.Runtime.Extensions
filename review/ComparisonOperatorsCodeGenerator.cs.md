Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ComparisonOperatorsCodeGenerator.cs

- Incorrect ArgumentNullException.ThrowIfNull usage:
  - The generated null checks use `ArgumentNullException.ThrowIfNull(nameof(left));` and `ThrowIfNull(nameof(right));` which pass a string name instead of the actual variable.
  - This will always succeed (non-null string) and will not guard against null values, defeating the purpose.
  - Recommendation: Emit `ArgumentNullException.ThrowIfNull(left);` (and `right`) instead.

- Fragile comparer accessor contract (hardcoded “.Comparer”):
  - Code paths that use a custom comparer emit `[{accessor}].Comparer.Compare(...)`. If the accessor already represents an `IComparer<T>` (e.g., `StringComparer.Ordinal` or `MyComparer.Instance`), appending `.Comparer` is invalid.
  - Same issue appears in both main- and key-type-overload generations.
  - Recommendation: Use a structured `ComparerInfo` (string + IsAccessor) or otherwise define the contract precisely. If the accessor is the comparer, emit `accessor.Compare(...)`; if it’s a factory/type holder, resolve the correct property or field instead of always appending `.Comparer`.

- Hardcoded string comparison semantics:
  - String-based comparisons use `StringComparer.OrdinalIgnoreCase`. This may diverge from equality semantics configured elsewhere (e.g., project-specific string comparer for value objects), causing ordering/equality inconsistencies.
  - Recommendation: Prefer the explicitly configured comparer for the key if available. If none, consider `StringComparer.Ordinal` as a safer default; or ensure equality and ordering use the same comparer configuration.

- Interface implementation vs operator emission might diverge:
  - `GenerateBaseTypes` adds `IComparisonOperators<T,T,bool>` (and optionally `<T, TKey, bool>`) when using a comparer or for string keys, regardless of actual key-type operators being present (which is fine since comparisons are done via comparer). However, if key-type operators are partially available and `withKeyTypeOverloads` is true, ensure all four operator overloads required by the interface are emitted to avoid interface implementation gaps.
  - Current code guards by not adding base types when operators are incomplete and no comparer/string; this is correct. Keep this invariant tightly coupled with operator emission to avoid future drift.

- Null-checks for nullable value-like key members:
  - Null checks for key-type overloads use `IsReferenceType` to determine whether to emit `ThrowIfNull(...)`. For `Nullable<T>` key members (not reference types), null values are possible but currently unchecked.
  - Recommendation: If `Nullable<T>` is a valid key type here, consider handling nulls explicitly or document that nullable keys are unsupported for comparison operator generation.

- Minor formatting robustness:
  - The `_LEFT_NULL_CHECK` and `_RIGHT_NULL_CHECK` strings include trailing indentation and newlines. While harmless, ensure these snippets align with surrounding formatting to avoid odd whitespace in generated code across different cases.

- Naming/style consistency:
  - `CodeGeneratorName`/`FileNameSuffix` follow the same pattern as other generators; keep these consistent and documented if external tools rely on them.
