# Review: CodeAnalysis/SwitchMapMethodsGeneration.cs

Issues found (errors/warnings only):

1) Risky sentinel value: None = -1 in a non-flags enum
- Problem: Negative sentinel for “None” is unusual for non-[Flags] enums and can introduce subtle bugs:
  - Range checks or comparisons (e.g., >= 0) may mis-handle None (-1).
  - Persisting/transporting the value via MSBuild properties or other config systems that expect non-negative integers can fail or behave inconsistently.
  - If serialized numerically (JSON/MessagePack), downstream consumers may not expect negative values.
- Recommendation: Prefer None = 0, Default = 1, DefaultWithPartialOverloads = 2. If compatibility requires -1, add explicit validation/tests and avoid numeric range comparisons; compare by value equality or parse by names.

2) Naming ambiguity: “DefaultWithPartialOverloads”
- Problem: “PartialOverloads” is unclear—does “partial” refer to C# partial methods, a subset of overloads, or overloads for partially-specified switch/map cases? Ambiguity can cause misuse and makes logs/review harder.
- Recommendation: Rename to an unambiguous term that reflects the exact behavior (e.g., DefaultWithSubsetOverloads, DefaultWithPartialCaseOverloads, or similar), and ensure usages/logs clarify the semantics.

3) Cross-enum consistency risk
- Problem: Pattern mirrors OperatorsGeneration (Default/DefaultWithXOverloads) but uses a different qualifier (“PartialOverloads”). If the semantics aren’t intentionally different, this inconsistency hampers discoverability and user intuition across options.
- Recommendation: Align naming patterns across option enums so “DefaultWith…Overloads” variants clearly and consistently communicate what extra overloads are generated.
