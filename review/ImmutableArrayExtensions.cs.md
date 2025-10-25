Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/ImmutableArrayExtensions.cs

1) Definite assignment errors (compile-time)
- newArray and distinctArray locals are declared without initialization and their properties are accessed before any assignment.
  - RemoveAll: ImmutableArray<T> newArray; followed by reads of newArray.IsDefault before any assignment on certain code paths.
  - Where (private overload): ImmutableArray<T> newArray; followed by reads of newArray.IsDefault before any assignment on certain code paths.
  - Distinct: ImmutableArray<T> distinctArray; followed by reads of distinctArray.IsDefault on the first iteration.
- Impact: CS0165 “Use of unassigned local variable” compiler errors.
- Fix: Initialize variables to default explicitly and/or restructure checks to avoid accessing before assignment.
  - Example: ImmutableArray<T> newArray = default; and ImmutableArray<T> distinctArray = default;.
  - Additionally, ensure any conditionals that examine IsDefault occur only after initialization or are guarded by other flags that short-circuit before accessing the variable.

2) Performance: repeated ImmutableArray.Add causes O(n^2) copying in worst case
- In RemoveAll, Where and SelectWhere, once a new array is created, further items are appended via newArray.Add(item), which creates a new array each time.
- Impact: Potential quadratic time and memory churn for longer arrays with many retained items.
- Fix: Prefer ImmutableArray<T>.Builder for incremental construction, then call ToImmutable() at the end. Alternatively, pre-size/copy once and fill into a builder.

3) Inconsistent static lambda usage (minor allocation risk)
- RemoveAll uses elem => elem (non-static) for ImmutableArray.CreateRange, whereas Where uses static elem => elem.
- Impact: While this particular lambda captures nothing, using static consistently removes any risk of inadvertent captures and can avoid delegate allocations in some cases.
- Fix: Use static elem => elem in RemoveAll to align with other usages.

4) Exception type for invalid predicate usage (API/diagnostic quality)
- Private Where overload throws new Exception("Both predicates must not be null") when both predicates are null.
- Impact: Throwing generic Exception reduces diagnostic clarity and deviates from standard BCL patterns.
- Fix: Throw ArgumentNullException or ArgumentException with parameter name details to aid debugging and align with conventions.

5) Hashing of strings uses string.GetHashCode() (determinism concern)
- ComputeHashCode(Func<T, string> selector) uses selector(...)? .GetHashCode() ?? 0.
- Impact: In .NET Core+, string.GetHashCode is randomized per process; results are not stable across runs. If these hashes feed into persistent caches or affect deterministic outputs in source generation, this can lead to non-deterministic behavior.
- Fix: Consider using StringComparer.Ordinal.GetHashCode(s) or a stable hashing approach (e.g., System.Security.Cryptography or a custom non-randomized algorithm) where determinism is required. If determinism is not required, add a comment to document the intended scope.

6) Minor: Seed consistency for ComputeHashCode with selector
- The overload ComputeHashCode<T, TProperty>(Func<T, TProperty> selector) seeds with typeof(T).GetHashCode().
- Impact: Not inherently incorrect, but can reduce hash distribution consistency across differently-typed sequences that are compared by their selected properties.
- Fix: Consider seeding with typeof(TProperty).GetHashCode() or a constant seed to better reflect that hashing is based on the projected property rather than the source element type.
