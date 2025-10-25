Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ComparableCodeGenerator.cs

- Likely incorrect comparer accessor usage:
  - When `_comparerAccessor` is provided, the code emits `[_comparerAccessor].Comparer.Compare(lhs, rhs)`. This assumes the accessor points to a type/object with a `.Comparer` property returning a comparer instance. If the accessor already denotes a comparer instance (e.g., `StringComparer.Ordinal`, `MyComparer.Instance`), appending `.Comparer` produces invalid code (`StringComparer.Ordinal.Comparer` doesn’t exist).
  - Recommendation: Accept a structured `ComparerInfo` (string + IsAccessor flag) or define clear contract for `_comparerAccessor` (e.g., “full comparer expression that exposes Compare”). If the accessor is already the comparer, emit `[_comparerAccessor].Compare(...)`; if it’s a type/factory, resolve the correct member (e.g., `Comparer<T>.Default`) instead of hardcoding `.Comparer`.

- Unsafe cast to IComparable<T> on key member:
  - Fallback branch does `((IComparable<keyType>) this.KeyMember).CompareTo(obj.KeyMember)`. This will not compile if the key type implements only non-generic `IComparable` or neither interface. It also forces a generic interface cast that may not be supported.
  - Recommendation: Use `Comparer<keyType>.Default.Compare(lhs, rhs)` which works for both generic and non-generic `IComparable` implementations and throws early if unsupported. This is the standard, robust approach.

- Hardcoded string comparer semantics:
  - For string keys, the generator uses `StringComparer.OrdinalIgnoreCase`. This may diverge from configured project-wide comparer settings and from equality semantics (which the analyzers enforce via attributes for string keys). Using ignore-case by default can cause subtle mismatches between equality and ordering.
  - Recommendation: Honor explicitly configured key member comparer if one exists; otherwise use `StringComparer.Ordinal` as a safer default. Ideally, the same comparer used for equality should be used for ordering where appropriate.

- Exception message string concatenation/escaping risk:
  - The ArgumentException message in `CompareTo(object?)` embeds a string literal with a runtime-injected type name via `AppendTypeMinimallyQualified(state.Type)`. If this helper emits code that evaluates to a string at runtime, ensure proper concatenation and escaping. If it emits a string literal directly, ensure that the literal is escaped (quotes, generics).
  - Recommendation: Prefer a robust helper for emitting string literals (e.g., a method that generates a properly escaped string literal) or use `typeof(T).Name` within generated code.

- Reference-type null handling asymmetry:
  - For reference types, `CompareTo(T? obj)` returns 1 for null. Ensure this matches the desired ordering contract (nulls last). This is acceptable, but should be consistent across generator families.

- Base types comma/style robustness:
  - `GenerateBaseTypes` appends both `IComparable` and `IComparable<T>`. Ensure the surrounding code correctly inserts commas and line breaks so that conditional base type lists (if any future conditions are added) won’t result in malformed declarations.

- API surface naming consistency:
  - `CodeGeneratorName` and `FileNameSuffix` strings should match naming conventions used in other code generators (hyphens vs spaces, suffix casing). This is minor but helps downstream tooling that consumes names/suffixes.
