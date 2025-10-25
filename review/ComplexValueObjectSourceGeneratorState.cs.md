Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/ValueObjects/ComplexValueObjectSourceGeneratorState.cs

1) Type-level DisallowsDefaultValue ignores member-level constraints
- Location: Property `DisallowsDefaultValue`
- Code: `public bool DisallowsDefaultValue => !IsReferenceType && !Settings.AllowDefaultStructs;`
- Details: For complex structs, this returns false (i.e., allows default) when `AllowDefaultStructs == true`, even if some members disallow default values (e.g., non-nullable reference members or members marked to disallow defaults). The serializers and code generators do check per-member `DisallowsDefaultValue`, but using a permissive type-level flag can lead to inconsistent behavior (e.g., default(struct) may be considered allowed at the type level while member-level checks will then reject during use).
- Impact: Potentially inconsistent codegen behavior and validation paths for default(struct) instances when any member disallows default.
- Suggested fix: Fold member-level constraints into the type-level flag, for example:
  `public bool DisallowsDefaultValue => !IsReferenceType && (!Settings.AllowDefaultStructs || AssignableInstanceFieldsAndProperties.Any(m => !m.IsReferenceTypeOrNullableStruct && m.DisallowsDefaultValue || m.IsReferenceTypeOrNullableStruct && m.DisallowsDefaultValue));`
  Alternatively, compute once in ctor and store a bool if performance/allocation matters.

2) Inconsistent detection of ValidateFactoryArguments method compared to keyed VO state
- Location: Ctor (assignment of `FactoryValidationReturnType`)
- Code: `var factoryValidationReturnType = (type.GetMembers().FirstOrDefault(m => m.IsStatic && m.Name == Constants.Methods.VALIDATE_FACTORY_ARGUMENTS && m is IMethodSymbol method && method.ReturnType.SpecialType != SpecialType.System_Void) as IMethodSymbol)?.ReturnType;`
- Details: The keyed VO state uses `member.IsValidateFactoryArgumentsImplementation(out var method)` to detect the implementation, which likely encapsulates signature checks and non-static cases. Here, detection is limited to static methods with the exact name, risking missed matches if the implementation shape is broader (or future changes extend support).
- Impact: Missed detection of `ValidateFactoryArguments` for complex value objects under legitimate shapes, leading to missing `FactoryValidationReturnType` and downstream codegen differences.
- Suggested fix: Align with keyed VO: reuse the same helper (`IsValidateFactoryArgumentsImplementation`) for robust detection and maintain consistency across VO kinds.

3) Equality/HashCode omit nullability flags that might influence codegen
- Location: `Equals` / `GetHashCode`
- Details: `NullableAnnotation` and `IsNullableStruct` are not included in equality/hash. If any generator paths (present or future) condition codegen on these flags, the incremental driver may fail to regenerate affected code on nullability changes.
- Impact: Potential stale generated code when only nullability changes.
- Suggested fix: If nullability can affect generated code, include `NullableAnnotation` and `IsNullableStruct` in equality and hash code. If guaranteed to be irrelevant, this item can be ignored.
