MemberDeclarationSyntaxExtensions.cs – Issues

- Warning: Null receiver risk for extension method.
  - Details: Extension methods can be invoked with a null receiver (e.g., via reflection or atypical call paths), which would throw NullReferenceException when accessing tds.Modifiers.
  - Recommendation: If this API may be used across boundaries, add a guard or Debug.Assert. If strictly internal and validated by upstream contracts, this can be ignored.

- Minor: Name of parameter “tds” is misleading.
  - Details: The parameter is a MemberDeclarationSyntax, not strictly a TypeDeclarationSyntax.
  - Recommendation: Rename to “member” to improve readability.

- Minor: Imperative loop can be simplified for readability.
  - Details: The for-loop scans modifiers manually.
  - Recommendation: Use LINQ for clarity, e.g., return tds.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)); This is equivalent and more idiomatic. If avoiding LINQ allocations is important, the current loop is fine.

- Note: Syntactic check only.
  - Details: This inspects only the current declaration’s modifiers. That’s appropriate for “is declared partial” semantics; just ensure call sites don’t expect any broader/semantic inference.
  - Recommendation: Document the syntactic intent in summary/comments to avoid misuse.
