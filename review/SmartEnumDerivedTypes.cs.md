SmartEnumDerivedTypes.cs — Review (issues only)

Summary
- Scope: src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/SmartEnumDerivedTypes.cs
- Focus: nullability, immutability/defensive copies, equality/hash stability, incremental determinism.

Issues

1) No defensive copy of list inputs (immutability leak)
- Problem: ContainingTypes and DerivedTypesFullyQualified are assigned directly from constructor parameters. Even though properties are IReadOnlyList<>, the incoming instances can be mutable (e.g., List<T> referenced as IReadOnlyList<T>) and be mutated after construction.
- Impact: High for incremental model correctness. Equality and hash code depend on these lists; external mutation can change object identity while it is used as a key in caches/dictionaries, leading to hard-to-debug cache corruption and non-deterministic generator behavior.
- Fix: Snapshot inputs on construction:
  - For value types/strings: DerivedTypesFullyQualified = derivedTypesFullyQualified?.ToArray() ?? Array.Empty<string>();
  - For complex types: ContainingTypes = containingTypes?.ToArray() ?? Array.Empty<ContainingTypeState>();
  Consider switching property types to ImmutableArray<T> to make intent explicit.

2) Potential null element handling in lists
- Problem: Code assumes list elements are non-null. If a null sneaks into DerivedTypesFullyQualified or ContainingTypes, Equals (SequenceEqual) is fine for strings but GetHashCode uses DerivedTypesFullyQualified.ComputeHashCode(StringComparer.Ordinal) which may not handle nulls (depends on extension). ContainingTypes.ComputeHashCode() may also not handle nulls.
- Impact: Medium. Unexpected null entries can cause NullReferenceException during hashing.
- Fix: Enforce non-null elements when snapshotting:
  - Validate inputs (throw SourceGenException or sanitize by filtering nulls).
  - Alternatively, ensure ComputeHashCode extensions handle null elements gracefully.

3) Order-sensitive equality and hashing for DerivedTypesFullyQualified
- Problem: Equals uses SequenceEqual and GetHashCode uses sequence-based hashing, making equality order-sensitive. If the source of derived types is not strictly ordered (e.g., symbol enumeration order), instances that are logically equivalent but differently ordered will not be equal and will produce different hashes.
- Impact: Medium. Can cause non-deterministic incremental cache invalidations between compilations and across machines.
- Fix: Normalize order deterministically (Ordinal sort) during construction:
  ```
  var derived = (derivedTypesFullyQualified ?? Array.Empty<string>()).OrderBy(s => s, StringComparer.Ordinal).ToArray();
  DerivedTypesFullyQualified = derived;
  ```

4) String hashing consistency and explicitness
- Problem: GetHashCode uses string.GetHashCode for Namespace, Name, and TypeFullyQualified. Other code paths (for collections) already use StringComparer.Ordinal. Using string.GetHashCode is process-randomized but consistent within a process; still, mixing styles is inconsistent and less explicit.
- Impact: Low. Prefer explicit ordinal hashing for consistency and clarity.
- Fix: Use StringComparer.Ordinal for all string hashing:
  ```
  var sc = StringComparer.Ordinal;
  hashCode = (hashCode * 397) ^ (Namespace is null ? 0 : sc.GetHashCode(Namespace));
  hashCode = (hashCode * 397) ^ sc.GetHashCode(Name);
  hashCode = (hashCode * 397) ^ sc.GetHashCode(TypeFullyQualified);
  ```

5) Missing validation for required non-null parameters
- Problem: Name, TypeFullyQualified, ContainingTypes, and DerivedTypesFullyQualified are non-nullable but not validated. If any are null, Equals/GetHashCode will throw.
- Impact: Medium. While NRT helps at compile-time, values may still come from Roslyn analysis; defensive checks improve robustness.
- Fix: Guard in constructor (throw SourceGenException or use defaults):
  ```
  if (name is null) throw new ArgumentNullException(nameof(name));
  if (typeFullyQualified is null) throw new ArgumentNullException(nameof(typeFullyQualified));
  containingTypes ??= Array.Empty<ContainingTypeState>();
  derivedTypesFullyQualified ??= Array.Empty<string>();
  ```

6) Equality for ContainingTypes depends on ContainingTypeState.Equals
- Problem: SequenceEqual for ContainingTypes relies on ContainingTypeState providing value-based equality. If ContainingTypeState does not implement IEquatable/override Equals, this will degrade to reference equality.
- Impact: Medium if ContainingTypeState lacks value equality; otherwise no issue.
- Fix: Ensure ContainingTypeState implements structural equality and GetHashCode aligned with its usage here. Alternatively, provide an explicit comparer to SequenceEqual and ComputeHashCode if available.

Suggested Tests
- Defensive copy test:
  - Pass mutable lists into constructor; mutate original lists afterward; assert Equals/GetHashCode of the instance do not change.
- Deterministic ordering:
  - Construct with derived types in different orders; assert instances are equal and hashes match (after applying order normalization).
- Null element handling:
  - Attempt constructing with null elements in lists; ensure either an exception is thrown or nulls are filtered and hashing does not throw.
- String hashing consistency:
  - Verify GetHashCode stability across identical inputs using explicit StringComparer.Ordinal.
- Structural equality of ContainingTypeState:
  - Create two different ContainingTypeState instances with the same values; ensure SmartEnumDerivedTypes equality treats them as equal.

Risk Assessment
- High: Immutability leak via list references can break incremental caches.
- Medium: Order-sensitivity may cause non-deterministic equality/hashing if upstream order is not normalized.
- Medium: Missing parameter/element validation can NRE during hashing.
- Low: String hashing explicitness.

Proposed Patch (condensed)
```
public SmartEnumDerivedTypes(
   string? ns,
   string name,
   string typeFullyQualified,
   bool isReferenceType,
   IReadOnlyList<ContainingTypeState> containingTypes,
   IReadOnlyList<string> derivedTypesFullyQualified)
{
   if (name is null) throw new ArgumentNullException(nameof(name));
   if (typeFullyQualified is null) throw new ArgumentNullException(nameof(typeFullyQualified));

   Namespace = ns;
   Name = name;
   TypeFullyQualified = typeFullyQualified;
   IsReferenceType = isReferenceType;

   var containing = (containingTypes ?? Array.Empty<ContainingTypeState>()).ToArray();
   var derived = (derivedTypesFullyQualified ?? Array.Empty<string>())
                 .Where(static s => s is not null)
                 .OrderBy(static s => s, StringComparer.Ordinal)
                 .ToArray();

   ContainingTypes = containing;
   DerivedTypesFullyQualified = derived;
}

public override int GetHashCode()
{
   unchecked
   {
      var sc = StringComparer.Ordinal;
      var hashCode = Namespace is null ? 0 : sc.GetHashCode(Namespace);
      hashCode = (hashCode * 397) ^ sc.GetHashCode(Name);
      hashCode = (hashCode * 397) ^ sc.GetHashCode(TypeFullyQualified);
      hashCode = (hashCode * 397) ^ IsReferenceType.GetHashCode();
      hashCode = (hashCode * 397) ^ DerivedTypesFullyQualified.ComputeHashCode(sc);
      hashCode = (hashCode * 397) ^ ContainingTypes.ComputeHashCode();
      return hashCode;
   }
}
