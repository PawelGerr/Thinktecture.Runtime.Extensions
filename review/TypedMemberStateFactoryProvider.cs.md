Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/TypedMemberStateFactoryProvider.cs

- Missing argument validation:
  - `GetFactoryOrNull(Compilation compilation)` does not validate `compilation`. Misuse (reflection/external caller) would cause NREs in `GetSpecialType`.
  - Recommendation: Add `ArgumentNullException.ThrowIfNull(compilation);`.

- Cross-compilation cache reuse keyed only by runtime major version:
  - Factories are cached globally per-process for versions 7–10 and for other versions in a dictionary keyed by `version = assembly.Identity.Version.Major`. This reuses a factory across different Compilation instances that happen to share the same major version, even if their reference graphs differ.
  - Impact: The underlying `TypedMemberStateFactory` builds token-based caches from the first compilation it sees. Subsequent compilations for the same major version may have different BCL layout or reference metadata (e.g., ref vs runtime assemblies), causing missed cache hits or subtle identity mismatches. It should still fall back to building fresh states but defeats caching efficiency and risks inconsistency.
  - Recommendation: Consider including more identity in the cache key (e.g., assembly MVID(s) of System.Private.CoreLib/System.Runtime), or scope the cache to the particular `Compilation` (e.g., per `Compilation` key via weak table) to avoid cross-compilation mixing.

- Version 0 bucket is ambiguous:
  - If `objSymbol.ContainingAssembly.Identity.Version` is null, `version` remains 0; multiple disparate compilations with unknown versions will share the same factory instance.
  - Recommendation: Treat unknown version as its own cache slot per compilation (e.g., fall back to the dictionary with a unique key derived from compilation identity), or avoid global caching when version is 0.

- Cache growth without eviction:
  - `_factoriesByVersion` grows over time and is never trimmed. While the number of .NET major versions is small, this can still accumulate in long-lived processes with unusual or preview versions.
  - Recommendation: Use a bounded cache or `ConditionalWeakTable` keyed by a compilation identity (or assembly references set) to allow GC.

- Multiple lock objects add complexity:
  - Separate `_lock7/_lock8/_lock9/_lock10` plus a general `_lock` for the dictionary increases complexity without clear benefit.
  - Recommendation: Use a single dictionary `{int -> factory}` with a single lock (or `ConcurrentDictionary`) and optionally `Lazy<TypedMemberStateFactory>` for thread-safe initialization.

- Double-checked locking without volatile:
  - `cachedFactory` is read outside the lock; although the inner lock re-check avoids duplicate initialization, lack of `volatile` can lead to occasional extra locking due to visibility races.
  - Recommendation: Mark the cached fields as `volatile` or use `Lazy<T>` to avoid custom double-checked locking.

- No invalidation when references change:
  - If the host reconfigures references for the same runtime major within the same process (e.g., different target packs), the pre-built factory will remain. This may reduce cache effectiveness or miss types expected by later compilations.
  - Recommendation: Either scope caches more granularly or provide an explicit invalidation path when reference sets change.
