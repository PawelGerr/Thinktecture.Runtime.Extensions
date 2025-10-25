Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/TypeOnlyComparer.cs

- Identity limited to TypeFullyQualified only:
  - Equality and hashing use only `TypeFullyQualified`. If that representation doesn’t encode all identity aspects (nullability, generic arity/arguments, array rank, pointer/byref), semantically different types might collide.
  - Recommendation: Ensure all involved states produce a canonical `TypeFullyQualified` that includes nullability and generic specifics; otherwise expand the comparer to include additional distinguishing properties.

- Inconsistency with TypeInformationComparer:
  - `TypeInformationComparer` also considers `IsReferenceType`, while this comparer does not. Using these comparers interchangeably can lead to subtle set/dictionary behavior differences.
  - Recommendation: Document intended usage differences. If the same collections may switch comparers, align semantics or make the divergence explicit in naming (e.g., “TypeFqnOnlyComparer”).

- String hashing and equality intent not explicit:
  - Uses `==` on strings and `GetHashCode()` of strings (ordinal, process-randomized). Fine for in-memory collections but unsuitable for persisted/cross-process hashing.
  - Recommendation: Document ordinal semantics. If persisted/deterministic hashing is required anywhere, compute a stable hash over a normalized FQN.

- No null guards in Equals/GetHashCode:
  - Methods assume non-null inputs (matching generic comparer contracts), but misuse could still pass null and cause NRE via `.Type` or `.TypeFullyQualified`.
  - Recommendation: Optionally add defensive `ArgumentNullException.ThrowIfNull(obj);` in GetHashCode and null checks in Equals for robustness.

- High duplication across overloads:
  - The class repeats identical logic for 13 different state types, alternating between `.Type.TypeFullyQualified` and `.TypeFullyQualified`.
  - Recommendation: Reduce duplication with a shared helper, e.g., a small adapter/selector per type: `private static bool EqualsBy(Func<T,string> f, T x, T y) => StringComparer.Ordinal.Equals(f(x), f(y));` and corresponding hash helper. Alternatively, unify the state types behind an interface exposing `TypeFullyQualified` to reuse a single implementation.

- Minor readability improvement:
  - Consider using `StringComparer.Ordinal.Equals(...)` in Equals to make the intended comparison explicit, mirroring the hashing intent.
