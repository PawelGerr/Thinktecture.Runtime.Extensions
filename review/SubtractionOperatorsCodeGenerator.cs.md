Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/SubtractionOperatorsCodeGenerator.cs

1) Critical bug: ineffective null checks (reference types)
- Problem:
  - The constants use ThrowIfNull(nameof(left/right)), which always receives a non-null string literal and never validates the actual parameter.
  - Result: No null guard is emitted for reference-type operands; potential NullReferenceException and inconsistent behavior with other operators.
- Location:
  - _LEFT_NULL_CHECK and _RIGHT_NULL_CHECK
- Fix:
  - Use the value, not its name.
  - Replace:
    global::System.ArgumentNullException.ThrowIfNull(nameof(left));
    global::System.ArgumentNullException.ThrowIfNull(nameof(right));
  - With:
    global::System.ArgumentNullException.ThrowIfNull(left);
    global::System.ArgumentNullException.ThrowIfNull(right);

2) Typo/misleading identifier: typeLightNullCheck
- Problem:
  - Variable name typeLightNullCheck appears to be a typo of typeRightNullCheck.
  - While not functionally incorrect, it hurts readability/maintainability and is inconsistent with _RIGHT_NULL_CHECK.
- Locations:
  - GenerateImplementation local var and method parameters of GenerateOverloadsForKeyType.
- Fix:
  - Rename typeLightNullCheck to typeRightNullCheck and adjust parameter name accordingly.

3) Over-restrictive interface emission (Generic Math compatibility)
- Problem:
  - GenerateBaseTypes returns early unless _keyMemberOperators.HasOperator(ImplementedOperators.All).
  - This prevents implementing System.Numerics.ISubtractionOperators<TSelf, TOther, TResult> when only the default (unchecked) operator is available.
  - ISubtractionOperators only requires op_Subtraction; the checked form is optional and generated separately anyway.
  - Effect: Types that support subtraction but lack a checked variant won&#39;t implement the interface, degrading generic math compatibility and API consistency.
- Location:
  - GenerateBaseTypes: 
    if (!_keyMemberOperators.HasOperator(ImplementedOperators.All))
       return;
- Fix (suggested):
  - Gate interface emission on Default operator availability:
    if (!_keyMemberOperators.HasOperator(ImplementedOperators.Default))
       return;
  - Keep checked operator methods guarded as they are now.

4) Verify checked casting semantics with AppendCast (potential subtlety)
- Observation:
  - In checked variants, code composes: return Create(checked( AppendCast(...)(left.Key - right.Key) ));
  - If AppendCast emits a cast wrapper that includes the subtraction expression inside the checked(...) parentheses, overflow on the arithmetic is correctly checked.
  - If AppendCast were to close parentheses before the subtraction is appended, the arithmetic might not be inside the checked scope.
- Action:
  - Confirm AppendCast implementation ensures the arithmetic expression remains inside the checked(...) region. If not, adjust AppendCast or the calling pattern to keep the subtraction within checked.

5) Minor: naming consistency
- Observation:
  - CodeGeneratorName = "SubtractionOperators-CodeGenerator" and FileNameSuffix = ".SubtractionOperators".
  - Ensure this naming aligns with other operator code generators for consistency. Not a functional bug; note only.

Recommended patch sketch

- Null checks:
  private const string _LEFT_NULL_CHECK = "global::System.ArgumentNullException.ThrowIfNull(left);\n      ";
  private const string _RIGHT_NULL_CHECK = "global::System.ArgumentNullException.ThrowIfNull(right);\n      ";

- Interface gating:
  // before: if (!_keyMemberOperators.HasOperator(ImplementedOperators.All)) return;
  if (!_keyMemberOperators.HasOperator(ImplementedOperators.Default))
     return;

- Rename:
  typeLightNullCheck -> typeRightNullCheck (and update GenerateOverloadsForKeyType signature and call sites)
