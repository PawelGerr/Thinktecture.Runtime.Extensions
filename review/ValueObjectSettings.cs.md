Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/ValueObjectSettings.cs

1) Missing passthrough properties required by generators (compile-time break)
- Location: Type `ValueObjectSettings`
- Details: `ValueObjectSourceGenerator` accesses the following properties on `state.Settings`:
  - SkipIComparable, SkipIParsable, SkipIFormattable
  - ComparisonOperators, EqualityComparisonOperators
  - AdditionOperators, SubtractionOperators, MultiplyOperators, DivisionOperators
  These properties are defined on `AllValueObjectSettings` and used by generator pipelines, but are not exposed by this wrapper struct. Current file only exposes a subset (e.g., ConversionToKeyMemberType, SkipToString, etc.).
- Impact: References like `state.Settings.SkipIFormattable` and `state.Settings.ComparisonOperators` will not compile or will break if this wrapper is the only path to settings within states (as seen in ValueObjectSourceGenerator).
- Suggested fix: Add passthrough properties to `ValueObjectSettings`:
  - `public bool SkipIComparable => _allSettings.SkipIComparable;`
  - `public bool SkipIParsable => _allSettings.SkipIParsable;`
  - `public bool SkipIFormattable => _allSettings.SkipIFormattable;`
  - `public OperatorsGeneration ComparisonOperators => _allSettings.ComparisonOperators;`
  - `public OperatorsGeneration EqualityComparisonOperators => _allSettings.EqualityComparisonOperators;`
  - `public ImplementedOperators AdditionOperators => _allSettings.AdditionOperators;`
  - `public ImplementedOperators SubtractionOperators => _allSettings.SubtractionOperators;`
  - `public ImplementedOperators MultiplyOperators => _allSettings.MultiplyOperators;`
  - `public ImplementedOperators DivisionOperators => _allSettings.DivisionOperators;`
  Also update `Equals/GetHashCode` accordingly.

2) Potential null-dereference in GetHashCode for string properties
- Location: `GetHashCode` uses `.GetHashCode()` on string properties: `CreateFactoryMethodName`, `TryCreateFactoryMethodName`, `DefaultInstancePropertyName`
- Details: These are typed as non-nullable `string`, but rely on `_allSettings` to guarantee non-null at runtime. If upstream defaults ever change or attribute parsing returns null (e.g., due to misuse), this will throw at runtime during incremental hashing.
- Impact: Hard crash during generator execution in edge cases.
- Suggested fix: Either:
  - Keep strong non-null contract but assert (Debug.Assert) non-null at construction-time, or
  - Use null-conditional in hash computation similar to `KeyMemberEqualityComparerAccessor`, e.g. `(CreateFactoryMethodName?.GetHashCode() ?? 0)`, if a defensive approach is preferred. Ensure consistency with `Equals` semantics.
