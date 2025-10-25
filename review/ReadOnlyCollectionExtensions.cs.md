# Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/ReadOnlyCollectionExtensions.cs

- Severity: Warning
  Title: Possible NullReferenceException when `comparer` is null
  Details: The overload `ComputeHashCode<T>(IReadOnlyList<T> collection, IEqualityComparer<T> comparer)` dereferences `comparer` without a null check. If `null` is passed, this will throw at `comparer.GetHashCode(collection[i])`.
  Suggestion:
  - Either add `comparer ??= EqualityComparer<T>.Default;` or add a `[NotNull]` contract/guard.
  - Example fix:
    ```csharp
    public static int ComputeHashCode<T>(this IReadOnlyList<T> collection, IEqualityComparer<T>? comparer)
        where T : IEquatable<T>
    {
        comparer ??= EqualityComparer<T>.Default;
        var hashCode = typeof(T).GetHashCode();
        for (int i = 0, count = collection.Count; i < count; i++)
        {
            hashCode = (hashCode * 397) ^ comparer.GetHashCode(collection[i]);
        }
        return hashCode;
    }
    ```

- Severity: Warning
  Title: Potential NullReferenceException for null elements
  Details: Both the generic (no-comparer) and `string`-specific overloads call `.GetHashCode()` on elements without guarding for `null`. For reference types, lists may contain `null` elements even if the element type is non-nullable in annotations, which will throw.
  Suggestion:
  - If `null` elements are not permitted, add `where T : notnull` to generic methods (C# 8+) and rely on analyzers to enforce it.
  - Otherwise, hash `null` as 0 (consistent with `EqualityComparer<T>.Default.GetHashCode(default)`) or with the comparer:
    ```csharp
    var item = collection[i];
    hashCode = (hashCode * 397) ^ (item is null ? 0 : item.GetHashCode());
    ```
    or for the comparer overload:
    ```csharp
    var item = collection[i];
    hashCode = (hashCode * 397) ^ (item is null ? 0 : comparer.GetHashCode(item));
    ```

- Severity: Warning
  Title: `string`-specific overload ignores comparer semantics and duplicates generic functionality
  Details: `ComputeHashCode(this IReadOnlyList<string> collection)` uses `string.GetHashCode()`, which:
  - Is randomized per-process in .NET (non-deterministic across processes).
  - Forces case-sensitive ordinal semantics and prevents callers from using a custom comparer (e.g., `OrdinalIgnoreCase`) unless they avoid this overload.
  The project’s guidance prefers explicit equality comparers for strings to avoid culture-sensitive or inconsistent comparisons.
  Suggestion:
  - Remove the string-specific overload and use the generic comparer overload (`ComputeHashCode(list, StringComparer.Ordinal)`), or
  - Change implementation to use a well-defined comparer:
    ```csharp
    public static int ComputeHashCode(this IReadOnlyList<string> collection)
    {
        var comparer = StringComparer.Ordinal;
        var hashCode = typeof(string).GetHashCode();
        for (int i = 0, count = collection.Count; i < count; i++)
        {
            hashCode = (hashCode * 397) ^ comparer.GetHashCode(collection[i]);
        }
        return hashCode;
    }
    ```
  - Consider also providing `ComputeHashCode(this IReadOnlyList<string> collection, StringComparer comparer)` for clarity.

- Severity: Info/Low
  Title: Non-deterministic hash seed using `typeof(T).GetHashCode()`
  Details: `Type.GetHashCode()` is not guaranteed to be stable across runtime versions/processes. If these hash codes are used for cross-process determinism (e.g., incremental build keys or cache serialization), this could cause inconsistencies.
  Suggestion:
  - Prefer a constant seed or `HashCode` accumulator and explicitly add the type:
    ```csharp
    var hc = new HashCode();
    hc.Add(typeof(T)); // or a constant
    ...
    return hc.ToHashCode();
    ```
  Note: If used only intra-process (e.g., dictionary keys during a single generator run), this is acceptable.

- Severity: Info/Low
  Title: Overflow context not explicit
  Details: Multiplication by 397 may overflow. This is fine for hash composition, but if the project uses `checked` context globally, this could throw.
  Suggestion:
  - Wrap multiplication in `unchecked` to make intent explicit:
    ```csharp
    hashCode = unchecked((hashCode * 397) ^ elementHash);
    ```

- Severity: Nit
  Title: Minor micro-optimization opportunity
  Details: `collection.Count` is read each loop iteration. Caching it into a local can avoid repeated property access, although JIT often optimizes this.
  Suggestion:
  ```csharp
  for (int i = 0, count = collection.Count; i < count; i++) { ... }
