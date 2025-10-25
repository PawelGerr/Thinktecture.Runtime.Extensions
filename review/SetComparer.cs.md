Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SetComparer.cs

- Incorrect set equality in presence of duplicates (multiset mismatch):
  - Equality checks only that every element of x is contained in y, and lengths are equal. This fails to detect differing element multiplicities.
    - Example: x = [A, A], y = [A, B]. Lengths equal (2). Loop sees both A&#39;s are contained in y, returns true, but sets differ.
  - Recommendation: Decide on semantics:
    - True set semantics (ignore duplicates): Compare based on distinct elements; build HashSet for both and compare counts and membership.
    - Multiset semantics (respect counts): Build frequency maps for x and y and compare counts for each element.

- Quadratic time complexity:
  - Using y.Contains(x[i]) in a loop is O(n^2).
  - Recommendation: Build a HashSet<T> or Dictionary<T,int> for y (and possibly x) upfront to reduce to O(n) average.

- Extremely weak hashing (massive collisions):
  - GetHashCode returns hash of only the length. Any two sets with the same number of elements will have the same hash, causing very high collision rates and poor dictionary performance.
  - Recommendation:
    - For set semantics: Combine item hashes in an order-independent way, e.g., XOR or addition of item hash codes (with a salt), ideally using a stable combiner (e.g., HashCode.Add with commutative approach).
    - For multiset semantics: Combine item hashes weighted by their counts (e.g., add hash(item) * count or fold count into the combiner).

- No comparer injection:
  - Constrains T to IEquatable<T> and relies on default equality. This prevents alternative equality semantics (e.g., StringComparer.OrdinalIgnoreCase).
  - Recommendation: Provide an overload or constructor accepting IEqualityComparer<T> and use it for Contains/HashSet and hashing.

- Inconsistent default/empty handling:
  - Treats default and empty as equal (reasonable), but the length-based hash returns 0 for default/empty and the same value for any empty sets, which is acceptable; however with improved hashing, maintain a clear mapping for empty sets (e.g., 0).

- Potential allocation patterns:
  - Current algorithm allocates none, but any improved approach will likely allocate a HashSet or Dictionary. Consider amortizing allocations via pooling when used on hot paths, if profiling indicates this is hot.
