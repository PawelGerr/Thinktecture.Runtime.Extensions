Issues found in ITypeInformationWithNullability.cs

1) Warning: Unqualified Roslyn type usage
- Problem: NullableAnnotation is referenced without namespace qualification or a local using directive. If there is no global using Microsoft.CodeAnalysis in the project, this will not compile.
- Fix options:
  - Add `using Microsoft.CodeAnalysis;` at the top of the file, or
  - Fully qualify the type: `Microsoft.CodeAnalysis.NullableAnnotation`.

2) Warning: Interface naming vs. inheritance consistency
- Problem: The name ITypeInformationWithNullability suggests it builds on ITypeInformation, but it inherits only ITypeFullyQualified and ITypeKindInformation.
- Risk: Consumers may expect ITypeInformation members to be available via this interface; potential inconsistency in the abstraction hierarchy.
- Fix options:
  - If intended, also inherit from ITypeInformation.
  - Otherwise, consider renaming to reflect the actual contract (e.g., ITypeNullabilityInfo).

3) Warning: Ambiguity of nullability contract
- Problem: The interface exposes both NullableAnnotation and IsNullableStruct but does not define how they interact. It&#39;s unclear:
  - For reference types, whether NullableAnnotation must be Annotated when the type is declared as T?.
  - For value types (Nullable<T>), what NullableAnnotation should be, and whether consumers should prefer IsNullableStruct for checks.
- Risk: Inconsistent implementations and consumer confusion.
- Fix options:
  - Define clear invariants in XML docs (e.g., "IsNullableStruct is true iff the type is Nullable<T>"; "For reference types, NullableAnnotation == Annotated iff T? is used").
  - Optionally add a normalized helper like `bool IsNullableReferenceType` to complement `IsNullableStruct`.
