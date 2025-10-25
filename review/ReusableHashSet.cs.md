# Review: src/Thinktecture.Runtime.Extensions.SourceGenerator/ReusableHashSet.cs

Status: Reviewed

Summary:
A minimal single-item HashSet pool keyed by comparer instance. Provides Lease/Return APIs to reuse a HashSet with the same IEqualityComparer<T>, bound by a max element count threshold. Implementation is compact and correct for single-threaded use. A few design and performance considerations are noted below.

Strengths:
- Correctly ties cached set to the exact comparer instance via ReferenceEquals, avoiding semantic mismatches that could occur with equal-but-different comparer instances.
- Limits caching by element count (_maxSetSize), preventing retention of very large sets.
- Clears the set on return to avoid leaking elements between leases.
- Simple, easy to reason about, no unnecessary allocation when a cached match exists.

Issues, Risks, and Suggestions:

1) Thread-safety (risk: race conditions if used concurrently)
- The cache field (_cacheWithComparer) is read and written without synchronization. If Lease/Return are called from multiple threads, races are possible (e.g., two concurrent Lease calls both observe non-null and attempt to take the same instance, or Return overwrites another Return).
- If this type is only used from single-threaded contexts (e.g., per-generator execution path), this is acceptable. Otherwise, consider interlocked exchange or a simple lock.

Example (lock-based):
```
private readonly object _gate = new();

public HashSet<T> Lease(IEqualityComparer<T> comparer)
{
    lock (_gate)
    {
        if (_cacheWithComparer is null || !ReferenceEquals(_cacheWithComparer.Value.Comparer, comparer))
            return new HashSet<T>(comparer);

        var set = _cacheWithComparer.Value.Set;
        _cacheWithComparer = null;
        return set;
    }
}

public void Return(HashSet<T> set)
{
    if (set.Count > _maxSetSize)
        return;

    set.Clear();

    lock (_gate)
    {
        _cacheWithComparer = (set, set.Comparer);
    }
}
```

Or interlocked pattern with a nullable local would be possible if you store the tuple in a single reference field (e.g., a small wrapper class) and use Interlocked.Exchange.

2) API visibility and intent (public abstract without abstract members)
- The class is public and abstract, but has no abstract/virtual members. This implies inheritance is required (protected ctor), but there is no customization point. If consumers outside the assembly are not intended to use this, consider making it internal. If inheritance is not required, consider sealed (and public/internal constructor) or provide a static instance per T.
- If the intention is to create per-T specialized pools with preconfigured max sizes, keep it abstract and add a protected constructor as currently, but consider documenting expected subclassing pattern.

3) Capacity retention and memory usage
- Return only checks set.Count, not capacity. HashSet.Clear does not shrink capacity; large internal arrays may be retained and then cached if count ≤ _maxSetSize. This may be desirable (amortized performance) but could also retain more memory than intended for long-lived caches.
- If you want to avoid capacity bloat, consider conditionally calling TrimExcess when returning large sets that still fall under the maxSize threshold.

Example:
```
public void Return(HashSet<T> set)
{
    var count = set.Count;

    if (count > _maxSetSize)
        return;

    set.Clear();

    // Optional: shrink if previously large
    if (count > (_maxSetSize / 2))
        set.TrimExcess();

    _cacheWithComparer = (set, set.Comparer);
}
```

Note: TrimExcess may allocate; measure before adopting.

4) Null-conditional in Lease is redundant after null check
- In `if (_cacheWithComparer is null || !ReferenceEquals(_cacheWithComparer?.Comparer, comparer))`
the `_cacheWithComparer?.Comparer` is redundant because the first disjunct guards null. This is harmless but can be simplified.

Suggested:
```
if (_cacheWithComparer is null || !ReferenceEquals(_cacheWithComparer.Value.Comparer, comparer))
    return new HashSet<T>(comparer);
```

5) Naming aligned with BCL precedent
- BCL pools (ArrayPool) use Rent/Return. Current names Lease/Return are fine, but if consistency with common patterns is valued, consider renaming Lease → Rent.

6) Consider disposal/clearing hooks
- If this cache is long-lived, holding a reference to the comparer instance can keep it rooted. Usually comparer instances are static and cheap, but if custom comparers capture large object graphs, consider exposing a Clear() method to drop the cached pair explicitly, or implement IDisposable on a containing service that clears the cache on shutdown.

7) XML documentation (public API)
- If this remains public, add XML docs to clarify:
  - Only the exact same comparer instance will hit the cache.
  - Not thread-safe unless stated otherwise.
  - Max set size semantics and that capacity is not reduced unless explicitly trimmed.

Minor nit:
- Consider readonly for _maxSetSize (already readonly) and possibly making the tuple type explicit for readability, but the current tuple usage is concise.

Overall Assessment:
- Implementation is correct and efficient for single-threaded scenarios with a single cached set per comparer instance. If there is any chance of concurrent access in the generator pipeline, add synchronization. Consider visibility (internal/sealed) and optional TrimExcess on return depending on memory/perf trade-offs. Add docs to communicate the comparer identity requirement and intended usage.
