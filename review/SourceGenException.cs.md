Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SourceGenException.cs

- Default(record struct) permits invalid null state:
  - As a record struct, `default(SourceGenException)` yields `Exception == null` and `Node == null` even though both are declared non-nullable. Any later dereference will throw.
  - Recommendation: Use a class to avoid invalid default, or add an `IsDefault/IsValid` pattern and guard usage. Alternatively, make the primary constructor internal and expose validated factory methods.

- Missing constructor argument validation:
  - No validation for `Exception` or `Node`. Reflection/misuse can pass null leading to NREs later.
  - Recommendation: Enforce `ArgumentNullException.ThrowIfNull(exception);` and `ArgumentNullException.ThrowIfNull(node);`.

- Holding full syntax node retains large trees:
  - Storing `TypeDeclarationSyntax` can keep the entire `SyntaxTree` alive, increasing memory usage for long-lived caches/collections.
  - Recommendation: Prefer lightweight identity like `Location`, `SyntaxReference`, or `(FilePath, TextSpan)`.

- Exception payload can be heavy and non-deterministic:
  - Capturing raw `Exception` may pull in large graphs (InnerException chains, Data dictionaries, StackTrace) and is not serializable/deterministic across processes.
  - Recommendation: Consider storing a structured error snapshot (Message, StackTrace string, HResult, Inner message) if later uses require serialization/diagnostic reporting, or keep raw `Exception` only transiently.

- Limited diagnostic metadata:
  - Only carries `Exception` and node; lacks diagnostic ID, severity, category, or additional locations, limiting integration with `Diagnostic` reporting.
  - Recommendation: Add optional fields or companion conversion logic to `DiagnosticDescriptor`/`Diagnostic` to integrate well with `SourceProductionContext.ReportDiagnostic`.

- CSharp-specific node type:
  - Uses `TypeDeclarationSyntax` which limits applicability to type declarations and to C#. Other error contexts might require broader `SyntaxNode` or `Location`.
  - Recommendation: Generalize to `SyntaxNode` or store only `Location` if sufficient.
