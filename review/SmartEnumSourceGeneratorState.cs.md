Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/CodeAnalysis/SmartEnums/SmartEnumSourceGeneratorState.cs

1) HasDerivedTypes only considers inner derived types
- Details: HasDerivedTypes is computed via type.FindDerivedInnerTypes().Count > 0 which, by name, appears to check only nested/inner derived types.
- Impact: If derived smart-enum types are allowed outside the declaring type (e.g., sibling or external derived types), this flag will be false and may cause incorrect codegen decisions (e.g., sealing requirements, Switch/Map exhaustiveness generation).
- Recommendation: Clarify intended scope. If external derived types are valid, extend detection to non-nested derived types (within same compilation), or surface a diagnostic if only inner derivations are supported by design.

2) Equality/hash stability depends on element types’ value equality
- Details: Equals and GetHashCode rely on SequenceEqual/ComputeHashCode for:
  - AssignableInstanceFieldsAndProperties (InstanceMemberInfo)
  - ContainingTypes (ContainingTypeState)
  - DelegateMethods (DelegateMethodState)
  and Items (EnumItems.Equals/GetHashCode).
- Impact: If these element types don’t implement stable value equality/hash semantics, the state equality becomes reference-based or unstable, causing unnecessary incremental pipeline invalidations and repeated code generation.
- Recommendation: Ensure InstanceMemberInfo, ContainingTypeState, DelegateMethodState, and EnumItems implement consistent value-based equality and hash codes. If ordering is not guaranteed by Roslyn APIs in all cases, enforce a deterministic order before SequenceEqual/ComputeHashCode.

3) Possible unnecessary computation of AssignableInstanceFieldsAndProperties for Smart Enums
- Details: AssignableInstanceFieldsAndProperties is populated via GetAssignableFieldsAndPropertiesAndCheckForReadOnly(...). Smart Enums are supposed to have no assignable instance members; this computation may be unnecessary in most cases.
- Impact: Extra analysis work per type and potential diagnostics emission during state construction rather than in a dedicated validation pass.
- Recommendation: Consider deferring/guarding this computation (e.g., only when specific settings require it), or move related validations into a targeted analyzer/validation stage to reduce overhead.

4) Nullable-related properties may be misleading for class-only Smart Enums
- Details: IsNullableStruct is set using type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T (always false for class Smart Enums) and NullableAnnotation is captured from the declaration symbol (may not be meaningful for type declarations).
- Impact: These properties are included in equality/hash and could contribute to state differences without practical effect for class-only Smart Enums.
- Recommendation: If Smart Enums are class-only, consider excluding IsNullableStruct from equality/hash (it’s always false) and review whether NullableAnnotation on the declaration symbol is needed in equality. Alternatively, document why they are part of state identity if downstream codegen depends on them.

5) Public API shape suggests class-only Smart Enums but relies on external conventions
- Details: IsReferenceType hard-coded to true, IsStruct to false, DisallowsDefaultValue to false, IsRecord to false. This matches class-only Smart Enums but the enforcement is external (generator predicate), not this state.
- Impact: If future support for records/structs is desired, this will need changes across multiple components. If class-only is by design, consider a defensive diagnostic earlier in the pipeline to fail fast on non-class declarations (to avoid silent misconfigurations reaching this state).
- Recommendation: Add or ensure an explicit diagnostic at discovery time when a non-class is annotated with [SmartEnum], aligning the state’s assumptions with generator validation.
