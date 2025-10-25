# Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/BackingFieldName.cs

## Warnings

1) Nullable reference invariants not enforced (permits null state)
- Details: The readonly struct has non-nullable fields `string Name` and `string PropertyName` but:
  - `default(BackingFieldName)` yields both fields as null.
  - `Create(string name, string propertyName)` performs no null validation.
  - Methods account for nulls (`Name ?? string.Empty`, null-safe `GetHashCode`), implying null is expected.
- Risk: Inconsistent invariants and potential misuse when used as keys in collections or during logging/debugging.
- Suggestions:
  - Enforce non-null invariants at construction: null-check inputs in `Create` and/or make fields/properties nullable (`string?`) to reflect reality.
  - If default is not a valid state, consider a class, or a struct with an explicit “invalid” sentinel and `HasValue` semantics.

2) Public fields instead of get-only properties
- Details: Exposes `public readonly string Name;` and `public readonly string PropertyName;`.
- Risk: Diverges from project style (public properties preferred), and complicates future validation/normalization hooks.
- Suggestion: Replace with get-only properties and keep constructor/factory as the single mutation point.

3) ToString swallows invalid state
- Details: `ToString` returns `Name ?? string.Empty`.
- Risk: Hides invalid/null state, making diagnostics harder.
- Suggestion: If non-null is enforced, simply return `Name`. Otherwise, return a more explicit representation or include `PropertyName` to aid debugging.

4) Equality and hashing tolerate nulls instead of preventing them
- Details: `Equals` and `GetHashCode` both handle nulls gracefully.
- Risk: Default/null instances can end up in sets/dictionaries with silent behavior; mixing valid and default instances may be surprising.
- Suggestions:
  - If nulls are invalid, enforce at construction to simplify `Equals`/`GetHashCode`.
  - If nulls are valid, consider documenting and making the nullability explicit (`string?`) to match the actual domain.

5) Missing DebuggerDisplay for diagnostics
- Details: No `[DebuggerDisplay]` attribute.
- Suggestion: Add `[DebuggerDisplay("{Name} ({PropertyName})")]` (or similar) to improve debugging of generator state.

6) Consider explicit StringComparison semantics
- Details: String equality uses `==` which maps to ordinal comparison; intent is implicit.
- Suggestion: If ordinal semantics are required by design, consider using `string.Equals(Name, other.Name, StringComparison.Ordinal)` for clarity and consistency with repo guidance on explicit comparers for strings.

7) Optional: Consider record struct or sealed class
- Details: Equality members are manually implemented.
- Suggestion: If non-null invariants and reference semantics are preferable, a sealed class may simplify default-state concerns. Alternatively, a `readonly record struct` can reduce boilerplate if semantics match requirements.
