Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/SmartEnumCodeGeneratorFactory.cs

Severity: Warning

1) Brittle equality semantics (reference-only)
- Problem: Equals(ICodeGeneratorFactory<SmartEnumSourceGeneratorState> other) returns ReferenceEquals(this, other). This makes two logically identical factories unequal if they are different instances (e.g., instantiated via reflection, deserialization, or separate DI scopes).
- Impact: Can break incremental generator deduplication or caching that expects factories of the same kind to be considered equal by value (type), not by reference. Leads to unnecessary re-generation and reduced cache hits.
- Recommendation:
  - Compare by type rather than instance:
    public bool Equals(ICodeGeneratorFactory<SmartEnumSourceGeneratorState>? other)
        => other is SmartEnumCodeGeneratorFactory;

2) Inconsistent equality contract with object.Equals/GetHashCode
- Problem: The class provides a typed Equals but does not override object.Equals or GetHashCode. Collections or algorithms using object equality (e.g., HashSet, Dictionary keys) will not respect the intended equality semantics, causing subtle bugs if these factories are ever used as keys.
- Impact: Potential mismatches between interface-based comparisons and object-based comparisons, causing duplicate entries or cache misses.
- Recommendation:
  - If equality is by type (recommended), also override object.Equals and GetHashCode consistently:
    public override bool Equals(object? obj) => obj is SmartEnumCodeGeneratorFactory;
    public override int GetHashCode() => typeof(SmartEnumCodeGeneratorFactory).GetHashCode();

Notes
- If the intended design is strict reference identity, ensure every call site consistently uses the static Instance and does not construct new instances. However, value-by-type equality is typically safer and more resilient in incremental generator pipelines.
