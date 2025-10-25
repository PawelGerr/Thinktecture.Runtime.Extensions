# ITypedMemberState.cs — Issues

- Missing `using Microsoft.CodeAnalysis;`
  - The interface references Roslyn types `SpecialType`, `TypeKind`, and `NullableAnnotation` without qualifying them and without a local `using Microsoft.CodeAnalysis;`.
  - A repository scan did not reveal a `global using Microsoft.CodeAnalysis;` (only sub-namespaces like `Microsoft.CodeAnalysis.CSharp.Syntax` and `Microsoft.CodeAnalysis.Diagnostics` are used). Without a local/global using (or fully-qualified names), this file will not compile (`CS0246`).
  - Fix: Add `using Microsoft.CodeAnalysis;` to this file, introduce a `GlobalUsings.cs` with `global using Microsoft.CodeAnalysis;`, or fully-qualify the types.

- Ambiguous/possibly unnecessary `IsToStringReturnTypeNullable`
  - In the BCL, `ToString()` returns non-null `string`. This flag is likely always `false` unless there is specific target framework metadata or custom APIs affecting nullability analysis.
  - If the generator uses this to emit nullable `string?` conditionally, verify with usage sites; otherwise consider removing to avoid confusion or documenting why it can ever be `true`.

- Equality contract not defined
  - The interface extends `IEquatable<ITypedMemberState>` but does not specify which properties participate in equality. Implementations can diverge, causing subtle bugs (e.g., collections using equality vs. code assuming identity).
  - Consider documenting the equality contract, providing a comparer, or exposing a base implementation to ensure consistency.
