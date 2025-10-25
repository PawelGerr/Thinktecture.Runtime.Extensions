- Errors
  - Missing using for IReadOnlyList. If ImplicitUsings or a global using for System.Collections.Generic is not enabled for this project/target framework, the interface will not compile. Remediation: add `using System.Collections.Generic;` at the top or fully-qualify the type (`System.Collections.Generic.IReadOnlyList<...>`).

- Warnings
  - Interface couples to concrete state type: `IReadOnlyList<ContainingTypeState> ContainingTypes`. Using a state class in an interface surface leaks implementation details and increases coupling across layers. Consider exposing an abstraction (e.g., `IContainingType`) or a minimal shape that represents only required data (e.g., name + arity).
  - Ambiguous semantics of `NumberOfGenerics`. For nested types, generic arity is per-type (outer and inner can each have arity). A single `int` may be misinterpreted (own arity vs. total), which can lead to incorrect name/arity handling in generators. Consider clarifying the contract (e.g., rename to `GenericArity` and document it represents only the current type's arity) and ensure per-containing-type arity is available via `ContainingTypes`.
