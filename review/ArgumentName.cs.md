Errors
- default(ArgumentName) can cause NullReferenceException:
  - Equals(string other) calls Name.Equals(...), which throws if Name is null (default struct or null passed to Create).
  - GetHashCode() calls Name.GetHashCode(), which throws if Name is null.
  - ToString() returns Name; for a default instance this returns null, which is unexpected for ToString and can cause downstream issues.
  - Remediation: Enforce non-null invariant in the private constructor (ArgumentNullException.ThrowIfNull(name)) and/or make fields private with a validating factory. Additionally, harden methods against default by using Name is null checks (e.g., return false in Equals(string), return 0 in GetHashCode, return string.Empty in ToString) or prevent default by using a non-defaultable pattern.

Warnings
- Inconsistent equality semantics:
  - Equals(string other) uses OrdinalIgnoreCase, while Equals(ArgumentName other) and operator== use case-sensitive string comparison and include RenderAsIs in equality. This inconsistency can lead to subtle bugs where ArgumentName.Equals("name") is true but new ArgumentName("name") != new ArgumentName("Name").
  - Remediation: Decide on a single comparison semantic. If case-insensitive is intended, update Equals(ArgumentName), operator==, and GetHashCode to use StringComparer.OrdinalIgnoreCase. If case-sensitive is intended, change Equals(string) to case-sensitive and/or remove the overload to avoid confusion.

- Public API surface leakage from SourceGenerator assembly:
  - Type is public and placed in the root Thinktecture namespace, with public readonly fields. This unnecessarily expands the public surface of the analyzer/generator assembly and risks name collisions with consumer code that references Thinktecture.* namespaces. Public fields also reduce versioning flexibility.
  - Remediation: Make the type internal (and ideally move to an internal sub-namespace used by the source generator). Consider making fields private and exposing read-only properties if external access is ever needed.

- Non-nullable field with defaultable struct:
  - Name is declared as non-nullable string but can be null in default(ArgumentName), violating the declared nullability contract and leading to the errors noted above. This can also produce misleading nullability analysis.
  - Remediation: Prevent default, or relax the field to string? and handle nulls, or enforce non-null invariant as described above.
