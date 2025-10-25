Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/EqualityInstanceMemberInfo.cs

- Missing constructor argument validation:
  - `member` is not validated for null. If a null slips in (reflection/misuse), later calls to `Member.GetHashCode()`/`Member.Equals(...)` will throw NullReferenceException.
  - Recommendation: Add `ArgumentNullException.ThrowIfNull(member);`. Optionally validate/normalize `equalityComparerAccessor` (e.g., null-or-whitespace to null) for stable equality semantics.

- Brittle textual equality for comparer accessor:
  - Equality compares `EqualityComparerAccessor` strings directly. Semantically equivalent accessors can differ textually (e.g., with/without `global::`, whitespace, aliasing), leading to false negatives.
  - Recommendation: Normalize the accessor string (trim, add/remove `global::` consistently) or represent the comparer via a structured model (e.g., `ComparerInfo`) rather than raw text.

- Equality/hash dependence on `InstanceMemberInfo` semantics:
  - Equality and GetHashCode rely on `InstanceMemberInfo`’s value semantics. If `InstanceMemberInfo` changes equality/hash behavior (or defers to components with reference equality), this type’s value semantics become unstable.
  - Recommendation: Ensure `InstanceMemberInfo` maintains strict value-based equality/hash and is covered by tests for stability.

- Null/empty distinction for accessor may be unintended:
  - `null` and `""` are treated as different in equality/hash. If intention is “no custom comparer,” consider normalizing empty/whitespace to null to avoid accidental inequality.
