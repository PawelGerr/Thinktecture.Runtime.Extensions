Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/SmartEnumCodeGenerator.cs

Severity: Warning

1) Nullable return type on ToString override (nullability mismatch)
- Problem: The generator emits:
  public override string? ToString()
  when keyProperty.IsToStringReturnTypeNullable is true. Object.ToString() is non-nullable (string), so this creates a nullability mismatch and produces CS8765 warnings in consumer projects.
- Impact: Consumers see avoidable NRT warnings; inconsistent override signature can also confuse analyzers and users.
- Recommendation:
  - Always emit a non-nullable signature:
      public override string ToString()
    and either:
      - return this.Key.ToString() ?? string.Empty;
      - or return this.Key.ToString()!;
    to satisfy NRT without changing method signature.

2) Unsatisfiable allows ref struct constraints on methods that use Func/Action delegates
- Problem: Methods like:
    public void Switch<TState>(... Action<TState> callback, ...)
    where TState : allows ref struct
  and
    public TResult SwitchPartially<TState, TResult>(... Func<TState, TResult> callback, ...)
    where TState : allows ref struct
    where TResult : allows ref struct
  constrain generic parameters to allow ref struct, but then use them in delegate types (System.Action and System.Func) that do not support byref-like types as generic arguments.
- Impact: These constraints cannot be satisfied at call sites (e.g., Span<T>), causing user confusion and compile failures when attempting to pass ref struct types. The constraints bring no benefit while the parameter shapes are delegates.
- Recommendation:
  - Remove the allows ref struct constraints from these methods, or
  - Provide alternative overloads that do not use heap-allocated delegates (e.g., function pointers delegate* or specialized APIs) if ref struct support is truly intended. As-is, the constraints are misleading and unusable.

3) Potentially surprising equality semantics (reference equality only)
- Problem: IEquatable<T>.Equals(T other) returns ReferenceEquals(this, other). GetHashCode is based on the key for keyed enums.
- Impact: While singletons make reference equality viable, this is surprising for smart-enum consumers expecting value/key-based equality. Also, the hash code uses key semantics while Equals uses reference semantics (permitted, but counterintuitive).
- Recommendation:
  - Confirm intended contract. If value semantics are desired, compare by key (and keep cached hash based on key). If reference semantics are intended, this is acceptable but should be consistent across operators and documentation.

4) O(n) lookup to compute item index on first access
- Problem: _itemIndex is initialized via linear scan over Items comparing by reference to find the current item index.
- Impact: For large enums this is O(n) per instance (first access). There is a code-size safeguard for switch/map generation at 1000 items, but _itemIndex linearization still remains.
- Recommendation:
  - During lookups construction, also compute and store a dictionary from item reference to index or embed index in the item metadata to allow O(1) initialization.

Severity: Minor

5) Dead generator method GenerateEnsureValid
- Problem: GenerateEnsureValid(IMemberState keyProperty) is defined but never invoked, so no emitted code. It also references IsValid, which could become fragile if enabled accidentally.
- Impact: Maintenance overhead; risk of stale/incorrect code if later enabled without full context.
- Recommendation: Remove or wire it up intentionally with tests.

6) Typos in XML docs
- Problem: XML docs say “Item to covert.” and “Value to covert.”
- Impact: Minor quality issue; leaks into consumer IntelliSense.
- Recommendation: Fix to “convert.”
