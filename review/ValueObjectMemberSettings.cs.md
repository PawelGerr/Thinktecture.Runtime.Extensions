Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/ValueObjectMemberSettings.cs

1) Constructor chaining passes constant `true` instead of the parameter (clarity/maintainability)
- Location: Secondary constructor
- Code:
  private ValueObjectMemberSettings(
      bool isExplicitlyDeclared,
      SyntaxReference? equalityComparerAttr,
      string? equalityComparerAccessorType,
      bool hasInvalidEqualityComparerType)
      : this(true)
  {
      _equalityComparerAttr = equalityComparerAttr;
      IsExplicitlyDeclared = isExplicitlyDeclared;
      ...
  }
- Details: The chained call uses `this(true)` regardless of the `isExplicitlyDeclared` argument, only to immediately overwrite the property in the body. This is redundant and confusing, and could become error-prone if initialization logic is later added to the chained constructor that depends on the correct value.
- Impact: No functional bug today, but unnecessary work and increased risk during future refactors.
- Suggested fix: Change `: this(true)` to `: this(isExplicitlyDeclared)` to remove redundant assignment and better reflect intent.

2) Multiple `[MemberEqualityComparer]` attributes: last-one-wins silently
- Location: `Create` method
- Code: Iterates all attributes and assigns `equalityComparerAttr = attribute` on each match without `break`.
- Details: If multiple `MemberEqualityComparer` attributes are applied (misconfiguration), the last one will be used without reporting a problem here. This may be intentional if the analyzer covers this, but at generator state level it silently hides duplicates.
- Impact: Silent override could make misconfigurations harder to diagnose if analyzer is not active or disabled.
- Suggested fix: Either:
  - Break on the first match (and rely on analyzer to report duplicates), or
  - Accumulate matches and let a diagnostic be raised at this stage if more than one is found.

3) Minor null-safety consideration for attribute syntax extraction
- Location: `GetEqualityComparerAttributeLocationOrNull`
- Code: `_equalityComparerAttr?.GetSyntax(cancellationToken).GetLocation();`
- Details: `SyntaxReference.GetSyntax` typically returns a non-null `SyntaxNode`, but the method signature allows for the possibility of issues in edge cases. If it ever returned null, `.GetLocation()` would throw.
- Impact: Very low, but potential NRE in rare scenarios.
- Suggested fix: Make the null-safety explicit:
  ```
  var node = _equalityComparerAttr?.GetSyntax(cancellationToken);
  return node?.GetLocation();
