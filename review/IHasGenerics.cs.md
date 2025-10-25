- Errors
  - Missing using for IReadOnlyList. If ImplicitUsings or a global using for System.Collections.Generic is not enabled for this project/target framework, the interface will not compile. Remediation: add `using System.Collections.Generic;` or fully-qualify (`System.Collections.Generic.IReadOnlyList<...>`).

- Warnings
  - Interface couples to a concrete state type: `IReadOnlyList<GenericTypeParameterState> GenericParameters`. This leaks implementation details and increases coupling across layers. Consider returning an abstraction (e.g., `IGenericTypeParameter`) or a minimal shape required by consumers.
  - Naming clarity: `IHasGenerics` is ambiguous; “generics” could mean types or methods. The property is `GenericParameters`, suggesting the interface should be named `IHasGenericParameters` for precision and consistency.
  - Contract ambiguity: It is not specified whether `GenericParameters` includes only the current type’s generic parameters or also those from containing types (for nested types) or constraints ordering. Ambiguity can lead to incorrect code generation. Clarify scope (current-type-only vs. transitive/combined) and ordering guarantees.
