ValueObjectCodeGeneratorFactory.cs – Issues

1) Inconsistent and limited equality semantics (overloads without overriding object.Equals/GetHashCode)
- Severity: Warning
- Location: Equals(ICodeGeneratorFactory<KeyedValueObjectSourceGeneratorState>?), Equals(ICodeGeneratorFactory<ComplexValueObjectSourceGeneratorState>)
- Details:
  - The class adds two Equals overloads but does not override object.Equals nor implement IEquatable<ValueObjectCodeGeneratorFactory>.
  - This yields inconsistent behavior depending on the static type of the caller:
    - If variables are typed as the specific interface, the overload is used (reference equality).
    - If variables are typed as object or a different interface, object.Equals is used (also reference equality, but the overloads won’t be considered).
  - Additionally, one overload accepts nullable (Keyed variant) while the other does not (Complex variant), causing nullability inconsistency.
- Suggested fix:
  - Prefer removing these overloads and rely on reference equality via the singleton Instance; OR
  - Implement IEquatable<ValueObjectCodeGeneratorFactory> and override object.Equals/GetHashCode consistently.
  - Align nullability for any retained overloads (either both nullable or both non-nullable).

2) CodeGeneratorName is generic and may reduce traceability in logs
- Severity: Minor
- Location: CodeGeneratorName => "ValueObject-CodeGenerator"
- Details: This factory produces both keyed and complex VO generators; the generic name may be ambiguous in logs/diagnostics.
- Suggested fix:
  - Either keep as-is but include the concrete generator’s name in emitted files/logs elsewhere, or
  - Consider a slightly more specific name (e.g., "ValueObject-Core-CodeGenerator") if that aids diagnostics.

Tests to add
- Equality semantics:
  - When variables are typed as ICodeGeneratorFactory<KeyedValueObjectSourceGeneratorState>, factory.Equals(other) is true only for the same Instance.
  - When variables are typed as object, equality behavior is consistent with the above (after implementing object.Equals/GetHashCode or removing overloads).
  - Verify null argument behavior does not produce nullable warnings (align overload nullability).
