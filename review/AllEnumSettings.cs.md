AllEnumSettings.cs — Review (issues only)

Summary
- Scope: src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/AllEnumSettings.cs
- Focus: correctness, nullability, robustness, invariants, incremental-safety.

Issues

1) Nullability bug: SwitchMapStateParameterName may be null
- Problem: SwitchMapStateParameterName is declared non-nullable string but assigned from attribute.FindSwitchMapStateParameterName() with no fallback. If the attribute doesn’t provide a value, this will be null, violating NRT and causing a NullReferenceException in GetHashCode via SwitchMapStateParameterName.GetHashCode().
- Impact: High. A missing attribute argument will surface as runtime exception during generator execution or equality/hash use in incremental pipelines.
- Fix:
  - Provide a default (per project guidance: default parameter name should be "state").
  - Also defensively compute hash using a comparer.
  Example patch:
  ```
  SwitchMapStateParameterName = attribute.FindSwitchMapStateParameterName() ?? Constants.SmartEnum.DEFAULT_SWITCH_MAP_STATE_PARAMETER_NAME; // e.g. "state"
  ```
  and in GetHashCode prefer:
  ```
  hashCode = (hashCode * 397) ^ StringComparer.Ordinal.GetHashCode(SwitchMapStateParameterName);
  ```

2) Missing/implicit defaults for SwitchMethods and MapMethods
- Problem: SwitchMethods = attribute.FindSwitchMethods(); MapMethods = attribute.FindMapMethods(); have no explicit fallback while most other settings do (with ??). If the Find* extensions can return null/unspecified, these fields can become uninitialized logically (or rely on implicit default semantics not visible here).
- Impact: Medium. Inconsistent defaulting can lead to unexpected behavior if the extension methods ever return default/null.
- Fix: Mirror the pattern used for other settings with explicit defaults:
  ```
  SwitchMethods = attribute.FindSwitchMethods() ?? Constants.SmartEnum.DEFAULT_SWITCH_METHODS;
  MapMethods = attribute.FindMapMethods() ?? Constants.SmartEnum.DEFAULT_MAP_METHODS;
  ```

3) Reliance on enum numeric ordering: ComparisonOperators > EqualityComparisonOperators
- Problem: The post-condition
  ```
  if (ComparisonOperators > EqualityComparisonOperators)
      EqualityComparisonOperators = ComparisonOperators;
  ```
  assumes OperatorsGeneration has a meaningful ordered scale. If OperatorsGeneration is [Flags] or its numeric ordering changes, this logic is brittle or incorrect.
- Impact: Medium. Could silently misconfigure generation if enum semantics differ from assumed ordering.
- Fix: Replace ordering comparison with intent-based logic. Options:
  - If OperatorsGeneration is not flags, introduce a Max helper that encodes intended precedence explicitly.
  - If OperatorsGeneration is flags, ensure equality covers at least all flags required by comparison:
    ```
    EqualityComparisonOperators |= ComparisonOperators;
    ```
  Add unit tests to assert the intended invariant across all enum values.

4) Potential empty-string inputs for identifiers
- Problem: KeyMemberName and SwitchMapStateParameterName can be provided as empty/whitespace by attributes. KeyMemberName has a null fallback but no normalization for empty/whitespace. This can generate invalid identifiers and/or break codegen.
- Impact: Medium for DX robustness.
- Fix: Normalize both names:
  ```
  var keyName = attribute.FindKeyMemberName();
  KeyMemberName = string.IsNullOrWhiteSpace(keyName)
      ? Helper.GetDefaultSmartEnumKeyMemberName(KeyMemberAccessModifier, KeyMemberKind)
      : keyName;

  var stateName = attribute.FindSwitchMapStateParameterName();
  SwitchMapStateParameterName = string.IsNullOrWhiteSpace(stateName)
      ? Constants.SmartEnum.DEFAULT_SWITCH_MAP_STATE_PARAMETER_NAME
      : stateName;
  ```

5) String hashing and equals clarity
- Observation: GetHashCode uses KeyMemberName.GetHashCode() and SwitchMapStateParameterName.GetHashCode(). Using string.GetHashCode is process-randomized on modern .NET; it&#39;s fine within a single generator process but intent is clearer with StringComparer.Ordinal.
- Impact: Low. Within the generator process this works; however using StringComparer.Ordinal avoids surprises if hashing strategy changes.
- Fix (optional but recommended):
  ```
  var sc = StringComparer.Ordinal;
  hashCode = (hashCode * 397) ^ sc.GetHashCode(KeyMemberName);
  hashCode = (hashCode * 397) ^ sc.GetHashCode(SwitchMapStateParameterName);
  ```
  And in Equals, use string.Equals(x, y, StringComparison.Ordinal) for explicitness.

Suggested Tests
- SwitchMapStateParameterName_defaulted_when_missing:
  - Construct settings from attribute without state parameter; assert SwitchMapStateParameterName == "state" (or default constant).
  - Assert GetHashCode does not throw.
- SwitchMap_and_Switch_methods_defaults:
  - Attributes without Switch/Map args produce expected defaults.
- Comparison_vs_Equality_invariant:
  - For all enum values (or representative ones), ensure resulting EqualityComparisonOperators covers the expected subset based on intended semantics (flags: bitwise superset; non-flags: expected precedence).
- Identifier_normalization:
  - Empty/whitespace KeyMemberName/StateParameterName fall back to defaults.

Risk Assessment
- High: Nullability bug on SwitchMapStateParameterName leading to NRE in GetHashCode.
- Medium: Enum ordering reliance; missing explicit defaults for Switch/Map; empty identifier handling.
- Low: String hashing clarity.

Proposed Patch (condensed)
```
SwitchMethods = attribute.FindSwitchMethods() ?? Constants.SmartEnum.DEFAULT_SWITCH_METHODS;
MapMethods = attribute.FindMapMethods() ?? Constants.SmartEnum.DEFAULT_MAP_METHODS;
SwitchMapStateParameterName = attribute.FindSwitchMapStateParameterName() ?? Constants.SmartEnum.DEFAULT_SWITCH_MAP_STATE_PARAMETER_NAME;

if (string.IsNullOrWhiteSpace(KeyMemberName))
   KeyMemberName = Helper.GetDefaultSmartEnumKeyMemberName(KeyMemberAccessModifier, KeyMemberKind);

if (string.IsNullOrWhiteSpace(SwitchMapStateParameterName))
   SwitchMapStateParameterName = Constants.SmartEnum.DEFAULT_SWITCH_MAP_STATE_PARAMETER_NAME;

// If OperatorsGeneration is flags:
EqualityComparisonOperators |= ComparisonOperators;
// else use a dedicated Max helper that codifies precedence.
