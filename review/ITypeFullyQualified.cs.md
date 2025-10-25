Issues found in ITypeFullyQualified.cs

1) Warning: Ambiguous contract for “fully qualified”
- Problem: The property name does not define what “fully qualified” means in this context. Unspecified aspects include:
  - Whether the value is a code-safe display string (for generated source) or a metadata name (`TypeSymbol.ToDisplayString` vs `MetadataName`).
  - Whether it is prefixed with `global::` to avoid ambiguity in generated code.
  - How generics are represented (backtick-arity vs. type arguments recursively fully qualified).
  - Formatting of nested types (`+` vs `.`), arrays, pointers, by-ref (`ref/out/in`), tuples, function pointers.
  - Inclusion of nullable reference annotations (`string?`) and nullable value types (`int?`).
- Risk: Inconsistent implementations and invalid/ambiguous generated code (e.g., missing `global::`, using metadata name in source).
- Fix options:
  - Define precise invariants: e.g., “Code-ready fully qualified type name, always prefixed with `global::`, generic type arguments recursively fully qualified, nested types with `.`, never uses backtick-arity, includes array/pointer/byref syntax as in source. Includes `?` for annotated reference types where applicable.”
  - Provide examples in XML docs.

2) Warning: Potential confusion with Roslyn’s metadata name vs. display name
- Problem: Generators often need both the code display form and the metadata name (e.g., for symbol identity or diagnostics). A single property may encourage misuse or repeated recomputation.
- Fix options:
  - Expose a second property with explicit semantics, e.g., `string FullyQualifiedMetadataName` vs. `string FullyQualifiedDisplayName`.
  - Alternatively, rename current property to make semantics explicit (e.g., `FullyQualifiedTypeName` for code, and add a metadata counterpart when needed).

3) Warning: Naming consistency and discoverability
- Problem: `TypeFullyQualified` is unusual ordering in .NET naming; common patterns are `FullyQualifiedName` or `FullyQualifiedTypeName`.
- Risk: Reduced readability and API discoverability; potential inconsistency with the counterpart interface `ITypeMinimallyQualified` (if it uses a different naming pattern).
- Fix options:
  - Rename to `FullyQualifiedTypeName` or `FullyQualifiedName`. If public API, consider adding an alias and deprecating the old name with `[EditorBrowsable(EditorBrowsableState.Never)]`.

4) Warning: Contract does not guarantee non-null/non-empty
- Problem: The type is non-nullable `string`, but there is no stated invariant that it is non-empty/whitespace.
- Risk: Callers may need defensive checks; implementations may inadvertently return empty strings for unsupported types.
- Fix options:
  - Document the invariant: “never null or empty; throws for unsupported types,” or adjust to `string?` with clear semantics if `null` is possible.

5) Suggestion: Performance/caching guidance
- Problem: Implementations may repeatedly compute/allocate the string per access; the interface offers no guidance.
- Impact: Avoidable allocations during generation runs.
- Fix options:
  - Document expectation that implementations cache the computed value, or provide a factory/helper that computes once per symbol and stores it.
