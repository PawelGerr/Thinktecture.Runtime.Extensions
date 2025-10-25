Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/AdditionOperatorsCodeGenerator.cs

1) Critical correctness bug: Null-checks pass parameter names instead of arguments
- Code:
  - _LEFT_NULL_CHECK = "global::System.ArgumentNullException.ThrowIfNull(nameof(left));"
  - _RIGHT_NULL_CHECK = "global::System.ArgumentNullException.ThrowIfNull(nameof(right));"
- Problem: ArgumentNullException.ThrowIfNull expects the argument instance, not the parameter name. Passing nameof(left/right) checks a non-null string and will never throw, allowing null to flow and cause NullReferenceException on left/right.Key.
- Fix:
  - Use ThrowIfNull(left); and ThrowIfNull(right); (optionally pass paramName as second arg).
  - Example:
    - const string _LEFT_NULL_CHECK = @"global::System.ArgumentNullException.ThrowIfNull(left, nameof(left));
      ";
    - const string _RIGHT_NULL_CHECK = @"global::System.ArgumentNullException.ThrowIfNull(right, nameof(right));
      ";

2) Naming typo reduces readability and risks copy-paste errors
- Code:
  - var typeLightNullCheck = state.Type.IsReferenceType ? _RIGHT_NULL_CHECK : null;
  - GenerateOverloadsForKeyType(..., string? typeLightNullCheck, ...)
- Problem: Should be typeRightNullCheck, not typeLightNullCheck. This typo is repeated, making the intent less clear and increasing maintenance risk.
- Fix:
  - Rename all occurrences of typeLightNullCheck to typeRightNullCheck in this file.

3) Potentially incorrect interface implementation gating (design)
- Code:
  - GenerateBaseTypes returns early unless _keyMemberOperators.HasOperator(ImplementedOperators.All)
- Problem: IAdditionOperators<TSelf, TOther, TResult> requires only the default operator implementation. With current gating, if only ImplementedOperators.Default is available, the type won&#39;t implement IAdditionOperators<T,T,T> (and/or IAdditionOperators<T, TKey, T>) despite having the operator. This may break generic math scenarios that rely on these interfaces.
- Suggested change:
  - Implement IAdditionOperators when Default is available:
    - if (_keyMemberOperators.HasOperator(ImplementedOperators.Default)) append IAdditionOperators<T, T, T>.
    - Only add checked operator methods when ImplementedOperators.Checked is also available (current behavior).
  - If the intent is to require both checked and unchecked to implement the interface, add a comment explaining the rationale and ensure this is consistent across other arithmetic generators.

4) Possible operator ambiguity when ConversionFromKeyMemberType is Implicit
- Scenario: If the value object has an implicit conversion from key type to the VO, and key-type overloads are enabled, expressions like key + vo may have two applicable candidates: 
  - (Key + Type) defined operator
  - via implicit conversion: (Type + Type) operator
- Risk: Could produce ambiguous call errors depending on overload resolution and exact conversions generated.
- Recommendation:
  - Verify with tests covering both directions (VO + Key and Key + VO) under implicit conversion settings.
  - If ambiguity occurs, consider:
    - Making one of the conversions explicit by default when key-type addition overloads are emitted, or
    - Omitting one of the symmetric operators when an implicit conversion exists, or
    - Use operator constraints/priority comments to document intended behavior.

5) Consistency/style: Mixed qualification of BCL types
- Code:
  - throw new ArgumentOutOfRangeException(...) is unqualified, whereas ArgumentNullException is fully-qualified as global::System...
- Impact: Minor. If implicit usings are disabled or differ across targets, this may fail to compile. Consistency helps avoid surprises.
- Fix:
  - Either add using System; at top or use global::System.ArgumentOutOfRangeException consistently.

Additional notes
- XML inheritdoc cref points to op_Addition(TSelf, TOther) for all overloads, including checked variants. This is acceptable but ensure build produces no XML doc warnings for duplicates.
- The overall append/cast pattern appears correct assuming AppendCast writes only the cast and not additional parentheses; parenthesis balancing results in: Create((Type)(left.Key + right.Key)) when casting; Create((left.Key + right.Key)) otherwise. Please double-check AppendCast for this contract.

Proposed test coverage (to prevent regressions)
- Reference type VO with null left/right: verify ArgumentNullException is thrown (after fixing bug #1).
- Value type VO (struct): verify no null checks are emitted and operators work.
- Default-only underlying key operators: verify VO implements IAdditionOperators<T,T,T> (or document otherwise).
- With withKeyTypeOverloads=true:
  - VO + Key and Key + VO both compile and produce expected results.
  - With implicit ConversionFromKeyMemberType: ensure no ambiguity for VO + Key and Key + VO.
- Checked operator variants: verify overflow behavior is enforced when checked context operator is used.
