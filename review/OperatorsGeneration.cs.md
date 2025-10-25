# Review: CodeAnalysis/OperatorsGeneration.cs

Issues found (errors/warnings only):

1) Risky sentinel value: None = -1 in a non-flags enum
- Problem: Negative sentinel for “None” is unusual for non-[Flags] enums and can introduce subtle bugs:
  - Range checks or comparisons (e.g., > 0) may accidentally treat Default (0) and None (-1) similarly or incorrectly.
  - Persisting the enum as numeric in MSBuild properties or other config channels that expect non-negative integers can fail or be mishandled.
  - If serialized to numeric (e.g., JSON/MessagePack), downstream consumers might not expect negative values for this setting.
- Recommendation:
  - If feasible, prefer None = 0, Default = 1, DefaultWithKeyTypeOverloads = 2. If not feasible due to compatibility, add explicit validation/tests in option parsing to accept -1 and avoid numeric comparisons; compare by value equality or parse by names only.

2) Naming clarity
- Problem: “DefaultWithKeyTypeOverloads” is long and slightly ambiguous. Without context, it’s not obvious what “Default” comprises and what additional behavior “KeyTypeOverloads” brings.
- Recommendation: Consider a more explicit name such as DefaultAndKeyTypeOverloads or EnabledWithKeyTypeOverloads, and/or add concise XML docs where it’s defined/consumed. Low severity, but improves readability and reduces misuse.
