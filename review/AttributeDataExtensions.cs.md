File: src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/AttributeDataExtensions.cs

Issues found:

1) Enum parameter extraction assumes underlying int
- Severity: Warning
- Details: GetEnumParameterValue<T> checks TypedConstant.Value is int. Roslyn can box enum values using their underlying integral type; while your enums currently default to int, this is fragile if any enum switches to a different underlying type or Roslyn surfaces non-int underlying values. The method will return null and downstream code will silently fall back to defaults.
- Recommendation: Support TypedConstant.Kind == Enum and all integral underlying types. Example:
  - if (tc.Kind == TypedConstantKind.Enum) { var raw = Convert.ToInt64(tc.Value); return ((long)raw).GetValidValue<T>(); }
  - else if (tc.Value is sbyte/byte/short/ushort/int/uint/long/ulong) normalize via Convert.ToInt64 and map via GetValidValue<T>().

2) StopAt array handling drops all values if any element is unresolved
- Severity: Warning
- Details: FindUnionSwitchMapOverloadStopAtTypes returns an empty list as soon as a single array entry has TypeKind.Error. This discards other valid types in the same array, leading to surprising behavior.
- Recommendation: Skip only erroneous entries instead of returning an empty list. Continue collecting valid type symbols and ignore (or log) the erroneous ones.

3) Inconsistent use of string literals vs constants for named argument keys
- Severity: Maintainability
- Details: Some keys use Constants.Attributes.Properties (e.g., SKIP_KEY_MEMBER) while others are string literals (e.g., "DelegateName", "SkipFactoryMethods"). Typos will lead to silent nulls/defaults and are harder to detect.
- Recommendation: Centralize all known named argument keys into constants and reference them uniformly.

4) Repeated linear scans over NamedArguments
- Severity: Performance (low)
- Details: Each accessor calls FindNamedAttribute, which linearly scans NamedArguments. With many accessors per attribute and multiple attributes processed, this can add up.
- Recommendation: Consider building a Dictionary<string, TypedConstant> per AttributeData once and reading from it. Alternatively, pass a cached lookup into helper methods when hot paths are identified.

5) Method name FindNamedAttribute is misleading
- Severity: Maintainability (low)
- Details: The helper returns a named argument TypedConstant, not an attribute. The name may confuse readers and future contributors.
- Recommendation: Rename to FindNamedArgument or GetNamedArgumentValue to reflect the actual behavior.

6) AggressiveInlining on small helpers
- Severity: Maintainability (low)
- Details: [MethodImplOptions.AggressiveInlining] on trivial helpers may not provide measurable benefit and can hinder debugging or JIT heuristics.
- Recommendation: Remove unless a benchmark demonstrates a benefit in source generator hot paths.
