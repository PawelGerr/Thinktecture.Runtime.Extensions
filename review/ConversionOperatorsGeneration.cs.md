# Review: CodeAnalysis/ConversionOperatorsGeneration.cs

Issues found (errors/warnings only):

1) Design limitation: cannot express “both” implicit and explicit
- Problem: The enum is non-flags with mutually exclusive values (None, Implicit, Explicit). If a scenario requires generating both implicit and explicit conversions, there is no way to represent it.
- Recommendation: Either switch to [Flags] with values None = 0, Implicit = 1, Explicit = 2, Both = Implicit | Explicit; or add a dedicated Both = 3 option if flags are not desired. Add tests to ensure combined behavior is handled correctly.

2) Option style inconsistency vs. other enums
- Problem: Other option enums (e.g., OperatorsGeneration) include a Default value. Here, there is no Default, so default(T) resolves to None (0). This may be surprising and can cause silent misconfiguration if a default should actually generate conversions.
- Recommendation: Align option patterns: either add Default and define its semantics, or document that None is intended as the default. Consider validating/deserializing by name to avoid accidental numeric defaults.

3) Semantics ambiguity: directionality not explicit
- Problem: It’s unclear whether “Implicit”/“Explicit” applies to conversions to the key type, from the key type, or both directions. This can lead to misuse and hard-to-debug generator output.
- Recommendation: Clarify with XML docs and/or separate directional options if needed (e.g., ToKeyImplicit, FromKeyImplicit, etc.), or document that each option covers both directions.
