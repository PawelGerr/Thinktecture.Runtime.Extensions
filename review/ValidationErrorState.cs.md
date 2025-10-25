Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValidationErrorState.cs

- Default(struct) instance is invalid and can cause NRE:
  - readonly struct with non-nullable string property `TypeFullyQualified` is not initialized for `default(ValidationErrorState)`. Methods like `GetHashCode()` and `ToString()` dereference it and will throw if a default instance is used (e.g., as a field default or in arrays).
  - Recommendation: Make the type a class to avoid invalid default state, or make methods null-safe (e.g., use `TypeFullyQualified ?? string.Empty`) and consider an `IsDefault/IsValid` property to guard usage. Alternatively, add analyzable guards wherever default could be produced/consumed.

- Missing constructor argument validation:
  - The constructor does not validate `typeFullyQualified`. Passing null (via reflection or misuse) will lead to NRE later (e.g., in `GetHashCode()`/`ToString()`), and passing empty/whitespace likely violates domain constraints for a fully-qualified type name.
  - Recommendation: Add `ArgumentException.ThrowIfNullOrWhiteSpace(typeFullyQualified);`.

- Hard-coded FQN string literal in `Default`:
  - `Default` uses a hard-coded `"global::Thinktecture.ValidationError"` which can drift if the type is renamed/moved or needs central configuration.
  - Recommendation: Centralize well-known type names in a single place (e.g., `Constants`) and reference them from here.

- String comparison/hash intent not explicit:
  - Equality uses `==` and hashing uses `GetHashCode()` on string which are ordinal by default. The intent is not documented.
  - Recommendation: If used in custom comparers/collections, consider using `StringComparer.Ordinal` explicitly where applicable to make the intended semantics obvious.
