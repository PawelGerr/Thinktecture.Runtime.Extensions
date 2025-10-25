Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SourceGenError.cs

- Default(record struct) permits invalid null state:
  - As a record struct, `default(SourceGenError)` yields `Message == null` and `Node == null` even though both are declared non-nullable. Any later dereference (e.g., `Message.GetHashCode()` or use of `Node`) can throw.
  - Recommendation: Use a class to avoid invalid default, or add `IsDefault/IsValid` pattern and guard usage. Alternatively, make the primary constructor internal and provide validated factory methods.

- Missing constructor argument validation:
  - No checks for `Message` or `Node`. Reflection/misuse could pass null, leading to NRE downstream.
  - Recommendation: Enforce `ArgumentException.ThrowIfNullOrWhiteSpace(message);` and `ArgumentNullException.ThrowIfNull(node);`.

- Holding onto full Syntax node can retain large syntax trees:
  - Storing `TypeDeclarationSyntax` can keep the entire `SyntaxTree` alive and increase memory pressure in long-lived caches.
  - Recommendation: Store a `Location`, `SyntaxReference`, or minimal identity (e.g., `FilePath`, `TextSpan`) instead of the full node when possible.

- Limited diagnostic fidelity:
  - Only carries a message and node. No diagnostic ID, category, severity, or additional locations, which reduces usefulness when reporting via `Diagnostic`.
  - Recommendation: Add optional fields like `Id`, `Severity`, `Category`, `Location` to integrate cleanly with `SourceProductionContext.ReportDiagnostic`.

- CSharp-specific node type:
  - Ties the error to `TypeDeclarationSyntax` specifically, limiting reuse for other syntax contexts (e.g., members, parameters) and making it C#-specific.
  - Recommendation: Generalize to `SyntaxNode` (or store `Location`) if broader applicability is intended.

- Consistency with project conventions:
  - Many CodeAnalysis state types implement `IHashCodeComputable` for deterministic hashing consistency; this type does not and relies on record-generated hashing.
  - Recommendation: Consider implementing `IHashCodeComputable` if uniformity across state objects is desired.
