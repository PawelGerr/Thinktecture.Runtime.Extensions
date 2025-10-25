TypeSymbolExtensions.cs — Issues Found

- Warning: TryBuildMemberName does not handle type parameters (ITypeParameterSymbol).
  - Impact: For generic types that include unbound type parameters, TryBuildMemberName returns false, which can break downstream name generation for complex value objects/unions that are generic.
  - Suggestion: Add an explicit branch for ITypeParameterSymbol to use the parameter name (e.g., name = type.Name; return true).

- Warning: TryBuildMemberName for arrays ignores array rank (multi-dimensional arrays).
  - Impact: int[] and int[,] both become Int32Array (and e.g., string[] vs string[,] both become StringArray), leading to potential name collisions in generated members.
  - Suggestion: Incorporate rank into the name, e.g., elementName + "Array" for 1D, "Array2D" for rank 2, "Array3D" for rank 3, etc.

- Warning: GetEnumItems returns only static fields and ignores static properties.
  - Impact: Project documentation allows Smart Enum items as public static readonly/const fields or properties. If this method is used to enumerate items, property-based items will be missed.
  - Suggestion: Either (a) rename the method to clarify it returns fields only, or (b) extend it to also include eligible static properties of the enum type (not ignored, get-only) to match documented behavior.

- Warning: IterateAssignableFieldsAndPropertiesAndCheckForReadOnly downgrades severity to Warning for properties from other assemblies.
  - Code: ReportPropertyMustBeReadOnly(property, DiagnosticSeverity.Warning);
  - Impact: Diagnostics that should be Errors by design may be emitted as Warnings when the member originates from another assembly, potentially hiding important issues.
  - Suggestion: Confirm this is intentional. If not, align effectiveSeverity with the descriptor&#39;s DefaultSeverity or make the downgrade configurable via analyzer options.
